using Microsoft.AspNetCore.SignalR;

namespace AplikasiCheckDimensi.Hubs
{
    /// <summary>
    /// SignalR Hub untuk real-time updates QC Data
    /// Broadcasts data baru ke semua client yang terhubung
    /// </summary>
    public class QCHub : Hub
    {
        /// <summary>
        /// Broadcast data QC baru ke semua connected clients
        /// Method ini akan dipanggil dari Controller setelah data di-save
        /// </summary>
        /// <param name="data">Object data QC yang baru di-submit</param>
        public async Task BroadcastNewQCData(object data)
        {
            // Send ke SEMUA client yang connected
            await Clients.All.SendAsync("NewQCDataAdded", data);
        }

        /// <summary>
        /// Broadcast NG update ke semua clients
        /// </summary>
        public async Task BroadcastNGUpdate(object data)
        {
            await Clients.All.SendAsync("NGDataUpdated", data);
        }

        /// <summary>
        /// Event saat client connect
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
        }

        /// <summary>
        /// Event saat client disconnect
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
        }
    }
}
