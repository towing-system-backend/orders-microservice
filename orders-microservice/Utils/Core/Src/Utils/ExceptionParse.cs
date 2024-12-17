namespace Application.Core
{
    public static class ExceptionParser
    {
        public static Exception Parse(Exception e)
        {
            if (e is ApplicationError)
            {
                return new InvalidOperationException(e.Message);
            }

            return new InvalidDataException(e.Message);
        }
    }
}