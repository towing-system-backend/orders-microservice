using System;
using Application.Core;
using orders_microservice.Domain.Events;
using orders_microservice.Domain.Exceptions;
using orders_microservice.Domain.ValueObjects;
using orders_microservice.Src.Domain.Entities.AdditionalCost;
using orders_microservice.Src.Domain.Entities.AdditionalCost.ValueObjects;
using orders_microservice.Src.Domain.Events;
using orders_microservice.Src.Domain.ValueObjects;

public class Order : AggregateRoot<OrderId>
{
	private OrderId Id; 
	private OrderStatus Status;
	private OrderIssueLocation IssueLocation;
	private OrderDestinationLocation DestinationLocation;
	private OrderTowDriverAssigned? TowDriverAssigned;
	private OrderDetails Details;
	private OrderClientInformation ClientInformation;
	private List<AdditionalCost>? AdditionalCosts;
	private OrderTotalCost TotalCost;

	private Order(OrderId OrderId) : base(OrderId)
	{
		Id = OrderId;
	}

	public override void ValidateState()
	{
		if (Id == null ||
		    Status == null ||
		    IssueLocation == null ||
		    DestinationLocation == null ||
		    Details == null ||
		    ClientInformation == null
		    )
		{
			throw new InvalidOrderException();
		}
	}
	
	public OrderId GetOrderId => Id;
	public OrderStatus GetOrderStatus => Status;
	public OrderIssueLocation GetOrderIssueLocation => IssueLocation;
	public OrderDestinationLocation GetOrderDestinationLocation => DestinationLocation;
	public OrderTowDriverAssigned GetOrderTowDriverAssigned => TowDriverAssigned!;
	public OrderDetails GetOrderDetails => Details;
	public OrderClientInformation GetOrderClientInformation => ClientInformation;
	public List<AdditionalCost>? GetAdditionalCosts => AdditionalCosts;
	public OrderTotalCost GetOrderTotalCost => TotalCost;

	public static Order Create(
		OrderId id, 
		OrderStatus status, 
		OrderIssueLocation issueLocation, 
		OrderDestinationLocation destinationLocation,
		OrderTowDriverAssigned todriverAssigned,
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
				Status = status,
				IssueLocation = issueLocation,
				DestinationLocation = destinationLocation,
				TowDriverAssigned = todriverAssigned,
				Details = details,
				ClientInformation = clientInformation,
				TotalCost = totalCost,
				AdditionalCosts = additionalCosts
			};
		}

		Order order = new Order(id);
		order.Apply(
			OrderCreated.CreateEvent(
				id, 
				status, 
				issueLocation, 
				destinationLocation,
				details,
				clientInformation,
                totalCost,
				additionalCosts!
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
        Status = new OrderStatus(context.Status);
        IssueLocation = new OrderIssueLocation(context.IssueLocation);
        DestinationLocation = new OrderDestinationLocation(context.Destination);
        Details = new OrderDetails(context.Details);
        ClientInformation = new OrderClientInformation(context.Name, context.Image, context.PolicyId, context.PhoneNumber);
		TotalCost = new OrderTotalCost(context.TotalCost);
        AdditionalCosts = context.AdditionalCosts;
    }

    private void OnOrderStatusUpdatedEvent(OrderStatusUpdated context)
	{
		Status = new OrderStatus(context.Status);
	}
	
	private void OnOrderTowDriverAssignedUpdatedEvent(OrderTowDriverAssignedUpdated context)
	{
		TowDriverAssigned = new OrderTowDriverAssigned(context.TowDriverAssigned);
	}
	
	private void OnOrderDestinationLocationUpdatedEvent(OrderDestinationLocationUpdated context)
	{
		DestinationLocation = new OrderDestinationLocation(context.DestinationLocation);
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
		if(AdditionalCosts == null) AdditionalCosts = new List<AdditionalCost>();

        AdditionalCosts.Add(
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
        AdditionalCosts.RemoveAll(x => x.GetAdditionalCostId.GetValue() == context.Id);
    }
}
