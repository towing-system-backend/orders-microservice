namespace Application.Core
{
    public class Optional<T>
    {
        private readonly T? Value;

        private Optional(T? value = default)
        {
            Value = value;
        }

        public bool HasValue()
        {
            return Value != null;
        }

        public T Unwrap()
        {
            if (Value == null)
            {
                throw new Exception("Optional is empty.");
            }
            return Value;
        }

        public static Optional<T> Of(T value)
        {
            return new Optional<T>(value);
        }

        public static Optional<T> Empty()
        {
            return new Optional<T>();
        }
    }
}