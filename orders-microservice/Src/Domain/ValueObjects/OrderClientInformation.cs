using Application.Core;

namespace Order.Domain
{
    public class OrderClientInformation : IValueObject<OrderClientInformation>
    {
        private readonly string _name;
        private readonly string _image;
        private readonly string _policyId;
        private readonly string _phoneNumber;
        private readonly int _identificationNumber;

        public OrderClientInformation(
            string name,
            string image,
            string policyId,
            string phoneNumber,
            int identificationNumber
        )
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

            if (identificationNumber is not > 999999 and < 100000000)
            {
                throw new InvalidOrderClientInformationIdentificationNumberException();
            }

            _name = name;
            _image = image;
            _policyId = policyId;
            _phoneNumber = phoneNumber;
            _identificationNumber = identificationNumber;
        }

        public string GetClientName() => _name;
        public string GetClientImage() => _image;
        public string GetClientPolicyId() => _policyId;
        public string GetClientPhoneNumber() => _phoneNumber;
        public int GetClientIdentificationNumber() => _identificationNumber;
        public bool Equals(OrderClientInformation other) => _policyId == other._policyId;
    }
}