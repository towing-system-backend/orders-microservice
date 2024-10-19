namespace Application.Core
{
    public static class GuidEx
    {
        public static bool IsGuid(string value)
        {
            return Guid.TryParse(value, out Guid x);
        }
    }
}