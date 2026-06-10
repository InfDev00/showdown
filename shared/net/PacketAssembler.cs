using System;
using System.Buffers.Binary;

public class PacketAssembler : ByteBuffer
{
    public void Append(byte[] data, int offset, int count)
    {
        EnsureCapacity(count);
        Array.Copy(data, offset, _buffer, _position, count);
        _position += count;
    }

    public bool TryAssemblePacket(out byte[]? data)
    {
        data = null;

        // 길이 헤더(2바이트)도 아직 안 옴
        if (_position < 2)
            return false;

        // 앞 2바이트(LE)로 패킷 전체 길이 확인
        int totalLength = BinaryPrimitives.ReadUInt16LittleEndian(_buffer.AsSpan(0));

        // 패킷이 아직 다 도착하지 않음
        if (_position < totalLength)
            return false;

        // 완성된 패킷 한 개를 잘라낸다
        data = new byte[totalLength];
        Array.Copy(_buffer, 0, data, 0, totalLength);

        // 소비한 만큼 남은 바이트를 앞으로 당긴다 (compaction)
        int remaining = _position - totalLength;
        if (remaining > 0)
            Array.Copy(_buffer, totalLength, _buffer, 0, remaining);
        _position = remaining;

        return true;
    }
}