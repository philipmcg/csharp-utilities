using System;
using System.Security.Cryptography;
using System.Text;

namespace Utilities
{
        public static class Hashing
        {
            public enum HashType : int
            {
                MD5,
                SHA1,
                SHA256,
                SHA512
            }

            static readonly MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            static readonly SHA1Managed sha1 = new SHA1Managed();
            static readonly SHA256Managed sha256 = new SHA256Managed();
            static readonly SHA512Managed sha512 = new SHA512Managed();
            static readonly UnicodeEncoding UE = new UnicodeEncoding();

            public static string GetHash(string text, HashType hashType)
            {
                string hashString;
                switch (hashType)
                {
                    case HashType.MD5:
                        hashString = GetMD5(text);
                        break;
                    case HashType.SHA1:
                        hashString = GetSHA1(text);
                        break;
                    case HashType.SHA256:
                        hashString = GetSHA256(text);
                        break;
                    case HashType.SHA512:
                        hashString = GetSHA512(text);
                        break;
                    default:
                        hashString = "Invalid Hash Type";
                        break;
                }
                return hashString;
            }

            public static bool CheckHash(string original, string hashString, HashType hashType)
            {
                string originalHash = GetHash(original, hashType);
                return (originalHash == hashString);
            }

            public static string GetMD5(string text)
            {
                
                byte[] hashValue;
                byte[] message = UE.GetBytes(text);

                string hex = "";

                hashValue = md5.ComputeHash(message);
                foreach (byte x in hashValue)
                {
                    hex += String.Format("{0:x2}", x);
                }
                return hex;
            }

            public static string GetSHA1(string text)
            {
                
                byte[] hashValue;
                byte[] message = UE.GetBytes(text);

                string hex = "";

                hashValue = sha1.ComputeHash(message);
                foreach (byte x in hashValue)
                {
                    hex += String.Format("{0:x2}", x);
                }
                return hex;
            }

            public static string GetSHA256(string text)
            {
                
                byte[] hashValue;
                byte[] message = UE.GetBytes(text);

                string hex = "";

                hashValue = sha256.ComputeHash(message);
                foreach (byte x in hashValue)
                {
                    hex += String.Format("{0:x2}", x);
                }
                return hex;
            }

            public static string GetSHA512(string text)
            {
                
                byte[] hashValue;
                byte[] message = UE.GetBytes(text);

                string hex = "";

                hashValue = sha512.ComputeHash(message);
                foreach (byte x in hashValue)
                {
                    hex += String.Format("{0:x2}", x);
                }
                return hex;
            }

            public static Guid GetGuidMD5(string text)
            {
                
                byte[] hashValue;
                byte[] message = UE.GetBytes(text);


                hashValue = md5.ComputeHash(message);
                return new Guid(hashValue);
            }
        }
}
