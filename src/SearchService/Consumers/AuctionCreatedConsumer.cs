using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    private readonly IMapper _mapper;

    public AuctionCreatedConsumer(IMapper mapper)
    {
        _mapper = mapper; 
    }

    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine($"Received AuctionCreated event: {context.Message.Id}");

        var item = _mapper.Map<Item>(context.Message);

        var result = await DB.Update<Item>()
            .Match(a => a.ID.ToString() == context.Message.Id.ToString())
            .ModifyOnly(x => new 
            {
                x.Make,
                x.Model,
                x.Year, 
                x.Color,
                x.Mileage
            }, item)
            .ExecuteAsync();

        if (!result.IsAcknowledged) {
            throw new MessageException(typeof(AuctionUpdated), "Problem updating mongodb");
        }

        await item.SaveAsync(); 
    }
}
