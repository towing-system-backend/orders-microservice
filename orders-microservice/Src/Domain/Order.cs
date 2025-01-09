using Application.Core;

namespace Order.Domain
{
    public class Order : AggregateRoot<OrderId>
    {
        private new OrderId Id;
        private OrderStatus _status;
        private OrderIssueLocation _issueLocation;
        private OrderDestinationLocation _destinationLocation;
        private OrderIssuer _issuer;
        private OrderTowDriverAssigned _towDriverAssigned;
        private OrderDetails _details;
        private OrderClientInformation _clientInformation;
        private List<AdditionalCost>? _additionalCosts;
        private OrderTotalCost _totalCost;

        private Order(OrderId orderId) : base(orderId)
        {
            Id = orderId;
        }

        public override void ValidateState()
        {
            if (Id == null ||
                _status == null ||
                _issueLocation == null ||
                _destinationLocation == null ||
                _details == null ||
                _clientInformation == null ||
                _issuer == null
                )
            {
                throw new InvalidOrderException();
            }
        }

        public OrderId GetOrderId() => Id;
        public OrderStatus GetOrderStatus() => _status;
        public OrderIssueLocation GetOrderIssueLocation() => _issueLocation;
        public OrderDestinationLocation GetOrderDestinationLocation() => _destinationLocation;
        public OrderDetails GetOrderDetails() => _details;
        public OrderIssuer GetOrderIssuer() => _issuer;
        public OrderTowDriverAssigned GetOrderTowDriverAssigned() => _towDriverAssigned;
        public OrderClientInformation GetOrderClientInformation() => _clientInformation;
        public List<AdditionalCost>? GetAdditionalCosts() => _additionalCosts;
        public OrderTotalCost GetOrderTotalCost() => _totalCost;

        public static Order Create(
            OrderId id,
            OrderStatus status,
            OrderIssueLocation issueLocation,
            OrderDestinationLocation destinationLocation,
            OrderTowDriverAssigned towdriverAssigned,
            OrderIssuer issuer,
            OrderDetails details,
            OrderClientInformation clientInformation,
            OrderTotalCost totalCost,
            List<AdditionalCost>? additionalCosts,
            bool fromPersistence = false
            )
        {
            if (fromPersistence)
            {
                return new Order(id)
                {
                    Id = id,
                    _status = status,
                    _issueLocation = issueLocation,
                    _destinationLocation = destinationLocation,
                    _issuer = issuer,
                    _towDriverAssigned = towdriverAssigned,
                    _details = details,
                    _clientInformation = clientInformation,
                    _totalCost = totalCost,
                    _additionalCosts = additionalCosts
                };
            }

            Order order = new Order(id);
            order.Apply(
                OrderCreated.CreateEvent(
                    id,
                    status,
                    issueLocation,
                    destinationLocation,
                    towdriverAssigned,
                    issuer,
                    details,
                    clientInformation,
                    totalCost
                )
            );

            return order;
        }

        public void UpdateOrderStatus(OrderStatus status)
        {
            Apply(OrderStatusUpdated.CreateEvent(Id, status));
        }

        public void UpdateOrderTowDriverAssigned(OrderTowDriverAssigned towDriver)
        {
            Apply(OrderTowDriverAssignedUpdated.CreateEvent(Id, towDriver));
        }

        public void UpdateOrderDestinationLocation(OrderDestinationLocation destinationLocation)
        {
            Apply(OrderDestinationLocationUpdated.CreateEvent(Id, destinationLocation));
        }

        private void OnOrderCreatedEvent(OrderCreated context)
        {
            _status = new OrderStatus(context.Status);
            _issueLocation = new OrderIssueLocation(context.IssueLocation);
            _destinationLocation = new OrderDestinationLocation(context.Destination);
            _issuer = new OrderIssuer(context.Issuer);
            _details = new OrderDetails(context.Details);
            _clientInformation = new OrderClientInformation
                (context.Name, context.Image, context.PolicyId, context.PhoneNumber, context.IdentificationNumber);
            _totalCost = new OrderTotalCost(context.TotalCost);
            _additionalCosts = context.AdditionalCosts;
        }

        private void OnOrderStatusUpdatedEvent(OrderStatusUpdated context)
        {
            _status = new OrderStatus(context.Status);
        }

        private void OnOrderTowDriverAssignedUpdatedEvent(OrderTowDriverAssignedUpdated context)
        {
            _towDriverAssigned = new OrderTowDriverAssigned(context.TowDriverAssigned);
        }

        private void OnOrderDestinationLocationUpdatedEvent(OrderDestinationLocationUpdated context)
        {
            _destinationLocation = new OrderDestinationLocation(context.DestinationLocation);
        }

        public void CreateAdditionalCost(
            AdditionalCostId id,
            AdditionalCostName name,
            AdditionalCostCategory category,
            AdditionalCostAmount amount
        )
        {
            Apply(AdditionalCostCreated.CreateEvent(Id, id, amount, category, name));
        }

        private void OnCreateAdditionalCostEvent(AdditionalCostCreated context)
        {
            _additionalCosts ??= new List<AdditionalCost>();

            _additionalCosts.Add(
                new AdditionalCost(
                    new AdditionalCostId(context.Id),
                    new AdditionalCostAmount(context.Amount),
                    new AdditionalCostName(context.Name),
                    new AdditionalCostCategory(context.Category)
                )
            );
        }

        public void RemoveAdditionalCost(AdditionalCostId id)
        {
            Apply(AdditionalCostRemoved.CreateEvent(Id, id));
        }

        private void OnAdditionalCostRemovedEvent(AdditionalCostRemoved context)
        {
            _additionalCosts?.RemoveAll(x =>
            {
                return x.GetAdditionalCostId().GetValue() == context.Id;
            });
        }
    }
}