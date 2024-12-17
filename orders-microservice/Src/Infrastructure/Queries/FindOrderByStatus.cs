using Application.Core;
using MongoDB.Driver;
using order.Infrastructure.Responses;
using orders_microservice.Application.Errors;
using orders_microservice.Infrastructure.Controllers;
using orders_microservice.Infrastructure.Controllers.Dtos;

namespace orders_microservice.Infrastructure.queries;

public class FindOrderByStatusQuery : IService<FindOrderByStatusDto, List<FindOrderByStatusResponse>>
{
    private readonly IMongoCollection<MongoOrder> _orderCollection;
    public FindOrderByStatusQuery()
    {
        MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI"));
        IMongoDatabase database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME"));
        _orderCollection = database.GetCollection<MongoOrder>("orders");
    }
    public async Task<Result<List<FindOrderByStatusResponse>>> Execute(FindOrderByStatusDto query)
    {
        var filter = Builders<MongoOrder>.Filter.Eq(order => order.Status, query.Status);
        var orders = await _orderCollection.Find(filter)
            .Project(order => new FindOrderByStatusResponse
            {
                Status = order.Status,
                IssueLocation = order.IssueLocation,
                Destination = order.Destination,
                TowDriverAssigned = order.TowDriverAssigned,
                Details = order.Details,
                Name = order.Name,
                Image = order.Image,
                Policy = order.PolicyId,
                PhoneNumber = order.PhoneNumber,
            })
            .ToListAsync();
    
        if (!orders.Any()) throw new OrdersNotFoundError();
        return Result<List<FindOrderByStatusResponse>>.MakeSuccess(orders);
    }
}