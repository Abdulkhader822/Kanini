using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ArtAuction.Hubs
{
    public class AuctionHub : Hub
    {
        // Join group per artwork
        public Task JoinArtworkGroup(int artworkId) =>
            Groups.AddToGroupAsync(Context.ConnectionId, $"artwork-{artworkId}");

        public Task LeaveArtworkGroup(int artworkId) =>
            Groups.RemoveFromGroupAsync(Context.ConnectionId, $"artwork-{artworkId}");
    }
}
