using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.PasswordHash
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 128 bit 
        private const int KeySize = 32; // 256 bit
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
        private const char Delimiter = ';';

        public string HashPassword(string password)
        {
            // Random salt yaratamiz
            var salt = RandomNumberGenerator.GetBytes(SaltSize);

            // Hash qilamiz
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                _hashAlgorithmName,
                KeySize);

            // Salt va hash ni birlashtiramiz
            var saltString = Convert.ToBase64String(salt);
            var hashString = Convert.ToBase64String(hash);

            // Natijani string ko'rinishida qaytaramiz
            return $"{saltString}{Delimiter}{hashString}";
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            // Hashlangan parolni qismlarga ajratamiz
            var parts = hashedPassword.Split(Delimiter);
            if (parts.Length != 2)
            {
                throw new FormatException("Hashed password format is invalid.");
            }

            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);

            // Kiritilgan parolni hash qilamiz
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                _hashAlgorithmName,
                KeySize);

            // Hashlarni solishtiramiz
            return CryptographicOperations.FixedTimeEquals(hash, hashToCompare);
        }
    }
}
