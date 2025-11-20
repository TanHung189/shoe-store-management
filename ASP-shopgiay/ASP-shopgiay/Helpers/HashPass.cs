using BCrypt.Net; // Gọi thư viện BCrypt

namespace ASP_shopgiay.Helpers
{
    public static class HashPass
    {
       
        public static string ToBCrypt(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool Verify(string passwordNhapVao, string passwordTrongDb)
        {
            return BCrypt.Net.BCrypt.Verify(passwordNhapVao, passwordTrongDb);
        }
    }
}