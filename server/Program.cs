using System.Net;
using System.Net.Sockets;

public class Program
{
    static SessionManager _sessions = new SessionManager();
    static PacketDispatcher _dispatcher = new PacketDispatcher();

    static async Task Main()
    {
        RegisterCallbacks();

        var listener = new TcpListener(IPAddress.Any, 8080);
        listener.Start();
        Console.WriteLine($"서버 시작. 대기중...");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            var session = new ClientSession(client);

            session.OnPacketCallback = _dispatcher.Dispatch;
            session.OnDisconnect = s =>
            {
                _sessions.Remove(s);
                Console.WriteLine($"세션 {s.Id} 접속 종료");
            };

            _sessions.Add(session);
            Console.WriteLine($"[세션 {session.Id}] 접속: {client.Client.RemoteEndPoint}");
            _ = session.RunAsync();
        }
    }

    static void RegisterCallbacks()
    {
        _dispatcher.Register(PacketId.Login, HandleLogin);
    }

    static async Task HandleLogin(ClientSession session, byte[] data)
    {
        var login = Serializer.Deserialize<LoginPacket>(data, 0, data.Length);
        Console.WriteLine($"로그인 요청: {login.UserName}");

        var result = new LoginResultPacket
        {
            Success = true,                 // 학습용 스텁: 항상 성공
            Message = $"환영합니다, {login.UserName}!"
        };

        await session.SendAsync(result);
    }
}
