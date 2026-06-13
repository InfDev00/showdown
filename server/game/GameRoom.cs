public class GameRoom
{
    private readonly ClientSession _player1;
    private readonly ClientSession _player2;

    public GameRoom(ClientSession player1, ClientSession player2)
    {
        _player1 = player1;
        _player2 = player2;
    }

    public void SendMatchFound()
    {
        _ = _player1.SendAsync(new MatchResultPacket
        {
            MatchStatus = Status.Success,
            OpponentName = _player2.UserName,
            IsMyTurnFirst = true,
        });
        _ = _player2.SendAsync(new MatchResultPacket
        {
            MatchStatus = Status.Success,
            OpponentName = _player1.UserName,
            IsMyTurnFirst = false,
        });
    }

    public Task BroadcastAsync(IPacket packet)
    {
        return Task.WhenAll(_player1.SendAsync(packet), _player2.SendAsync(packet));
    }
}