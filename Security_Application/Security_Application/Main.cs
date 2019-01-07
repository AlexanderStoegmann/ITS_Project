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
        CspParameters csp = new CspParameters();
        RSACryptoServiceProvider rsa;

        const string ContainerName = "Con1";
        const string PublicKeyPath = @"..\..\03_Keys\rsaPublicKey.txt";
        const string DocumentsPath = @"..\..\00_Keys\";
        const string EncryptPath = @"..\..\01_Encrypt\";
        const string DecryptPath = @"..\..\02_Decrypt\";
        public Main()
        {
            InitializeComponent();

        }

        //Generates and stores a key pair in the key container
        private void RSAGenerateKeys()
        {
            csp.KeyContainerName = ContainerName;
            var rsa = new RSACryptoServiceProvider(csp);
            rsa.PersistKeyInCsp = true;
        }
        
        private void RijndaelDecrypt(string fileName)
        {
            // Create instance of Rijndael for
            // symetric decryption of the data.
            RijndaelManaged rjndl = new RijndaelManaged();
            rjndl.KeySize = 256;
            rjndl.BlockSize = 256;
            rjndl.Mode = CipherMode.CBC;

            // Create byte arrays to get the length of
            // the encrypted key and IV.
            // These values were stored as 4 bytes each
            // at the beginning of the encrypted package.
            byte[] LenK = new byte[4];
            byte[] LenIV = new byte[4];

            // Consruct the file name for the decrypted file.
            string outFile = DecryptPath + fileName.Substring(0, fileName.LastIndexOf(".")) + ".txt";

            // Use FileStream objects to read the encrypted
            // file (inFs) and save the decrypted file (outFs).
            using (FileStream inFs = new FileStream(EncryptPath + fileName, FileMode.Open))
            {
                inFs.Seek(0, SeekOrigin.Begin);
                inFs.Seek(0, SeekOrigin.Begin);
                inFs.Read(LenK, 0, 3);
                inFs.Seek(4, SeekOrigin.Begin);
                inFs.Read(LenIV, 0, 3);

                // Convert the lengths to integer values.
                int lenK = BitConverter.ToInt32(LenK, 0);
                int lenIV = BitConverter.ToInt32(LenIV, 0);

                // Determine the start postition of
                // the ciphter text (startC)
                // and its length(lenC).
                int startC = lenK + lenIV + 8;
                int lenC = (int)inFs.Length - startC;

                // Create the byte arrays for
                // the encrypted Rijndael key,
                // the IV, and the cipher text.
                byte[] KeyEncrypted = new byte[lenK];
                byte[] IV = new byte[lenIV];

                // Extract the key and IV
                // starting from index 8
                // after the length values.
                inFs.Seek(8, SeekOrigin.Begin);
                inFs.Read(KeyEncrypted, 0, lenK);
                inFs.Seek(8 + lenK, SeekOrigin.Begin);
                inFs.Read(IV, 0, lenIV);
                Directory.CreateDirectory(DecryptPath);

                // Use RSACryptoServiceProvider
                // to decrypt the Rijndael key.
                byte[] KeyDecrypted = rsa.Decrypt(KeyEncrypted, false);

                // Decrypt the key.
                ICryptoTransform transform = rjndl.CreateDecryptor(KeyDecrypted, IV);

                // Decrypt the cipher text from
                // from the FileSteam of the encrypted
                // file (inFs) into the FileStream
                // for the decrypted file (outFs).
                using (FileStream outFs = new FileStream(outFile, FileMode.Create))
                {

                    int count = 0;
                    int offset = 0;

                    // blockSizeBytes can be any arbitrary size.
                    int blockSizeBytes = rjndl.BlockSize / 8;
                    byte[] data = new byte[blockSizeBytes];


                    // By decrypting a chunk a time,
                    // you can save memory and
                    // accommodate large files.

                    // Start at the beginning
                    // of the cipher text.
                    inFs.Seek(startC, SeekOrigin.Begin);
                    using (CryptoStream outStreamDecrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
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
        private void RijndaelEncrypt(string fileName)
        {
            RijndaelManaged RMCrypto = new RijndaelManaged();
            RMCrypto.KeySize = 256;
            RMCrypto.BlockSize = 256;
            RMCrypto.Mode = CipherMode.CBC;
            ICryptoTransform transform = RMCrypto.CreateEncryptor();

            // Use RSACryptoServiceProvider to
            // enrypt the Rijndael key.
            // rsa is previously instantiated: 
            //    rsa = new RSACryptoServiceProvider(cspp);
            byte[] keyEncrypted = rsa.Encrypt(RMCrypto.Key, false);

            // Create byte arrays to contain
            // the length values of the key and IV.
            byte[] LenK = new byte[4];
            byte[] LenIV = new byte[4];

            int lKey = keyEncrypted.Length;
            LenK = BitConverter.GetBytes(lKey);
            int lIV = RMCrypto.IV.Length;
            LenIV = BitConverter.GetBytes(lIV);

            // Write the following to the FileStream
            // for the encrypted file (outFs):
            // - length of the key
            // - length of the IV
            // - ecrypted key
            // - the IV
            // - the encrypted cipher content

            int startFileName = fileName.LastIndexOf("\\") + 1;
            // Change the file's extension to ".enc"
            string outFile = EncryptPath + fileName.Substring(startFileName, fileName.LastIndexOf(".") - startFileName) + ".enc";

            using (FileStream outFs = new FileStream(outFile, FileMode.Create))
            {

                outFs.Write(LenK, 0, 4);
                outFs.Write(LenIV, 0, 4);
                outFs.Write(keyEncrypted, 0, lKey);
                outFs.Write(RMCrypto.IV, 0, lIV);

                // Now write the cipher text using
                // a CryptoStream for encrypting.
                using (CryptoStream outStreamEncrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                {

                    // By encrypting a chunk at
                    // a time, you can save memory
                    // and accommodate large files.
                    int count = 0;
                    int offset = 0;

                    // blockSizeBytes can be any arbitrary size.
                    int blockSizeBytes = RMCrypto.BlockSize / 8;
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


        private void ExportPublicKey()
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

        private void ImportPublicKey()
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
                MessageBox.Show("Key not set.");
            else
            {
                openFileDialog1.InitialDirectory = DocumentsPath;
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
                MessageBox.Show("Key not set.");
            else
            {
                // Display a dialog box to select the encrypted file.
                openFileDialog2.InitialDirectory = EncryptPath;
                if (openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    string fName = openFileDialog2.FileName;
                    if (fName != null)
                    {
                        FileInfo fi = new FileInfo(fName);
                        string FileName = fi.Name;
                        RijndaelDecrypt(FileName);
                    }
                }
            }
        }
    }
}
