using System;
using System.Text;
using System.Security.Cryptography;

namespace Flixplorer.Helpers
{
    /// <summary>
    /// A helper class that handles the generation of salts and the hashing/verification of passwords.
    /// </summary>
    public class SecurityHelper
    {
        /// <summary>
        /// Generates a cryptographically secure random salt value.
        /// </summary>
        /// <returns>The generated salt value in base64 string format.</returns>
        public static string GenerateSalt()
        {
            byte[] salt = new byte[128 / 8]; // divide by 8 to convert bits to bytes

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        /// <summary>
        /// This method hashes the password using PBKDF2 algorithm with a given salt.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <param name="salt">The salt to be used for hashing.</param>
        /// <returns>The hashed password in base64 encoded string.</returns>
        public static string HashPassword(string password, string salt)
        {
            byte[] hashedPassword;

            int iterations = 10000;

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), iterations))
            {
                hashedPassword = pbkdf2.GetBytes(256 / 8); // divide by 8 to convert bits to bytes
            }

            return Convert.ToBase64String(hashedPassword);
        }

        /// <summary>
        /// Verifies if a saved password matches an input password.
        /// </summary>
        /// <param name="savedPassword">The password saved in the database.</param>
        /// <param name="inputPassword">The password provided by the user.</param>
        /// <returns>True if the saved password matches the input password, otherwise false.</returns>
        public static bool VerifyPassword(string savedPassword, string inputPassword)
        {
            return savedPassword.Equals(inputPassword);
        }
    }
}