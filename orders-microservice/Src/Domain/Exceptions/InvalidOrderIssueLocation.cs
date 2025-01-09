using Application.Core;

namespace Order.Domain
{
    public class InvalidOrderIssueLocationException() : DomainException("Invalid order issue location.");
}