using MassTransit;

namespace Application.Core
{
    public class OrderStateMachine : MassTransitStateMachine<OrderStatusStates>
    {
        public State ToAssign { get; private set; }
        public State Cancelled { get; private set; }
        public State Accepted { get; private set; }
        public State ToAccept { get; private set; }
        public State Located { get; private set; }
        public State Completed { get; private set; }
        public State Paid { get; private set; }
        public State InProgress { get; private set; }
        public Event<OrderCreatedEventt> OrderCreated { get; private set; }
        public Event<UpdateOrderStatusEvent> OrderUpdated { get; private set; }
        public Event<OrderCancelledEvent> OrderCancelled { get; private set; }
        public Event<OrderRejectedEvent> OrderRejected { get; private set; }


        public OrderStateMachine()
        {
            InstanceState(order => order.CurrentState);
            Event(() => OrderCreated, x => x.CorrelateById(order =>
                order.Message.OrderId));
            Event(() => OrderUpdated, x => x.CorrelateById(order =>
                order.Message.OrderId));
            Event(() => OrderCancelled, x => x.CorrelateById(order =>
                order.Message.OrderId));
            Event(() => OrderRejected, x => x.CorrelateById(order =>
                order.Message.OrderId));

            Initially(
                When(OrderCreated)
                .Then(context =>
                    {
                        context.Saga.CorrelationId = context.Message.OrderId;
                        context.Saga.CreatedAt = DateTime.UtcNow;
                    }
                )
                .TransitionTo(ToAssign)
            );

            During(ToAssign,
                When(OrderUpdated)
                .Then(context =>
                    {
                        context.Saga.LastStateChange = DateTime.UtcNow;
                    }
                )
                .TransitionTo(ToAccept)
            );

            During(ToAccept,
                When(OrderUpdated)
                .Then(context =>
                    {
                        context.Saga.LastStateChange = DateTime.UtcNow;
                    }
                )
                .TransitionTo(Accepted),
                When(OrderRejected)
                .Then(context =>
                    {
                        context.Saga.LastStateChange = DateTime.UtcNow;
                        context.Saga.DriversThatRejected.Add(context.Message.TowDriverId!);
                    }
                )
                .TransitionTo(ToAssign)
            );

            During(Accepted,
                When(OrderUpdated)
                .Then(context =>
                    {
                        context.Saga.LastStateChange = DateTime.UtcNow;
                    }
                )
                .TransitionTo(Located)
            );

            During(Located,
                When(OrderUpdated)
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
                When(OrderUpdated)
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
                When(OrderUpdated)
                .Then(context =>
                    {
                        context.Saga.LastStateChange = DateTime.UtcNow;
                    }
                )
                .TransitionTo(Paid)
            );

            During(Paid,
                When(OrderUpdated)
                .Then(context =>
                    {
                        context.Saga.LastStateChange = DateTime.UtcNow;
                    }
                )
            );

            During(Cancelled,
                When(OrderCancelled)
                .Then(context =>
                    {
                        context.Saga.LastStateChange = DateTime.UtcNow;
                    }
                )
            );
        }
    }
}