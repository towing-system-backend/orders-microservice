using System.ComponentModel.Design;
using Application.Core;
using MongoDB.Driver;
using order.Infrastructure.Responses;
using orders_microservice.Application.Errors;
using orders_microservice.Infrastructure.Controllers;
using orders_microservice.Infrastructure.Controllers.Dtos;

namespace orders_microservice.Infrastructure.queries;

public class FindOrderAssignedQuery : IService<FindOrderAssignedDto, FindOrderAssignedResponse>
{
    private readonly IMongoCollection<MongoOrder> _orderCollection;
    public FindOrderAssignedQuery()
    {
        MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI"));
        IMongoDatabase database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME"));
        _orderCollection = database.GetCollection<MongoOrder>("orders");
    }
    public async Task<Result<FindOrderAssignedResponse>> Execute(FindOrderAssignedDto query)
    {
        var filter = Builders<MongoOrder>.Filter.Eq(order => order.OrderId, query.Id);
        var res = await _orderCollection.Find(filter).FirstOrDefaultAsync();
    
        if (res == null) throw new OrderNotFoundError();
        if (res.TowDriverAssigned == "Not Assigned") throw new NotAssignedTowDriverError();
        
        var order = new FindOrderAssignedResponse
        {
            Status = res.Status,
            IssueLocation = res.IssueLocation,
            Destination = res.Destination,
            TowDriverAssigned = res.TowDriverAssigned,
            Details = res.Details,
            Name = res.Name,
            Image = res.Image,
            PhoneNumber = res.PhoneNumber
        };
        
        return Result<FindOrderAssignedResponse>.MakeSuccess(order);
    }
}