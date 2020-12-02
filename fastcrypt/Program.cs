using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

// Changed to 3DES to avoid FIPS140-2 crap on workstations that are FIPS enabled.

namespace fastcrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("{0} [-e|-d] key infile outfile", Assembly.GetExecutingAssembly().GetName());
                return;
            }

            if (args[1].Length != 16)
            {
                Console.WriteLine("Key must be exactly 16 bytes.");
                return;
            }

            if (args[0] == "-e")
            {
                EncryptFile(args[2], args[3], args[1]);
            }
            else if (args[0] == "-d")
            {
                DecryptFile(args[2], args[3], args[1]);
            }
            else
            {
                Console.WriteLine("Invalid operation specified.");
            }
        }
        private static void DecryptFile(string inputFile, string outputFile, string skey)
        {
            try
            {
                using (TripleDESCryptoServiceProvider aes = new TripleDESCryptoServiceProvider())
                {
                    byte[] key = ASCIIEncoding.UTF8.GetBytes(skey);
                    byte[] IV = ASCIIEncoding.UTF8.GetBytes(skey);

                    using (FileStream fsCrypt = new FileStream(inputFile, FileMode.Open))
                    {
                        using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                        {
                            using (ICryptoTransform decryptor = aes.CreateDecryptor(key, IV))
                            {
                                using (CryptoStream cs = new CryptoStream(fsCrypt, decryptor, CryptoStreamMode.Read))
                                {
                                    int data;
                                    while ((data = cs.ReadByte()) != -1)
                                    {
                                        fsOut.WriteByte((byte)data);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Decyption Failed.");
                Console.WriteLine(ex.Message);
            }
        }

        private static void EncryptFile(string inputFile, string outputFile, string skey)
        {
            try
            {
                using (TripleDESCryptoServiceProvider aes = new TripleDESCryptoServiceProvider())
                {
                    byte[] key = ASCIIEncoding.UTF8.GetBytes(skey);
                    byte[] IV = ASCIIEncoding.UTF8.GetBytes(skey);

                    using (FileStream fsCrypt = new FileStream(outputFile, FileMode.Create))
                    {
                        using (ICryptoTransform encryptor = aes.CreateEncryptor(key, IV))
                        {
                            using (CryptoStream cs = new CryptoStream(fsCrypt, encryptor, CryptoStreamMode.Write))
                            {
                                using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                                {
                                    int data;
                                    while ((data = fsIn.ReadByte()) != -1)
                                    {
                                        cs.WriteByte((byte)data);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to Encrypt file.");
                Console.WriteLine(ex.Message);
            }
        }


    }
}
