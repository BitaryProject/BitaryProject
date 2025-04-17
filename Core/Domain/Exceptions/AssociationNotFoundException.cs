namespace Domain.Exceptions
{
    public class AssociationNotFoundException : Exception
    {
        public AssociationNotFoundException(string message) : base(message) { }
    }
}