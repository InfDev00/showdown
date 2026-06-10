using System.Net;
using System.Net.Sockets;

public class Program
{
    static async Task Main()
    {
        var listener = new TcpListener(IPAddress.Any, 8080);
        listener.Start();
        Console.WriteLine($"서버 시작. 대기중...");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            Console.WriteLine($"클라이언트 접속: {client.Client.RemoteEndPoint}");

            _ = HandleClientAsync(client);
        }
    }

    static async Task HandleClientAsync(TcpClient client)
    {
        using (client)
        {
            var stream = client.GetStream();
            var assembler = new PacketAssembler();
            var recv = new byte[1024];

            int n = 0;
            do
            {
                n = await stream.ReadAsync(recv, 0, recv.Length);
                if (n > 0)
                {
                    assembler.Append(recv, 0, n);

                    while (assembler.TryAssemblePacket(out var packet))
                    {
                        await HandlePacket(packet!, stream);
                    }
                }


            } while (n > 0);
        }
    }

    static async Task HandlePacket(byte[] packet, NetworkStream stream)
    {
        var id = Serializer.PeekId(packet, 0, packet.Length);
        switch (id)
        {
            case PacketId.Login:
                var login = Serializer.Deserialize<LoginPacket>(packet, 0, packet.Length);
                Console.WriteLine($"로그인 요청: {login.UserName}");

                var result = new LoginResultPacket
                {
                    Success = true,                 // 학습용 스텁: 항상 성공
                    Message = $"환영합니다, {login.UserName}!"
                };
                byte[] response = Serializer.Serialize(result);
                await stream.WriteAsync(response, 0, response.Length);
                break;

            default:
                Console.WriteLine($"처리되지 않은 패킷: {id}");
                break;
        }
    }
}
