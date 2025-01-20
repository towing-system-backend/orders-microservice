using Application.Core;

namespace Order.Application;

public class OrderNotAssignableError() : ApplicationError("This order is already assign or is pending for a response");
