using System.Text.RegularExpressions;

namespace Application.Core
{
   public static class EmailRegex{
    public static bool IsEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
   } 
}