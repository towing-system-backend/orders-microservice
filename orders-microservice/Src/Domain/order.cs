using System;
using Application.Core;
using orders_microservice.Domain.Events;
using orders_microservice.Domain.Exceptions;
using orders_microservice.Domain.ValueObjects;

public class Order : AggregateRoot<OrderId>
{
	private OrderId Id; 
	private OrderStatus Status;
	private OrderIssueLocation IssueLocation;
	private OrderDestinationLocation DestinationLocation;
	private OrderTowDriverAssigned? TowDriverAssigned;
	private OrderDetails Details;
	private OrderClientInformation ClientInformation;

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
	
	public OrderId GetOrderId => this.Id;
	public OrderStatus GetOrderStatus => this.Status;
	public OrderIssueLocation GetOrderIssueLocation => this.IssueLocation;
	public OrderDestinationLocation GetOrderDestinationLocation => this.DestinationLocation;
	public OrderTowDriverAssigned GetOrderTowDriverAssigned => this.TowDriverAssigned;
	public OrderDetails GetOrderDetails => this.Details;
	public OrderClientInformation GetOrderClientInformation => this.ClientInformation;

	public static Order Create(
		OrderId id, 
		OrderStatus status, 
		OrderIssueLocation issueLocation, 
		OrderDestinationLocation destinationLocation,
		OrderTowDriverAssigned todriverAssigned,
		OrderDetails details,
		OrderClientInformation clientInformation,
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
				ClientInformation = clientInformation
			};
		}

		Order order = new Order(id);
		order.Apply(OrderCreated.CreateEvent(
			id, 
			status, 
			issueLocation, 
			destinationLocation,
			details,
			clientInformation
			)
		);
		return order;
	}

	private void OnOrderCreatedEvent(OrderCreated context)
	{
		Status = new OrderStatus(context.Status);
		IssueLocation = new OrderIssueLocation(context.IssueLocation);
		DestinationLocation = new OrderDestinationLocation(context.Destination);
		Details = new OrderDetails(context.Details);
		ClientInformation = new OrderClientInformation(context.Name, context.Image, context.PolicyId, context.PhoneNumber);	
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
}
