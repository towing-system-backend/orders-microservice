using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderIssueLocationException : DomainException
    {
        public InvalidOrderIssueLocationException() : base("Invalid order issue location.") { }
    }
}