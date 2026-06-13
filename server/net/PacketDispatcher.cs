public class PacketDispatcher
{
    private readonly Dictionary<PacketId, ClientSession.SessionCallback> _callbacks = new Dictionary<PacketId, ClientSession.SessionCallback>();

    public void Register(PacketId id, ClientSession.SessionCallback callback)
    {
        _callbacks[id] = callback;
    }

    public async Task Dispatch(ClientSession session, byte[] data)
    {
        var id = Serializer.PeekId(data, 0, data.Length);
        if (_callbacks.TryGetValue(id, out var callback))
        {
            await callback.Invoke(session, data);
        }
        else
        {
            Console.WriteLine($"처리되지 않는 패킷: {id}");
        }
    }
}