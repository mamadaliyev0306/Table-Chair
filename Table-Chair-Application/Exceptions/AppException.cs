﻿namespace Table_Chair_Application.Exceptions
{
    [Serializable]
    internal class AppException : Exception
    {
        public AppException()
        {
        }

        public AppException(string? message) : base(message)
        {
        }

        public AppException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}