namespace MetadataHandlerLambda;

public class EmptyEventException : Exception
{
    public EmptyEventException()
    {
    }

    public EmptyEventException(string message)
        : base(message)
    {
    }

    public EmptyEventException(string message, Exception inner)
        : base(message, inner)
    {
    }
}