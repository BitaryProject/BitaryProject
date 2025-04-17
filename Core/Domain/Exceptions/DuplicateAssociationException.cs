namespace Domain.Exceptions
{
    public class DuplicateAssociationException : Exception
    {
        public DuplicateAssociationException(string message) : base(message) { }
    }
}