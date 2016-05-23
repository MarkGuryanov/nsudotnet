using System;
using System.IO;
using System.Security.Cryptography;

namespace Guryanov.Nsudotnet.Enigma
{
    class Enigma
    {
        private readonly bool _toEncrypt;
        private readonly string _plainFileName, _cryptoFileName, _keyFileName;
        private readonly SymmetricAlgorithm _algorithm;

        public Enigma(string[] args)
        {
            if (args.Length == 4 && args[0] == "encrypt")
            {
                _toEncrypt = true;
                _plainFileName = args[1];
                _cryptoFileName = args[3];
            }
            else if (args.Length == 5 && args[0] == "decrypt")
            {
                _toEncrypt = false;
                _cryptoFileName = args[1];
                _keyFileName = args[3];
                _plainFileName = args[4];
            }
            else throw new ArgumentException("Wrong combination of arguments");

            switch (args[2])
            {
                case "aes":
                    _algorithm = Aes.Create();
                    break;
                case "des":
                    _algorithm = DES.Create();
                    break;
                case  "rc2":
                    _algorithm = RC2.Create();
                    break;
                case "rijndael":
                    _algorithm = Rijndael.Create();
                    break;
                default:
                    throw new ArgumentException("Wrong/unsupported algorithm");
            }
        }

        public void Run()
        {
            if (_toEncrypt) Encrypt();
            else Decrypt();
        }

        private void Encrypt()
        {
            _algorithm.GenerateIV();
            _algorithm.GenerateKey();

            using (var inputFileStream = new FileStream(_plainFileName, FileMode.Open, FileAccess.Read))
            {
                using (
                    var cryptoStream = new CryptoStream(inputFileStream, _algorithm.CreateEncryptor(),
                        CryptoStreamMode.Read))
                {
                    using (var outputFileStream = new FileStream(_cryptoFileName, FileMode.Create, FileAccess.Write))
                    {
                        cryptoStream.CopyTo(outputFileStream);
                    }
                }
            }

            string keyFilePath = Path.GetDirectoryName(_cryptoFileName);
            string fileName = Path.GetFileNameWithoutExtension(_plainFileName);
            string fileExtension = Path.GetExtension(_plainFileName);

            string keyFileName = string.Concat(fileName, ".key", fileExtension);
            if (keyFilePath != null) keyFileName = Path.Combine(keyFilePath, keyFileName);

            File.WriteAllLines(keyFileName, new []
            {
                Convert.ToBase64String(_algorithm.IV),
                Convert.ToBase64String(_algorithm.Key)
            });
        }

        private void Decrypt()
        {
            string[] keyLines = File.ReadAllLines(_keyFileName);
            _algorithm.IV = Convert.FromBase64String(keyLines[0]);
            _algorithm.Key = Convert.FromBase64String(keyLines[1]);

            using (var inputFileStream = new FileStream(_cryptoFileName, FileMode.Open, FileAccess.Read))
            {
                using (
                    var cryptoStream = new CryptoStream(inputFileStream, _algorithm.CreateDecryptor(),
                        CryptoStreamMode.Read))
                {
                    using (var outputFileStream = new FileStream(_plainFileName, FileMode.Create, FileAccess.Write))
                    {
                        cryptoStream.CopyTo(outputFileStream);
                    }
                }
            }
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new Enigma(args).Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
