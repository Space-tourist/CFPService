namespace App.Exceptions;

public class NotFoundApplicationException : Exception
{
    public NotFoundApplicationException(string message)
        : base(message)
    {
    }
}