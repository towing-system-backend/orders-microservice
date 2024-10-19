namespace Application.Core
{
    public class Result<T>
    {
        private readonly Exception? Error;
        private readonly T? Value;

        public Result(Exception? error, T? value)
        {
            if (error != null & value != null)
            {
                throw new Exception("Error and value can't be defined at the same time.");
            }

            Error = error;
            Value = value;
        }

        public bool IsError => Error != null;
        public bool IsSuccess => !IsError;


        public T Unwrap()
        {
            if (IsError)
            {
                throw Error!;
            }
            return Value!;
        }

        static public Result<T> MakeError(Exception value)
        {
            return new Result<T>(value, default(T?));
        }

        static public Result<T> MakeSuccess(T value)
        {
            return new Result<T>(null, value);
        }
    }
}