﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace BiddingService;

[ApiController]
[Route("api/[controller]")]
public class BidsController : ControllerBase
{

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Bid>> PlaceBid(string auctionId, int amount)
    {
        var auction = await DB.Find<Auction>().OneAsync(auctionId);

        if (auction == null) {
            //TODO: check with auction service if that has auction
            return NotFound();
        }

        if (auction.Seller == User.Identity.Name) {
            return BadRequest("You cannot bid on your own auction");
        }

        var bid = new Bid
        {
            Amount = amount,
            AuctionId = auctionId,
            Bidder = User.Identity.Name
        };


        if (auction.AuctionEnd < DateTime.UtcNow) {
            bid.BidStatus = BidStatus.Finished;
        } else {
            var highBid = await DB.Find<Bid>()
            .Match(b => b.AuctionId == auctionId)
            .Sort(b => b.Descending(x => x.Amount))
            .ExecuteFirstAsync();

            if (highBid != null && highBid.Amount < amount || highBid == null)
            {
                bid.BidStatus = amount > auction.ReservePrice
                ? BidStatus.Accepted
                : BidStatus.AcceptedBelowReserve;
            }

            if (highBid != null && bid.Amount <= highBid.Amount)
            {
                bid.BidStatus = BidStatus.TooLow;
            }
        }


        await DB.SaveAsync(bid);

        return Ok(bid);

    }
}
