using System.Net.Sockets;

public class ClientSession
{
    public delegate Task SessionCallback(ClientSession session, byte[] data);

    private static int _nextId = 0;

    public int Id { get; }
    private readonly TcpClient _client;
    private readonly PacketChannel _channel;

    public string? UserName;

    public SessionCallback? OnPacketCallback;
    public Action<ClientSession>? OnDisconnect;

    public ClientSession(TcpClient client)
    {
        Id = Interlocked.Increment(ref _nextId);
        _client = client;
        _channel = new PacketChannel(client.GetStream(), HandlePacket);
    }

    private Task HandlePacket(byte[] data)
        => OnPacketCallback?.Invoke(this, data) ?? Task.CompletedTask;

    public async Task RunAsync()
    {
        try
        {
            await _channel.RunAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"[세션 {Id}] 오류: {e.Message}");
        }
        finally
        {
            OnDisconnect?.Invoke(this);
            _client.Close();
        }
    }

    public Task SendAsync(IPacket packet) => _channel.SendAsync(packet);
}