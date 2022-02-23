using System;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Linq;
using System.Threading.Tasks;

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
        private string PrivatKey = string.Empty;
        private string DecNewPath = string.Empty;
        private byte[] EncryptedData;
        private byte[] DecryptedData;
        private byte[] data;
        private string publicxml = "";
        private string privatexml = "";
        private string DividedPath = string.Empty;
        private readonly int partSize = 112;
        private readonly int EncSize = 128;
        private string[] KeysArr = new string[2];
        private bool OperationCanceled = false;
        private string KeysName = string.Empty;

        #endregion

        #region Ключи
        private void CreateKey()
        {
            try
            {
                var RsaKey = new RSACryptoServiceProvider();
                string publickey = RsaKey.ToXmlString(false);
                string privatekey = RsaKey.ToXmlString(true);
                OperationCanceled = false;
                if (KeysPath != string.Empty)
                {
                    Directory.CreateDirectory(KeysPath);
                }
                if (KeysName != string.Empty) // создаёт ключи с названием папки
                {
                    if (!File.Exists(KeysPath + $@"\{KeysName}_public.xml") && !File.Exists(KeysPath + $@"\{KeysName}_private.xml"))
                    {
                        File.WriteAllText(KeysPath + $@"\{KeysName}_public.xml", publickey, Encoding.UTF8);
                        File.WriteAllText(KeysPath + $@"\{KeysName}_private.xml", privatekey, Encoding.UTF8);
                        PublicKey = $@"\{KeysName}_public.xml";
                        PrivatKey = $@"\{KeysName}_private.xml";
                    }
                    else
                    {
                        var Window = MessageBox.Show("Keys with this name are already exists. Do you want to rewrite them?\n","Same names",
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (Window == DialogResult.Yes)
                        {
                            File.WriteAllText(KeysPath + $@"\{KeysName}_public.xml", publickey, Encoding.UTF8);
                            File.WriteAllText(KeysPath + $@"\{KeysName}_private.xml", privatekey, Encoding.UTF8);
                            PublicKey = $@"\{KeysName}_public.xml";
                            PrivatKey = $@"\{KeysName}_private.xml";
                        }
                        else if(Window == DialogResult.No)
                        {
                            RtbLogs.Text += "Using previous keys\n";
                            PublicKey = $@"\{KeysName}_public.xml";
                            PrivatKey = $@"\{KeysName}_private.xml";
                        }
                        else if(Window == DialogResult.Cancel)
                        {
                            OperationCanceled = true;
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
                publicxml = File.ReadAllText(KeysPath + @"\" + PublicKey, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load public key\nChoose the right key\n" + ex.Message);
                return;
            }
            try
            {
                privatexml = File.ReadAllText(KeysPath + @"\" + PrivatKey, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load private key\nChoose the right key\n" + ex.Message);
                return;
            }            
        }

        #endregion

        #region Кнопки путей
        private void BtPathSourceDir_Click(object sender, EventArgs e) // путь к файлам для шифрования
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = @"C:\"
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SourcePath = dialog.FileName;
                TbSourceDir.Text = SourcePath;
                RtbLogs.Text += "Source directory selected successfully\n";
                ChbSourceDir.Checked = true;
            }
        }

        private void BtPathNewDir_Click(object sender, EventArgs e) // путь где зашифрованные файлы будут создаваться
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = @"C:\"
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                NewDirPath = dialog.FileName;
                KeysName = Path.GetFileName(NewDirPath);
                TbNewDir.Text = NewDirPath;
                RtbLogs.Text += "New directory selected successfully\n";
                ChbNewDir.Checked = true;
                KeysPath = dialog.FileName + @"\Keys";
                TbKeysDir.Text = KeysPath;
                RtbLogs.Text += $"Keys folder will in {TbKeysDir.Text}\n";
                DecNewPath = NewDirPath + @"\Decrypted"; // путь для расшифрованных файлов
            }
        }

        private void BtPathKeys_Click(object sender, EventArgs e) // изменить путь для ключей
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                KeysArr = dialog.FileNames;
                foreach (var key in KeysArr)
                {
                    if (key.Contains("_public.xml")) PublicKey = Path.GetFileName(key);
                    else if (key.Contains("_private.xml")) PrivatKey = Path.GetFileName(key);
                    KeysPath = Path.GetDirectoryName(key);
                }
                TbKeysDir.Text = KeysPath;
                RtbLogs.Text += $"Public key is {PublicKey}\nPrivate key is {PrivatKey}\n";
            }
        }



        #endregion

        private void Division(FileInfo file1, string DirPath, int _partSize)
        {
            byte[] file = File.ReadAllBytes(DirPath + @"\" + file1.Name); // для разделения файла на части
            int part = 1; // текущая часть файла
            int position = 0;//текущая позиция в куске
            DividedPath = NewDirPath + $@"\Divided"; // создание временной папки для разделённых файлов
            DirectoryInfo di = Directory.CreateDirectory(DividedPath);
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            int LastFileSize;
            for (int i = 0; i < file.Length; i += _partSize)
            {
                byte[] partbytes = new byte[Math.Min(_partSize, file.Length - i)];
                if (partbytes.Length < _partSize) LastFileSize = partbytes.Length;
                for (int j = 0; j < partbytes.Length; j++)
                {
                    partbytes[j] = file[position++];
                }
                File.WriteAllBytes(DividedPath + $@"\{file1.Name}_" + part + ".part", partbytes);
                part++;
            }
        }
        
        private void Addition(string _NewDirPath, string _DividedPath)
        {
            var filesInCurrentDirectory = new DirectoryInfo(_DividedPath);
            FileInfo[] filesInDirectory = filesInCurrentDirectory.GetFiles();
            string StartName = string.Empty;
            long SummaryLength = 0;
            int filesCount = filesInDirectory.Count();

            foreach (var file in filesInDirectory)// посчитать общий размер разделённого файла
            {
                SummaryLength += file.Length;
            }
            if (SummaryLength == 0)
            {
                RtbLogs.Text += "No files in directory";
                return;
            }

            foreach (var file in filesInDirectory)
            {
                if (file.Name.Contains("_1.part"))
                {
                    int test = file.Name.LastIndexOf("_1.part"); // поиск подстроки(_1.part)
                    StartName = file.Name.Remove(test);
                    break;
                }
            }
            try
            {
                byte[] partbytes = new byte[SummaryLength]; // размер массива в сумме всех файлов
                int counter = 0;
                for (int fileNum = 1; fileNum <= filesCount; fileNum++)
                {
                    byte[] file1 = File.ReadAllBytes(_DividedPath + @"\" + $"{StartName}_" + fileNum + ".part");
                    int size = file1.Length;
                    for (int i = 0; i < size; i++, counter++) // записываем все байты части в массив
                    {
                        partbytes[counter] = file1[i];
                    }
                }
                // полученный массив записываем в 1 файл
                File.WriteAllBytes(_NewDirPath + $@"\{StartName}", partbytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private async void BtEncrypt_Click(object sender, EventArgs e)
        {
            if (ChbSourceDir.Checked == true && ChbNewDir.Checked == true) // выбрана папка с изначальными файлами и папка куда записывать зашифрованные файлы
            {
                var rsa = new RSACryptoServiceProvider();
                CreateKey();
                if (OperationCanceled) return;
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

                var filesInCurrentDirectory = new DirectoryInfo(SourcePath);
                FileInfo[] filesInDirectory = filesInCurrentDirectory.GetFiles();

                foreach (var file in filesInDirectory) // шифрация каждого файла в папке
                {
                    data = new byte[partSize];
                    try
                    {
                        if (file.Length > 117)
                        {
                            await Task.Run(() => Division(file, SourcePath, partSize));
                            var filesInDividedDirectory = new DirectoryInfo(DividedPath);
                            FileInfo[] filesInDivDir = filesInDividedDirectory.GetFiles();
                            foreach (var DivFile in filesInDivDir) // шифрация всех файлов
                            {
                                await Task.Run(() =>
                                {
                                    data = File.ReadAllBytes(DividedPath + @"\" + DivFile.Name);
                                    EncryptedData = rsa.Encrypt(data, false);
                                    File.WriteAllBytes(DividedPath + @"\" + DivFile.Name, EncryptedData); // перезаписать этот файл
                                });
                            }
                            await Task.Run(() =>
                            {
                                Addition(NewDirPath, DividedPath);
                                RtbLogs.Text += $"{file.Name} encrypted\n";
                                Directory.Delete(DividedPath, true);
                            });
                        }
                        else
                        {
                            await Task.Run(() =>
                            {
                                data = File.ReadAllBytes(SourcePath + @"\" + file.Name);
                                EncryptedData = rsa.Encrypt(data, false);
                                File.WriteAllBytes(NewDirPath + @"\" + file.Name, EncryptedData);
                                RtbLogs.Text += $"{file.Name} encrypted\n";
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        RtbLogs.Text += $"{file.Name} is not encrypted: {ex.Message}\n";
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
                var filesInCurrentDirectory = new DirectoryInfo(NewDirPath);
                if (!Directory.Exists(NewDirPath))
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
                  
                foreach (var file in filesInDirectory) // расшифровка каждого файла в папке
                {
                    data = new byte[EncSize];
                    try
                    {
                        if (file.Length > 128)
                        {
                            await Task.Run(() => Division(file, NewDirPath, EncSize));
                            var filesInDividedDirectory = new DirectoryInfo(DividedPath);
                            FileInfo[] filesInDivDir = filesInDividedDirectory.GetFiles();
                            foreach (var DivFile in filesInDivDir)
                            {

                                await Task.Run(() =>
                                {
                                    data = File.ReadAllBytes(DividedPath + @"\" + DivFile.Name);
                                    DecryptedData = rsa.Decrypt(data, false);
                                    File.WriteAllBytes(DividedPath + @"\" + DivFile.Name, DecryptedData); // перезаписать этот файл
                                });
                            }
                            await Task.Run(() =>
                            {
                                Directory.CreateDirectory(DecNewPath); // создать директорию для расшифрованных файлов
                                Addition(DecNewPath, DividedPath);
                                RtbLogs.Text += $"{file.Name} decrypted\n";
                                Directory.Delete(DividedPath, true);
                            });
                        }
                        else
                        {
                            // расшифровка и вывод в консоль
                            await Task.Run(() =>
                            {
                                data = File.ReadAllBytes(NewDirPath + @"\" + file.Name);
                                DecryptedData = rsa.Decrypt(data, false);
                                Directory.CreateDirectory(DecNewPath); // создать директорию для расшифрованных файлов
                                File.WriteAllBytes(DecNewPath + @"\" + file.Name, DecryptedData);
                                RtbLogs.Text += $"{file.Name} decrypted\n";
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        RtbLogs.Text += $"{file.Name} is not decrypted: {ex.Message}\n";
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

