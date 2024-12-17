using System.Text.RegularExpressions;

namespace orders_microservice.Utils.Core.Src.Utils
{
    public static class PhoneNumberRegex
    {
        public static bool IsPhoneNumber(string value)
        {
            return Regex.IsMatch(value, @"^(\+58)?0(41[246]|42[46]|2\d{2})\d{7}$");
        }
    }
}

