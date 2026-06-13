using System.Net;
using System.Net.Sockets;

public class Program
{
    static SessionManager _sessions = new SessionManager();
    static PacketDispatcher _dispatcher = new PacketDispatcher();
    static MatchMaker _matchMaker = new MatchMaker();

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
        _dispatcher.Register(new LoginHandler(_matchMaker));
    }
}
