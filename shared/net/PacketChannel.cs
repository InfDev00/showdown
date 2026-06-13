using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class PacketChannel
{
    public delegate Task PacketCallback(byte[] data);

    private readonly Stream _stream;
    private readonly PacketCallback _callback;
    public PacketChannel(Stream stream, PacketCallback callback)
    {
        _stream = stream;
        _callback = callback;
    }

    public async Task RunAsync(CancellationToken token = default)
    {
        var assembler = new PacketAssembler();
        var recv = new byte[1024];

        int n;
        do
        {
            n = await _stream.ReadAsync(recv, 0, recv.Length, token);
            if (n > 0)
            {
                assembler.Append(recv, 0, n);
                while (assembler.TryAssemblePacket(out var data))
                {
                    await _callback.Invoke(data!);
                }
            }
        } while (n > 0 && !token.IsCancellationRequested);
    }

    public async Task SendAsync(IPacket packet)
    {
        byte[] frame = Serializer.Serialize(packet);
        await _stream.WriteAsync(frame, 0, frame.Length);
    }
}