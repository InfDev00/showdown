using System.Collections.Concurrent;

public class SessionManager
{
    private readonly ConcurrentDictionary<int, ClientSession> _sessions = new ConcurrentDictionary<int, ClientSession>();

    public void Add(ClientSession session)
    {
        _sessions[session.Id] = session;
    }

    public void Remove(ClientSession session)
    {
        _sessions.TryRemove(session.Id, out _);
    }

    public async Task BroadcastAsync(IPacket packet)
    {
        foreach (var session in _sessions.Values)
        {
            await session.SendAsync(packet);
        }
    }
}