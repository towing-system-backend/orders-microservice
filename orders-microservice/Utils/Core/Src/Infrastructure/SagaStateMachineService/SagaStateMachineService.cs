using MassTransit;
using RabbitMQ.Contracts;

namespace Application.Core
{
    public class OrderStateMachine : MassTransitStateMachine<OrderStatusStates>
    {
        public State? ToAssign { get; private set; }
        public State? Cancelled { get; private set; }
        public State? Accepted { get; private set; }
        public State? ToAccept { get; private set; }
        public State? Located { get; private set; }
        public State? Completed { get; private set; }
        public State? Paid { get; private set; }
        public State? InProgress { get; private set; }
        public Event<EventOrderToAssign>? OrderToAssign { get; private set; }
        public Event<EventOrderToAccept> OrderToAccept { get; private set; }
        public Event<EventOrderAccepted> OrderAccepted { get; private set; }
        public Event<EventOrderLocated> OrderLocated { get; private set; }
        public Event<EventOrderInProgress> OrderInProgress { get; private set; }
        public Event<EventOrderCompleted> OrderCompleted { get; private set; }
        public Event<EventOrderPaid> OrderPaid { get; private set; }
        public Event<EventOrderCancelled>? OrderCancelled { get; private set; }
        
        public OrderStateMachine()
        {
            InstanceState(order => order.CurrentState);
            Event(() => OrderToAssign, x => x.CorrelateById(order =>
                order.Message.OrderId));
            Event(() => OrderToAccept, x => x.CorrelateById(order =>
                order.Message.OrderId));
            Event(() => OrderAccepted, x => x.CorrelateById(order =>
                order.Message.OrderId));
            Event(() => OrderLocated, x => x.CorrelateById(order =>
                order.Message.OrderId));
            Event(() => OrderInProgress, x => x.CorrelateById(order =>
                order.Message.OrderId));
            Event(() => OrderCompleted, x => x.CorrelateById(order =>
                order.Message.OrderId));
            Event(() => OrderPaid, x => x.CorrelateById(order =>
                order.Message.OrderId));
            Event(() => OrderCancelled, x => x.CorrelateById(order =>
                order.Message.OrderId));
            
            Initially(
                When(OrderToAssign)
                    .Then(context =>
                        {
                            context.Saga.CorrelationId = context.Message.OrderId;
                            context.Saga.CreatedAt = DateTime.UtcNow;
                        }
                    )
                    .TransitionTo(ToAssign)
            );

            During(ToAssign,
                When(OrderToAccept)
                    .IfElse(context => string.IsNullOrEmpty(context.Message.DeviceToken),
                        binder => binder
                            .Then(context =>
                            {
                                context.Saga.DriversThatRejected.Add(context.Message.TowDriverId!);
                            }),
                        binder => binder
                            .Then(context =>
                            {
                                context.Saga.DriverThatAccept = context.Message.TowDriverId;
                                context.Saga.DeviceToken = context.Message.DeviceToken;
                                context.Saga.LastStateChange = DateTime.UtcNow;
                            })
                            .TransitionTo(ToAccept)
                    ),
                When(OrderCancelled)
                    .Then(context =>
                    {
                        context.Saga.LastStateChange = DateTime.UtcNow;
                    })
                    .TransitionTo(Cancelled)
            );


            During(ToAccept,
                When(OrderAccepted)
                .Then(context =>
                    {
                        context.Saga.LastStateChange = DateTime.UtcNow;
                    }
                )
                .TransitionTo(Accepted),
                When(OrderToAccept)
                    .Then(context =>
                        {
                            context.Saga.LastStateChange = DateTime.UtcNow;
                            context.Saga.DriverThatAccept = string.Empty;
                            context.Saga.DeviceToken = string.Empty;
                            context.Saga.DriversThatRejected.Add(context.Message.TowDriverId!);
                        }
                    )
                    .TransitionTo(ToAssign),
                When(OrderCancelled)
                    .Then(context =>
                    {
                        context.Saga.LastStateChange = DateTime.UtcNow;
                    }
                    )
                    .TransitionTo(Cancelled)
            );

            During(Accepted,
                When(OrderLocated)
                    .Then(context =>
                        {
                            context.Saga.LastStateChange = DateTime.UtcNow;
                        }
                )
                .TransitionTo(Located),
                When(OrderCancelled)
                    .Then(context =>
                        {
                            context.Saga.LastStateChange = DateTime.UtcNow;
                        }
                    )
                    .TransitionTo(Cancelled)
            );

            During(Located,
                When(OrderInProgress)
                .Then(context =>
                    {
                        context.Saga.LastStateChange = DateTime.UtcNow;
                    }
                )
                .TransitionTo(InProgress),
                When(OrderCancelled)
                .Then(context =>
                    {
                        context.Saga.LastStateChange = DateTime.UtcNow;
                    }
                )
                .TransitionTo(Cancelled)
            );

            During(InProgress,
                When(OrderCompleted)
                .Then(context =>
                    {
                        context.Saga.LastStateChange = DateTime.UtcNow;
                    }
                )
                .TransitionTo(Completed),
                When(OrderCancelled)
                .Then(context =>
                    {
                        context.Saga.LastStateChange = DateTime.UtcNow;
                    }
                )
                .TransitionTo(Cancelled)
            );

            During(Completed,
                When(OrderPaid)
                .Then(context =>
                    {
                        context.Saga.LastStateChange = DateTime.UtcNow;
                    }
                )
                .TransitionTo(Paid)
            );
            
            During(Cancelled,
                Ignore(OrderCancelled)
            );
            
            During(Paid,
                Ignore(OrderPaid)
            );
            SetCompletedWhenFinalized();
        }
    }
}