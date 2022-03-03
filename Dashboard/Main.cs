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
        private string PrivateKey = string.Empty;
        private string DecNewPath = string.Empty;
        private byte[] EncryptedData;
        private byte[] DecryptedData;
        private byte[] Data;
        private string Publicxml = "";
        private string Privatexml = "";
        private string DividedPath_Temp = string.Empty;
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
                        var Window = MessageBox.Show("Keys with this name are already exists. Do you want to rewrite them?\n","Same names",
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (Window == DialogResult.Yes)
                        {
                            CreateKeysInFolder();
                            GetKeysNameFromFolder();
                        }
                        else if(Window == DialogResult.No)
                        {
                            RtbLogs.Text += "Using previous keys\n";
                            GetKeysNameFromFolder();
                        }
                        else if(Window == DialogResult.Cancel)
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

        private void Division(FileInfo DividingFile, string DirPath, int _PartSize)
        {
            byte[] FullFile = File.ReadAllBytes(DirPath + @"\" + DividingFile.Name);
            int CurrentPart = 1;
            int CurrentPosition = 0;
            int LastFileSize;
            DividedPath_Temp = NewDirPath + $@"\Divided";
            DirectoryInfo DirInfo = Directory.CreateDirectory(DividedPath_Temp);
            DirInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;         

            for (int CurrentSize = 0; CurrentSize < FullFile.Length; CurrentSize += _PartSize)
            {
                byte[] PartBytes = new byte[Math.Min(_PartSize, FullFile.Length - CurrentSize)];
                if (PartBytes.Length < _PartSize) LastFileSize = PartBytes.Length;

                for (int Position = 0; Position < PartBytes.Length; Position++) // записываем часть целого файла в новый файл
                {
                    PartBytes[Position] = FullFile[CurrentPosition++];
                }
                File.WriteAllBytes(DividedPath_Temp + $@"\{DividingFile.Name}_" + CurrentPart + ".part", PartBytes);
                CurrentPart++;
            }
        }
        
        private void Addition(string _NewDirPath, string _DividedPath)
        {
            var FilesInCurrentDirectory = new DirectoryInfo(_DividedPath);
            FileInfo[] FilesInDirectory = FilesInCurrentDirectory.GetFiles();
            string StartName = string.Empty;
            long SummaryLength = 0;
            int FilesCount = FilesInDirectory.Count();

            foreach (var file in FilesInDirectory)
            {
                SummaryLength += file.Length;
            }
            if (SummaryLength == 0)
            {
                RtbLogs.Text += "No files in directory";
                return;
            }

            foreach (var file in FilesInDirectory)
            {
                if (file.Name.Contains("_1.part"))
                {
                    StartName = file.Name.Remove(file.Name.LastIndexOf("_1.part"));
                    break;
                }
            }
            try
            {
                byte[] PartBytes = new byte[SummaryLength];
                int Position = 0;
                for (int FileNum = 1; FileNum <= FilesCount; FileNum++)
                {
                    byte[] file = File.ReadAllBytes(_DividedPath + @"\" + $"{StartName}_" + FileNum + ".part");
                    int FileSize = file.Length;

                    for (int PartPosition = 0; PartPosition < FileSize; PartPosition++, Position++) // записываем все байты части в массив
                    {
                        PartBytes[Position] = file[PartPosition];
                    }
                }
                File.WriteAllBytes(_NewDirPath + $@"\{StartName}", PartBytes);
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
                    Data = new byte[partSize];
                    try
                    {
                        if (CurrentFile.Length > 117)
                        {
                            await Task.Run(() => Division(CurrentFile, SourcePath, partSize));
                            var FilesInDividedDirectory = new DirectoryInfo(DividedPath_Temp);
                            FileInfo[] FilesInDivDir = FilesInDividedDirectory.GetFiles();
                            foreach (var DivFile in FilesInDivDir)
                            {
                                await Task.Run(() =>
                                {
                                    Data = File.ReadAllBytes(DividedPath_Temp + @"\" + DivFile.Name);
                                    EncryptedData = RSA.Encrypt(Data, false);
                                    File.WriteAllBytes(DividedPath_Temp + @"\" + DivFile.Name, EncryptedData);
                                });
                            }
                            await Task.Run(() =>
                            {
                                Addition(NewDirPath, DividedPath_Temp);
                                RtbLogs.Text += $"{CurrentFile.Name} encrypted\n";
                                Directory.Delete(DividedPath_Temp, true);
                            });
                        }
                        else
                        {
                            await Task.Run(() =>
                            {
                                Data = File.ReadAllBytes(SourcePath + @"\" + CurrentFile.Name);
                                EncryptedData = RSA.Encrypt(Data, false);
                                File.WriteAllBytes(NewDirPath + @"\" + CurrentFile.Name, EncryptedData);
                                RtbLogs.Text += $"{CurrentFile.Name} encrypted\n";
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
                  
                foreach (var CurrentFile in FilesInDirectory)
                {
                    Data = new byte[EncSize];
                    try
                    {
                        if (CurrentFile.Length > 128)
                        {
                            await Task.Run(() => Division(CurrentFile, NewDirPath, EncSize));
                            var FilesInDividedDirectory = new DirectoryInfo(DividedPath_Temp);
                            FileInfo[] FilesInDivDir = FilesInDividedDirectory.GetFiles();
                            foreach (var DivFile in FilesInDivDir)
                            {
                                await Task.Run(() =>
                                {
                                    Data = File.ReadAllBytes(DividedPath_Temp + @"\" + DivFile.Name);
                                    DecryptedData = RSA.Decrypt(Data, false);
                                    File.WriteAllBytes(DividedPath_Temp + @"\" + DivFile.Name, DecryptedData);
                                });
                            }
                            await Task.Run(() =>
                            {
                                Directory.CreateDirectory(DecNewPath);
                                Addition(DecNewPath, DividedPath_Temp);
                                RtbLogs.Text += $"{CurrentFile.Name} decrypted\n";
                                Directory.Delete(DividedPath_Temp, true);
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
                                RtbLogs.Text += $"{CurrentFile.Name} decrypted\n";
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

