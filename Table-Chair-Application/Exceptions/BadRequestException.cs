namespace Table_Chair_Application.Exceptions
{
    [Serializable]
    public class BadRequestException : Exception
    {
        public BadRequestException():base(){}

        public BadRequestException(string? message) : base(message)
        {
        }

        public BadRequestException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}