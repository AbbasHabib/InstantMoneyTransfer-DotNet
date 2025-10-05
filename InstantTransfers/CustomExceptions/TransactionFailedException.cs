namespace InstantTransfers.CustomExceptions;

public class TransactionFailedException : Exception
{
    public TransactionFailedException(string message) : base(message) { }
}

