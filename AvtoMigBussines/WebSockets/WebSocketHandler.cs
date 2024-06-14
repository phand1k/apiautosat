using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketHandler
{
    private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

    public async Task AddSocket(WebSocket socket)
    {
        string connId = Guid.NewGuid().ToString();
        _sockets.TryAdd(connId, socket);
        await ListenConnection(socket, connId);
    }

    private async Task ListenConnection(WebSocket socket, string connId)
    {
        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            // Handle incoming messages here if needed
        }
    }

    public async Task SendMessageToAllAsync(string message)
    {
        foreach (var pair in _sockets)
        {
            if (pair.Value.State == WebSocketState.Open)
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                var segment = new ArraySegment<byte>(buffer);
                await pair.Value.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

    public async Task RemoveSocket(string connId)
    {
        if (_sockets.TryRemove(connId, out WebSocket socket))
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by handler", CancellationToken.None);
            socket.Dispose();
        }
    }
}
