﻿using Application.Core;
using orders_microservice.Domain.Exceptions;
using orders_microservice.Utils.Core.Src.Utils;

namespace orders_microservice.Domain.ValueObjects;

public class OrderClientInformation : IValueObject<OrderClientInformation>
{
    private readonly string _name;
    private readonly string _image;
    private readonly string _policyId;
    private readonly string _phoneNumber;

    public OrderClientInformation(string name, string image, string policyId, string phoneNumber)
    {
        if (name is not string)
        {
            throw new InvalidOrderClientInformationNameException();
        }

        if (!UrlRegex.IsUrl(image))
        {
            throw new InvalidOrderClientInformationImageException();
        }

        if (!GuidEx.IsGuid(policyId))
        {
            throw new InvalidOrderClientInformationPolicyException();
        }

        if (!PhoneNumberRegex.IsPhoneNumber(phoneNumber))
        {
            throw new InvalidOrderClientInformationPhoneNumberException();
        }
        
        _name = name;
        _image = image;
        _policyId = policyId;
        _phoneNumber = phoneNumber;
    }
    
    public string GetClientName() => _name;
    public string GetClientImage() => _image;
    public string GetClientPolicyId() => _policyId;
    public string GetClientPhoneNumber() => _phoneNumber;

    public bool Equals(OrderClientInformation other) => _policyId == other._policyId;
}