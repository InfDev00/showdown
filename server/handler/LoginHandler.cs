public class LoginHandler : IHandler
{
    public PacketId Id => PacketId.Login;

    private readonly MatchMaker _matchMaker;

    public LoginHandler(MatchMaker matchMaker)
    {
        _matchMaker = matchMaker;
    }

    public async Task Handle(ClientSession session, byte[] data)
    {
        var login = Serializer.Deserialize<LoginPacket>(data, 0, data.Length);
        session.UserName = login.UserName;
        Console.WriteLine($"로그인 요청: {login.UserName}");

        await session.SendAsync(new LoginResultPacket
        {
            Success = true,
            Message = $"환영합니다. {login.UserName}"
        });

        _matchMaker.Enqueue(session);
    }
}