﻿using Contracts;
using MassTransit;

namespace AuctionService;

public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{

    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        Console.WriteLine($"Received AuctionCreated fault: {context.Message.Message}");

        var exception = context.Message.Exceptions.First();

        if (exception.ExceptionType == "System.ArgumentException") {
            context.Message.Message.Model = "FooBar";
            await context.Publish(context.Message.Message);
        } else {
            Console.WriteLine($"Unhandled exception: {exception.Message}");
        }
    }

}
