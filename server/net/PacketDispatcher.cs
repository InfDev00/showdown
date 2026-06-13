public interface IHandler
{
    PacketId Id { get; }
    Task Handle(ClientSession session, byte[] data);
}

public class PacketDispatcher
{
    private readonly Dictionary<PacketId, IHandler> _handlers = new Dictionary<PacketId, IHandler>();

    public void Register(IHandler handler)
    {
        _handlers[handler.Id] = handler;
    }

    public async Task Dispatch(ClientSession session, byte[] data)
    {
        var id = Serializer.PeekId(data, 0, data.Length);
        if (_handlers.TryGetValue(id, out var handler))
        {
            await handler.Handle(session, data);
        }
        else
        {
            Console.WriteLine($"처리되지 않는 패킷: {id}");
        }
    }
}