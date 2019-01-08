using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace Security_Application
{
    public partial class Main : Form
    {
        private static CspParameters csp = new CspParameters();
        private static RSACryptoServiceProvider rsa;

        const string ContainerName = "Con1";
        const string PublicKeyPath = @"..\..\03_Keys\rsaPublicKey.txt";
        const string SymKeyPath = @"..\..\03_Keys\symKey.txt";
        const string DocumentsPath = @"..\..\00_Documents\";
        const string EncryptPath = @"..\..\01_Encrypt\";
        const string DecryptPath = @"..\..\02_Decrypt\";
        const string KeyPath = @"..\..\03_Keys\";
        public Main()
        {
            InitializeComponent();
        }

        private void btnCreateAsymmetric_Click(object sender, EventArgs e)
        {
            RSAGenerateKeys();
        }

        private void btnExportPublicRSAKey_Click(object sender, EventArgs e)
        {
            ExportPublicKey();
        }

        private void btnEncryptOneFile_Click(object sender, EventArgs e)
        {
            // check if RSA-Keys have already been created
            if (rsa == null)
                MessageBox.Show("RSA Key pairs are not set.");
            else
            {
                openFileDialog1.InitialDirectory = DecryptPath;
                openFileDialog1.RestoreDirectory = true;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string fName = openFileDialog1.FileName;
                    if (fName != null)
                    {
                        FileInfo fInfo = new FileInfo(fName);
                        // Pass the file name without the path.
                        string fileName = fInfo.FullName;
                        RijndaelEncrypt(fileName);
                    }
                }
            }
        }

        private void btnDecryptOneFile_Click(object sender, EventArgs e)
        {
            if (rsa == null)
                MessageBox.Show("RSA Key pairs are not set.");
            else
            {
                // Display a dialog box to select the encrypted file.
                openFileDialog2.InitialDirectory = DocumentsPath;
                openFileDialog2.RestoreDirectory = true;
                if (openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    string fName = openFileDialog2.FileName;
                    if (fName != null)
                    {
                        FileInfo fi = new FileInfo(fName);
                        string fileName = fi.Name;
                        RijndaelDecrypt(fileName);
                    }
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clearFolder(EncryptPath);
            clearFolder(DecryptPath);
            clearFolder(KeyPath);
            MessageBox.Show("In den Ordnern 01-03 wurden sämtliche vorhandenen Dateien gelöscht");
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private static void RSAGenerateKeys()
        {
            //Generates and stores a key pair in the key container
            csp.KeyContainerName = ContainerName;
            using (rsa = new RSACryptoServiceProvider(csp)) { rsa.PersistKeyInCsp = true; } 
        }

        private static void RijndaelEncrypt(string fileName)
        {
            // create an RijndaelManaged object
            RijndaelManaged aesAlg = NewRijndaelManaged();

            // create an encryptor to perform the stream transform
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key,aesAlg.IV);

            // use RSACryptoServiceProvider to enrypt the Rijndael key
            byte[] keyEncrypted = rsa.Encrypt(aesAlg.Key, false);

            // create byte arrays which contain the length values of the key and IV
            byte[] lengthKey = new byte[4];
            byte[] lengthIV = new byte[4];

            // convert given key length and IV length from int to byte[]
            lengthKey = BitConverter.GetBytes(keyEncrypted.Length);
            lengthIV = BitConverter.GetBytes(aesAlg.IV.Length);

            // write the following to the FileStream for the encrypted key file (keyFs):
            // length of the key + length of the IV + encrypted key + the IV

            using (FileStream keyFs = new FileStream(SymKeyPath, FileMode.Create))
            {
                keyFs.Write(lengthKey, 0, 4);
                keyFs.Write(lengthIV, 0, 4);
                keyFs.Write(keyEncrypted, 0, keyEncrypted.Length);
                keyFs.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                keyFs.Close();
            }

            // write the following to the FileStream for the encrypted cipher content file (outFs)
            int startFileName = fileName.LastIndexOf("\\") + 1;
            // change the file's extension to ".enc"
            string outFile = EncryptPath + fileName.Substring(startFileName, fileName.LastIndexOf(".") - startFileName) + ".enc";
            using (FileStream outFs = new FileStream(outFile, FileMode.Create))
            {
                // now write the cipher text using a CryptoStream for encrypting.
                using (CryptoStream outStreamEncrypted = new CryptoStream(outFs, encryptor, CryptoStreamMode.Write))
                {
                    int count = 0;
                    int offset = 0;
                    // blockSizeBytes can be any arbitrary size
                    int blockSizeBytes = aesAlg.BlockSize / 8;
                    byte[] data = new byte[blockSizeBytes];
                    int bytesRead = 0;

                    using (FileStream inFs = new FileStream(fileName, FileMode.Open))
                    {
                        do
                        {
                            count = inFs.Read(data, 0, blockSizeBytes);
                            offset += count;
                            outStreamEncrypted.Write(data, 0, count);
                            bytesRead += blockSizeBytes;
                        }
                        while (count > 0);
                        inFs.Close();
                    }
                    outStreamEncrypted.FlushFinalBlock();
                    outStreamEncrypted.Close();
                }
                outFs.Close();
            }
        }

        private static void RijndaelDecrypt(string fileName)
        {
            // create an RijndaelManaged object
            RijndaelManaged aesAlg = NewRijndaelManaged();

            // create byte arrays to get the length of the encrypted key and IV.
            // these values were stored as 4 bytes each at the beginning of the encrypted package.
            byte[] lengthKey = new byte[4];
            byte[] lengthIV = new byte[4];

            // consruct the file name for the decrypted file
            string outFile = DecryptPath + fileName.Substring(0, fileName.LastIndexOf(".")) + ".txt";

            // create an instance of the decrypter object
            ICryptoTransform decryptor = null;

            //use FileStream object to read the encrypted symmetric key + IV (keyFs)
            using (FileStream keyFs = new FileStream(SymKeyPath, FileMode.Open))
            {
                keyFs.Seek(0, SeekOrigin.Begin);
                keyFs.Read(lengthKey, 0, 3);
                keyFs.Seek(4, SeekOrigin.Begin);
                keyFs.Read(lengthIV, 0, 3);

                // convert the lengths to integer values
                int lenK = BitConverter.ToInt32(lengthKey, 0);
                int lenIV = BitConverter.ToInt32(lengthIV, 0);

                // create the byte arrays for the encrypted Rijndael key and the IV, and the cipher text
                byte[] KeyEncrypted = new byte[lenK];
                byte[] IV = new byte[lenIV];

                // extract the key and IV starting from index 8 after the length values
                keyFs.Seek(8, SeekOrigin.Begin);
                keyFs.Read(KeyEncrypted, 0, lenK);
                keyFs.Seek(8 + lenK, SeekOrigin.Begin);
                keyFs.Read(IV, 0, lenIV);
                Directory.CreateDirectory(DecryptPath);

                // use RSACryptoServiceProvider to decrypt the Rijndael key
                byte[] KeyDecrypted = rsa.Decrypt(KeyEncrypted, false);

                // create the instanced descripter object with the valid symmetric key + IV
                decryptor = aesAlg.CreateDecryptor(KeyDecrypted, IV);

                keyFs.Close();
            }

            // use FileStream objects to read the encrypted file (inFs) and save the decrypted file (outFs)
            using (FileStream inFs = new FileStream(EncryptPath + fileName, FileMode.Open))
            {

                // decrypt the cipher text from the FileSteam
                using (FileStream outFs = new FileStream(outFile, FileMode.Create))
                {
                    int count = 0;
                    int offset = 0;
                    // blockSizeBytes can be any arbitrary size.
                    int blockSizeBytes = aesAlg.BlockSize / 8;
                    byte[] data = new byte[blockSizeBytes];

                    inFs.Seek(0, SeekOrigin.Begin);
                    using (CryptoStream outStreamDecrypted = new CryptoStream(outFs, decryptor, CryptoStreamMode.Write))
                    {
                        do
                        {
                            count = inFs.Read(data, 0, blockSizeBytes);
                            offset += count;
                            outStreamDecrypted.Write(data, 0, count);

                        }
                        while (count > 0);
                        outStreamDecrypted.FlushFinalBlock();
                        outStreamDecrypted.Close();
                    }
                    outFs.Close();
                }
                inFs.Close();
            }
        }

        private static void ExportPublicKey()
        {
            //stream to save the public key
            StreamWriter sw = null;

            // create folder for public key if it doesn't exist
            if (Directory.Exists(PublicKeyPath)) { Directory.CreateDirectory(PublicKeyPath); }

            //save public key
            sw = new StreamWriter(PublicKeyPath,false);
            sw.Write(rsa.ToXmlString(false));
            sw.Flush();
            sw.Close();
        }

        private static void ImportPublicKey()
        {
            //stream to read the public key
            StreamReader sr = null;
            string key;

            //read in public key and save to container
            csp.KeyContainerName = ContainerName;
            using (rsa = new RSACryptoServiceProvider(csp))
            {
                sr = new StreamReader(PublicKeyPath);
                key = sr.ReadToEnd();
                rsa.FromXmlString(key);
                rsa.PersistKeyInCsp = true;
                sr.Close();
            }
        }

        private static RijndaelManaged NewRijndaelManaged()
        {
            RijndaelManaged aesAlg = new RijndaelManaged();
            aesAlg.KeySize = 256;
            aesAlg.BlockSize = 256;
            aesAlg.Mode = CipherMode.CBC;
            return aesAlg;
        }

        private void clearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
        }

    }
}
