using System.Security.Cryptography;
using System.Text;

namespace GameClub.Logic
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            if (password == null)
                password = string.Empty;

            using (var sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);

                var sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }
        public static bool Verify(string password, string hash)
        {
            if (string.IsNullOrEmpty(hash))
                return false;

            return Hash(password) == hash.ToLower();
        }
    }
}