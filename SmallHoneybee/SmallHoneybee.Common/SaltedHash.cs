using System;
using System.Security.Cryptography;
using System.Text;

namespace SmallHoneybee.Common
{
    public sealed class SaltedHash
    {
        private const int _saltLength = 6;

        private string _salt;
        private string _hash;

        private SaltedHash(string salt, string hash)
        {
            _salt = salt;
            _hash = hash;
        }

        public static SaltedHash Create(string password)
        {
            string salt = NewSalt();
            string hash = ComputeHash(salt, password);

            return new SaltedHash(salt, hash);
        }

        public static SaltedHash Create(string salt, string hash)
        {
            return new SaltedHash(salt, hash);
        }

        public string Salt
        {
            get
            {
                return _salt;
            }
        }

        public string Hash
        {
            get
            {
                return _hash;
            }
        }

        public bool Verify(string password)
        {
            return _hash.Equals(ComputeHash(_salt, password));
        }

        /// <summary>
        /// Create a new salt.
        /// </summary>
        /// <returns>New salt.</returns>
        private static string NewSalt()
        {
            return NewString(_saltLength);
        }

        private static string ComputeHash(string salt, string password)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(salt + password);
            (new SHA1CryptoServiceProvider()).ComputeHash(buffer);

            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// Generate a new string with specified length.
        /// </summary>
        /// <param name="length">String length.</param>
        /// <returns>Generated string.</returns>
        private static string NewString(int length)
        {
            byte[] buffer = new byte[(length * 7 + 7) / 8];
            (new RNGCryptoServiceProvider()).GetBytes(buffer);

            return Convert.ToBase64String(buffer).Substring(0, length);
        }
    }
}