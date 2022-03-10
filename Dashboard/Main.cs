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

        #region Переменные

        private string SourcePath = string.Empty;
        private string NewDirPath = string.Empty;
        private string KeysPath = string.Empty;
        private string PublicKey = string.Empty;
        private string PrivateKey = string.Empty;
        private string DecNewPath = string.Empty;
        private byte[] EncryptedData;
        private byte[] DecryptedData;
        private byte[] Data;
        private string Publicxml = "";
        private string Privatexml = "";
        private string DividedPath_Temp = string.Empty;
        private readonly int PartSize = 112;
        private readonly int EncSize = 128;
        private string[] KeysArr = new string[2];
        private bool OperationCanceled = false;
        private string KeysName = string.Empty;
        private int TotalDivisionCompleted;
        private int TotalMergingCompleted;
        private int TotalEncryptionCompleted;
        private int TotalDecryptionCompleted;
        private readonly int TotalAmountOfProcesses = 3; // 1) разделение файла на части; 2) шифрация / расшифровка; 3) сложение файлов в единое целое

        #endregion

        #region Ключи
        private void CreateKey()
        {
            try
            {
                var RSAKey = new RSACryptoServiceProvider();
                string RSAPublicKey = RSAKey.ToXmlString(false);
                string RSAPrivateKey = RSAKey.ToXmlString(true);
                OperationCanceled = false;

                if (KeysPath != string.Empty)
                {
                    Directory.CreateDirectory(KeysPath);
                }
                if (KeysName != string.Empty)
                {
                    void GetKeysNameFromFolder()
                    {
                        PublicKey = $@"\{KeysName}_public.xml";
                        PrivateKey = $@"\{KeysName}_private.xml";
                    }

                    void CreateKeysInFolder()
                    {
                        File.WriteAllText(KeysPath + $@"\{KeysName}_public.xml", RSAPublicKey, Encoding.UTF8);
                        File.WriteAllText(KeysPath + $@"\{KeysName}_private.xml", RSAPrivateKey, Encoding.UTF8);
                    }

                    if (!File.Exists(KeysPath + $@"\{KeysName}_public.xml") && !File.Exists(KeysPath + $@"\{KeysName}_private.xml"))
                    {
                        CreateKeysInFolder();
                        GetKeysNameFromFolder();
                    }
                    else
                    {
                        var Window = MessageBox.Show("Keys with this name are already exists. Do you want to rewrite them?\n", "Same names",
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (Window == DialogResult.Yes)
                        {
                            CreateKeysInFolder();
                            GetKeysNameFromFolder();
                        }
                        else if (Window == DialogResult.No)
                        {
                            RtbLogs.Text += "Using previous keys\n";
                            GetKeysNameFromFolder();
                        }
                        else if (Window == DialogResult.Cancel)
                        {
                            OperationCanceled = true;
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
                Publicxml = File.ReadAllText(KeysPath + @"\" + PublicKey, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load public key\nChoose the right key\n" + ex.Message);
                return;
            }
            try
            {
                Privatexml = File.ReadAllText(KeysPath + @"\" + PrivateKey, Encoding.UTF8);
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
            var Dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = @"C:\"
            };
            if (Dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SourcePath = Dialog.FileName;
                TbSourceDir.Text = SourcePath;
                ChbSourceDir.Checked = true;
            }
        }

        private void BtPathNewDir_Click(object sender, EventArgs e)
        {
            var Dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = @"C:\"
            };
            if (Dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                NewDirPath = Dialog.FileName;
                KeysName = Path.GetFileName(NewDirPath);
                TbNewDir.Text = NewDirPath;
                ChbNewDir.Checked = true;
                KeysPath = Dialog.FileName + @"\Keys";
                TbKeysDir.Text = KeysPath;
                DecNewPath = NewDirPath + @"\Decrypted";
            }
        }

        private void BtPathKeys_Click(object sender, EventArgs e)
        {
            var Dialog = new OpenFileDialog
            {
                Multiselect = true
            };
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                KeysArr = Dialog.FileNames;
                foreach (var Key in KeysArr) // List?
                {
                    if (Key.Contains("_public.xml")) PublicKey = Path.GetFileName(Key);
                    else if (Key.Contains("_private.xml")) PrivateKey = Path.GetFileName(Key);
                    KeysPath = Path.GetDirectoryName(Key);
                }
                TbKeysDir.Text = KeysPath;
                RtbLogs.Text += $"Public key is {PublicKey}\nPrivate key is {PrivateKey}\n";
            }
        }

        #endregion

        private void DivideFile(FileInfo DividingFile, string DirPath, int _PartSize)
        {
            byte[] FullFile = File.ReadAllBytes(DirPath + @"\" + DividingFile.Name);
            int CurrentPart = 1;
            int CurrentPosition = 0;
            int LastFileSize = FullFile.Length % _PartSize;
            int TotalNumberOfFiles = FullFile.Length / _PartSize + 1;

            DividedPath_Temp = NewDirPath + $@"\Divided";
            DirectoryInfo DirInfo = Directory.CreateDirectory(DividedPath_Temp);
            DirInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            for (int CurrentSize = 0; CurrentSize < FullFile.Length; CurrentSize += _PartSize)
            {
                byte[] PartBytes = new byte[Math.Min(_PartSize, FullFile.Length - CurrentSize)];

                for (int Position = 0; Position < PartBytes.Length; Position++) // записываем часть целого файла в новый файл
                {
                    PartBytes[Position] = FullFile[CurrentPosition++];
                }
                File.WriteAllBytes(DividedPath_Temp + $@"\{DividingFile.Name}_" + CurrentPart + ".part", PartBytes);

                TotalDivisionCompleted = CurrentPart * 100 / (TotalNumberOfFiles * TotalAmountOfProcesses);
                RtbProcess.Text = $"Dividing file. {TotalDivisionCompleted}% has done"; // ошибка при трассировки
                CurrentPart++;
            }
        }

        private void MergeFile(string _NewDirPath, string _DividedPath, int TotalDecryptionOrEncryptionCompleted)
        {
            var FilesInCurrentDirectory = new DirectoryInfo(_DividedPath);
            FileInfo[] FilesInDirectory = FilesInCurrentDirectory.GetFiles();
            string StartName = string.Empty;
            long SummaryLength = 0;
            int FilesCount = FilesInDirectory.Count();
            foreach (var PartFile in FilesInDirectory)
            {
                SummaryLength += PartFile.Length;
            }
            if(SummaryLength == 0)
            {
                RtbLogs.Text += "No files in directory";
                return;
            }

            foreach (var CurrentFile in FilesInDirectory)
            {
                if (CurrentFile.Name.Contains("_1.part"))
                {
                    StartName = CurrentFile.Name.Remove(CurrentFile.Name.LastIndexOf("_1.part"));
                    break;
                }
            }
            try
            {
                byte[] PartBytes = new byte[SummaryLength];
                int Position = 0;
                for (int FileNumber = 1; FileNumber <= FilesCount; FileNumber++)
                {
                    byte[] PartFile = File.ReadAllBytes(_DividedPath + @"\" + $"{StartName}_" + FileNumber + ".part");
                    int FileSize = PartFile.Length;

                    for (int PartPosition = 0; PartPosition < FileSize; PartPosition++, Position++) // записываем все байты части в массив
                    {
                        PartBytes[Position] = PartFile[PartPosition];
                    }
                    TotalMergingCompleted = FileNumber * 100 / (FilesCount * TotalAmountOfProcesses);
                    RtbProcess.Text = $"Merging file. {TotalDivisionCompleted + TotalDecryptionOrEncryptionCompleted + TotalMergingCompleted}% has done";
                }
                File.WriteAllBytes(_NewDirPath + $@"\{StartName}", PartBytes);
                RtbProcess.Text = "Merging file. 100% has done";
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
                var RSA = new RSACryptoServiceProvider();
                CreateKey();
                if (OperationCanceled) return;
                LoadKey();
                if (Publicxml.Length == 0)
                {
                    MessageBox.Show("Invalid public key");
                    return;
                }
                try
                {
                    RSA.FromXmlString(Publicxml);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Problems with RSA\n" + ex.Message);
                }

                var FilesInCurrentDirectory = new DirectoryInfo(SourcePath);
                FileInfo[] FilesInDirectory = FilesInCurrentDirectory.GetFiles();
                
                foreach (var CurrentFile in FilesInDirectory)
                {
                    int CurrentFilePosition = 1;
                    Data = new byte[PartSize];
                    try
                    {
                        if (CurrentFile.Length > 117)
                        {
                            RtbLogs.Text += $"[{DateTime.Now:HH:mm:ss}] Dividing {CurrentFile}\n";
                            await Task.Run(() => DivideFile(CurrentFile, SourcePath, PartSize));
                            var FilesInDividedDirectory = new DirectoryInfo(DividedPath_Temp);
                            FileInfo[] FilesInDivDir = FilesInDividedDirectory.GetFiles();
                            int TotalNumberOfFiles = FilesInDivDir.Count();

                            foreach (var DivFile in FilesInDivDir)
                            {
                                await Task.Run(() =>
                                {
                                    Data = File.ReadAllBytes(DividedPath_Temp + @"\" + DivFile.Name);
                                    EncryptedData = RSA.Encrypt(Data, false);
                                    File.WriteAllBytes(DividedPath_Temp + @"\" + DivFile.Name, EncryptedData);

                                    TotalEncryptionCompleted = CurrentFilePosition * 100 / (TotalNumberOfFiles * TotalAmountOfProcesses);
                                    RtbProcess.Text = $"Encrypting. {TotalDivisionCompleted + TotalEncryptionCompleted}% has done";
                                    CurrentFilePosition++;
                                });
                            }
                            await Task.Run(() =>
                            {
                                MergeFile(NewDirPath, DividedPath_Temp, TotalEncryptionCompleted);
                                RtbLogs.Text += $"[{DateTime.Now:HH:mm:ss}] {CurrentFile.Name} encrypted\n";
                                Directory.Delete(DividedPath_Temp, true);
                                RtbProcess.Text = "Done!";
                            });
                        }
                        else
                        {
                            await Task.Run(() =>
                            {
                                Data = File.ReadAllBytes(SourcePath + @"\" + CurrentFile.Name);
                                EncryptedData = RSA.Encrypt(Data, false);
                                File.WriteAllBytes(NewDirPath + @"\" + CurrentFile.Name, EncryptedData);
                                RtbLogs.Text += $"[{DateTime.Now:HH:mm:ss}] {CurrentFile.Name} encrypted\n";
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        RtbLogs.Text += $"{CurrentFile.Name} is not encrypted: {ex.Message}\n";
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
                if (Privatexml.Length == 0)
                {
                    MessageBox.Show("Invalid private key\n");
                    return;
                }
                var RSA = new RSACryptoServiceProvider();
                var FilesInCurrentDirectory = new DirectoryInfo(NewDirPath);
                if (!Directory.Exists(NewDirPath))
                {
                    MessageBox.Show("New directory is not exist");
                    return;
                }
                FileInfo[] FilesInDirectory = FilesInCurrentDirectory.GetFiles();

                try
                {
                    RSA.FromXmlString(Privatexml);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Problems with RSA\n" + ex.Message);
                }
                int CurrentFilePosition = 1;
                foreach (var CurrentFile in FilesInDirectory)
                {
                    Data = new byte[EncSize];
                    try
                    {
                        if (CurrentFile.Length > 128)
                        {
                            RtbLogs.Text += $"[{DateTime.Now:HH:mm:ss}] Dividing {CurrentFile}\n";
                            await Task.Run(() => DivideFile(CurrentFile, NewDirPath, EncSize));
                            var FilesInDividedDirectory = new DirectoryInfo(DividedPath_Temp);
                            FileInfo[] FilesInDivDir = FilesInDividedDirectory.GetFiles();
                            int TotalNumberOfFiles = FilesInDivDir.Count();

                            foreach (var DivFile in FilesInDivDir)
                            {
                                await Task.Run(() =>
                                {
                                    Data = File.ReadAllBytes(DividedPath_Temp + @"\" + DivFile.Name);
                                    DecryptedData = RSA.Decrypt(Data, false);
                                    File.WriteAllBytes(DividedPath_Temp + @"\" + DivFile.Name, DecryptedData);
                                    TotalDecryptionCompleted = CurrentFilePosition * 100 / (TotalNumberOfFiles * TotalAmountOfProcesses);
                                    RtbProcess.Text = $"Decrypting. {TotalDivisionCompleted + TotalDecryptionCompleted}% has done";
                                    CurrentFilePosition++;
                                });
                            }
                            await Task.Run(() =>
                            {
                                Directory.CreateDirectory(DecNewPath);
                                MergeFile(DecNewPath, DividedPath_Temp, TotalDecryptionCompleted);
                                RtbLogs.Text += $"[{DateTime.Now:HH:mm:ss}] {CurrentFile.Name} decrypted\n";
                                Directory.Delete(DividedPath_Temp, true);
                                RtbProcess.Text = "Done!";
                            });
                        }
                        else
                        {
                            await Task.Run(() =>
                            {
                                Data = File.ReadAllBytes(NewDirPath + @"\" + CurrentFile.Name);
                                DecryptedData = RSA.Decrypt(Data, false);
                                Directory.CreateDirectory(DecNewPath);
                                File.WriteAllBytes(DecNewPath + @"\" + CurrentFile.Name, DecryptedData);
                                RtbLogs.Text += $"[{DateTime.Now:HH:mm:ss}] {CurrentFile.Name} decrypted\n";
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        RtbLogs.Text += $"{CurrentFile.Name} is not decrypted: {ex.Message}\n";
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
            RtbProcess.Clear();
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
                KeysName = Path.GetFileName(NewDirPath);
                RtbLogs.Text += "Custom name for keys deactivated\n";
            }
        }

        private void TbCustomKeysName_TextChanged(object sender, EventArgs e)
        {
            if (ChbCustomKeysName.Checked)
            {
                KeysName = TbCustomKeysName.Text;
            }

        }
    }
}

