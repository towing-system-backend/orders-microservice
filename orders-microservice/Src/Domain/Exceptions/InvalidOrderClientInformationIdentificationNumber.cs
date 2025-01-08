using Application.Core;

namespace Order.Domain;

public class InvalidOrderClientInformationIdentificationNumberException()
    : DomainException("Identification number must be valid");