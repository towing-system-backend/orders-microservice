using Application.Core;

namespace Order.Domain;

public class InvalidOrderIssuerIdException() : DomainException("Issuer id must be a valid guid");