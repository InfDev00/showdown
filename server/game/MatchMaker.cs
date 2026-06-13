public class MatchMaker
{
    private readonly object _lock = new object();
    private Queue<ClientSession> _waiting = new Queue<ClientSession>();

    public void Enqueue(ClientSession session1)
    {
        GameRoom? room = null;

        lock (_lock)
        {
            if (_waiting.TryDequeue(out var session2))
            {
                // 먼저 들어온 사람 선공 처리
                room = new GameRoom(session2, session1);
            }
            else
            {
                _waiting.Enqueue(session1);
            }
        }

        if (room != null)
        {
            room?.SendMatchFound();
        }
        else
            _ = session1.SendAsync(new MatchResultPacket
            {
                MatchStatus = Status.Waiting,
            });
    }
}