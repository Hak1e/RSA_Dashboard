using System;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Dashboard
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        #region Поля

        private string sourcePath = string.Empty;
        private string newDirPath = string.Empty;
        private string keysPath = string.Empty;
        private string publicKey = string.Empty;
        private string privateKey = string.Empty;
        private string decNewPath = string.Empty;
        private byte[] encryptedData;
        private byte[] decryptedData;
        private byte[] data;
        private string publicxml = "";
        private string privatexml = "";
        private string dividedPath_Temp = string.Empty;
        private readonly int partSize = 112;
        private readonly int encSize = 128;
        private string[] keysArr = new string[2];
        private bool operationCanceled = false;
        private string keysName = string.Empty;
        private int totalDivisionCompleted;
        private int totalMergingCompleted;
        private int totalEncryptionCompleted;
        private int totalDecryptionCompleted;
        private readonly int totalAmountOfProcesses = 3; // 1) разделение файла на части; 2) шифрация / расшифровка; 3) сложение файлов в единое целое

        #endregion

        #region Ключи
        private void CreateKey()
        {
            try
            {
                var rsaKey = new RSACryptoServiceProvider();
                string rsaPublicKey = rsaKey.ToXmlString(false);
                string rsaPrivateKey = rsaKey.ToXmlString(true);
                operationCanceled = false;

                if (keysPath != string.Empty)
                {
                    Directory.CreateDirectory(keysPath);
                }
                if (keysName != string.Empty)
                {
                    void GetKeysNameFromFolder()
                    {
                        publicKey = $@"\{keysName}_public.xml";
                        privateKey = $@"\{keysName}_private.xml";
                    }

                    void CreateKeysInFolder()
                    {
                        File.WriteAllText(keysPath + $@"\{keysName}_public.xml", rsaPublicKey, Encoding.UTF8);
                        File.WriteAllText(keysPath + $@"\{keysName}_private.xml", rsaPrivateKey, Encoding.UTF8);
                    }

                    if (!File.Exists(keysPath + $@"\{keysName}_public.xml") && !File.Exists(keysPath + $@"\{keysName}_private.xml"))
                    {
                        CreateKeysInFolder();
                        GetKeysNameFromFolder();
                    }
                    else
                    {
                        var window = MessageBox.Show("Keys with this name are already exists. Do you want to rewrite them?\n", "Same names",
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (window == DialogResult.Yes)
                        {
                            CreateKeysInFolder();
                            GetKeysNameFromFolder();
                        }
                        else if (window == DialogResult.No)
                        {
                            RtbLogs.Text += "Using previous keys\n";
                            GetKeysNameFromFolder();
                        }
                        else if (window == DialogResult.Cancel)
                        {
                            operationCanceled = true;
                            RtbLogs.Text += "Operation canceled\n";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadKey()
        {
            try
            {
                publicxml = File.ReadAllText(keysPath + @"\" + publicKey, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load public key\nChoose the right key\n" + ex.Message);
                return;
            }
            try
            {
                privatexml = File.ReadAllText(keysPath + @"\" + privateKey, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load private key\nChoose the right key\n" + ex.Message);
                return;
            }
        }

        #endregion

        #region Кнопки путей
        private void BtPathSourceDir_Click(object sender, EventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = @"C:\"
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                sourcePath = dialog.FileName;
                TbSourceDir.Text = sourcePath;
                ChbSourceDir.Checked = true;
            }
        }

        private void BtPathNewDir_Click(object sender, EventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = @"C:\"
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                newDirPath = dialog.FileName;
                keysName = Path.GetFileName(newDirPath);
                TbNewDir.Text = newDirPath;
                ChbNewDir.Checked = true;
                keysPath = dialog.FileName + @"\Keys";
                TbKeysDir.Text = keysPath;
                decNewPath = newDirPath + @"\Decrypted";
            }
        }

        private void BtPathKeys_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                keysArr = dialog.FileNames;
                foreach (var key in keysArr) // List?
                {
                    if (key.Contains("_public.xml")) publicKey = Path.GetFileName(key);
                    else if (key.Contains("_private.xml")) privateKey = Path.GetFileName(key);
                    keysPath = Path.GetDirectoryName(key);
                }
                TbKeysDir.Text = keysPath;
                RtbLogs.Text += $"Public key is {publicKey}\nPrivate key is {privateKey}\n";
            }
        }

        #endregion

        private void DivideFile(FileInfo dividingFile, string dirPath, int _partSize)
        {
            byte[] fullFile = File.ReadAllBytes(dirPath + @"\" + dividingFile.Name);
            int currentPart = 1;
            int currentPosition = 0;
            int lastFileSize = fullFile.Length % _partSize;
            int totalNumberOfFiles = fullFile.Length / _partSize + 1;

            dividedPath_Temp = newDirPath + $@"\Divided";
            if(Directory.Exists(dividedPath_Temp))
                Directory.Delete(dividedPath_Temp, true);
            DirectoryInfo dirInfo = Directory.CreateDirectory(dividedPath_Temp);
            dirInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            for (int currentSize = 0; currentSize < fullFile.Length; currentSize += _partSize)
            {
                byte[] partBytes = new byte[Math.Min(_partSize, fullFile.Length - currentSize)];

                for (int position = 0; position < partBytes.Length; position++) // записываем часть целого файла в новый файл
                {
                    partBytes[position] = fullFile[currentPosition++];
                }
                File.WriteAllBytes(dividedPath_Temp + $@"\{dividingFile.Name}_" + currentPart + ".part", partBytes);

                totalDivisionCompleted = currentPart * 100 / (totalNumberOfFiles * totalAmountOfProcesses);
                LbProcess.Invoke(new Action(() => LbProcess.Text = $"Dividing file. {totalDivisionCompleted}% has done"));
                currentPart++;
            }
        }

        private void MergeFile(string _newDirPath, string _dividedPath, int totalDecryptionOrEncryptionCompleted)
        {
            var filesInCurrentDirectory = new DirectoryInfo(_dividedPath);
            FileInfo[] filesInDirectory = filesInCurrentDirectory.GetFiles();
            string startName = string.Empty;
            long summaryLength = 0;
            int filesCount = filesInDirectory.Count();
            foreach (var partFile in filesInDirectory)
            {
                summaryLength += partFile.Length;
            }
            if(summaryLength == 0)
            {
                RtbLogs.Text += "No files in directory";
                return;
            }

            foreach (var currentFile in filesInDirectory)
            {
                if (currentFile.Name.Contains("_1.part"))
                {
                    startName = currentFile.Name.Remove(currentFile.Name.LastIndexOf("_1.part"));
                    break;
                }
            }
            try
            {
                byte[] partBytes = new byte[summaryLength];
                int position = 0;
                for (int fileNumber = 1; fileNumber <= filesCount; fileNumber++)
                {
                    byte[] partFile = File.ReadAllBytes(_dividedPath + @"\" + $"{startName}_" + fileNumber + ".part");
                    int fileSize = partFile.Length;

                    for (int partPosition = 0; partPosition < fileSize; partPosition++, position++) // записываем все байты части в массив
                    {
                        partBytes[position] = partFile[partPosition];
                    }
                    totalMergingCompleted = fileNumber * 100 / (filesCount * totalAmountOfProcesses);
                    LbProcess.Invoke(new Action(() => 
                    LbProcess.Text = $"Merging file. " +
                    $"{totalDivisionCompleted + totalDecryptionOrEncryptionCompleted + totalMergingCompleted}% has done"));
                }
                File.WriteAllBytes(_newDirPath + $@"\{startName}", partBytes);
                LbProcess.Invoke(new Action(() => LbProcess.Text = "Merging file. 100% has done"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private async void BtEncrypt_Click(object sender, EventArgs e)
        {
            if (ChbSourceDir.Checked == true && ChbNewDir.Checked == true)
            {
                var rsa = new RSACryptoServiceProvider();
                CreateKey();
                if (operationCanceled) return;
                LoadKey();
                if (publicxml.Length == 0)
                {
                    MessageBox.Show("Invalid public key");
                    return;
                }
                try
                {
                    rsa.FromXmlString(publicxml);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Problems with RSA\n" + ex.Message);
                }

                var filesInCurrentDirectory = new DirectoryInfo(sourcePath);
                FileInfo[] filesInDirectory = filesInCurrentDirectory.GetFiles();
                
                foreach (var currentFile in filesInDirectory)
                {
                    int currentFilePosition = 1;
                    data = new byte[partSize];
                    try
                    {
                        if (currentFile.Length > 117)
                        {
                            RtbLogs.Text += $"[{DateTime.Now:HH:mm:ss}] Dividing {currentFile}\n";
                            await Task.Run(() => DivideFile(currentFile, sourcePath, partSize));
                            var filesInDividedDirectory = new DirectoryInfo(dividedPath_Temp);
                            FileInfo[] filesInDivDir = filesInDividedDirectory.GetFiles();
                            int totalNumberOfFiles = filesInDivDir.Count();

                            foreach (var divFile in filesInDivDir)
                            {
                                await Task.Run(() =>
                                {
                                    data = File.ReadAllBytes(dividedPath_Temp + @"\" + divFile.Name);
                                    encryptedData = rsa.Encrypt(data, false);
                                    File.WriteAllBytes(dividedPath_Temp + @"\" + divFile.Name, encryptedData);

                                    totalEncryptionCompleted = currentFilePosition * 100 / (totalNumberOfFiles * totalAmountOfProcesses);
                                    LbProcess.Invoke(new Action(() => 
                                    LbProcess.Text = $"Encrypting. {totalDivisionCompleted + totalEncryptionCompleted}% has done"));
                                    currentFilePosition++;
                                });
                            }
                            await Task.Run(() =>
                            {
                                MergeFile(newDirPath, dividedPath_Temp, totalEncryptionCompleted);
                                RtbLogs.Text += $"[{DateTime.Now:HH:mm:ss}] {currentFile.Name} encrypted\n";
                                Directory.Delete(dividedPath_Temp, true);
                                LbProcess.Invoke(new Action(() => LbProcess.Text = "Done!"));
                            });
                        }
                        else
                        {
                            await Task.Run(() =>
                            {
                                data = File.ReadAllBytes(sourcePath + @"\" + currentFile.Name);
                                encryptedData = rsa.Encrypt(data, false);
                                File.WriteAllBytes(newDirPath + @"\" + currentFile.Name, encryptedData);
                                RtbLogs.Text += $"[{DateTime.Now:HH:mm:ss}] {currentFile.Name} encrypted\n";
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        RtbLogs.Text += $"{currentFile.Name} is not encrypted: {ex.Message}\n";
                    }
                }
            }
            else
            {
                MessageBox.Show("Choose path to source directory\n");
                return;
            }
        }

        private async void BtDecrypt_Click(object sender, EventArgs e)
        {
            if (ChbNewDir.Checked == true)
            {
                LoadKey();
                if (privatexml.Length == 0)
                {
                    MessageBox.Show("Invalid private key\n");
                    return;
                }
                var rsa = new RSACryptoServiceProvider();
                var filesInCurrentDirectory = new DirectoryInfo(newDirPath);
                if (!Directory.Exists(newDirPath))
                {
                    MessageBox.Show("New directory is not exist");
                    return;
                }
                FileInfo[] filesInDirectory = filesInCurrentDirectory.GetFiles();

                try
                {
                    rsa.FromXmlString(privatexml);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Problems with RSA\n" + ex.Message);
                }
                int currentFilePosition = 1;
                foreach (var currentFile in filesInDirectory)
                {
                    data = new byte[encSize];
                    try
                    {
                        if (currentFile.Length > 128)
                        {
                            RtbLogs.Text += $"[{DateTime.Now:HH:mm:ss}] Dividing {currentFile}\n";
                            await Task.Run(() => DivideFile(currentFile, newDirPath, encSize));
                            var filesInDividedDirectory = new DirectoryInfo(dividedPath_Temp);
                            FileInfo[] filesInDivDir = filesInDividedDirectory.GetFiles();
                            int totalNumberOfFiles = filesInDivDir.Count();

                            foreach (var divFile in filesInDivDir)
                            {
                                await Task.Run(() =>
                                {
                                    data = File.ReadAllBytes(dividedPath_Temp + @"\" + divFile.Name);
                                    decryptedData = rsa.Decrypt(data, false);
                                    File.WriteAllBytes(dividedPath_Temp + @"\" + divFile.Name, decryptedData);
                                    totalDecryptionCompleted = currentFilePosition * 100 / (totalNumberOfFiles * totalAmountOfProcesses);
                                    LbProcess.Invoke(new Action(() => 
                                    LbProcess.Text = $"Decrypting. {totalDivisionCompleted + totalDecryptionCompleted}% has done"));
                                    currentFilePosition++;
                                });
                            }
                            await Task.Run(() =>
                            {
                                Directory.CreateDirectory(decNewPath);
                                MergeFile(decNewPath, dividedPath_Temp, totalDecryptionCompleted);
                                RtbLogs.Text += $"[{DateTime.Now:HH:mm:ss}] {currentFile.Name} decrypted\n";
                                Directory.Delete(dividedPath_Temp, true);
                                LbProcess.Invoke(new Action(() => LbProcess.Text = "Done!"));
                            });
                        }
                        else
                        {
                            await Task.Run(() =>
                            {
                                data = File.ReadAllBytes(newDirPath + @"\" + currentFile.Name);
                                decryptedData = rsa.Decrypt(data, false);
                                Directory.CreateDirectory(decNewPath);
                                File.WriteAllBytes(decNewPath + @"\" + currentFile.Name, decryptedData);
                                RtbLogs.Text += $"[{DateTime.Now:HH:mm:ss}] {currentFile.Name} decrypted\n";
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        RtbLogs.Text += $"{currentFile.Name} is not decrypted: {ex.Message}\n";
                    }
                }
            }
            else
            {
                MessageBox.Show("Choose path to new directory\n");
                return;
            }
        }


        private void BtClearConsole_Click(object sender, EventArgs e)
        {
            RtbLogs.Clear();
            LbProcess.Text = string.Empty;
        }

        private void ChbCustomKeysName_CheckedChanged(object sender, EventArgs e)
        {
            if (ChbCustomKeysName.Checked)
            {
                TbCustomKeysName.Visible = true;
                LbEnterName.Visible = true;
                RtbLogs.Text += "Custom name for keys activated\n";
            }
            else
            {
                TbCustomKeysName.Visible = false;
                LbEnterName.Visible = false;
                keysName = Path.GetFileName(newDirPath);
                RtbLogs.Text += "Custom name for keys deactivated\n";
            }
        }

        private void TbCustomKeysName_TextChanged(object sender, EventArgs e)
        {
            if (ChbCustomKeysName.Checked)
            {
                keysName = TbCustomKeysName.Text;
            }

        }
    }
}

