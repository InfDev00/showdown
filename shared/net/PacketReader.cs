using System;
using System.Buffers.Binary;
using System.Text;

public class PacketReader : ByteBuffer
{
    int _limit = 0;

    public void SetBuffer(byte[] data, int offset, int count)
    {
        _buffer = data;
        _position = offset;
        _limit = offset + count;
    }

    void CheckCanRead(int size)
    {
        if (_position + size > _limit)
            throw new InvalidOperationException("PacketReader: 버퍼 이상을 읽기 시도했습니다.");
    }

    public byte ReadByte()
    {
        CheckCanRead(1);
        return _buffer[_position++];
    }

    public bool ReadBool() => ReadByte() != 0;

    public short ReadShort()
    {
        CheckCanRead(2);
        short value = BinaryPrimitives.ReadInt16LittleEndian(_buffer.AsSpan(_position));
        _position += 2;
        return value;
    }

    public ushort ReadUShort()
    {
        CheckCanRead(2);
        ushort value = BinaryPrimitives.ReadUInt16LittleEndian(_buffer.AsSpan(_position));
        _position += 2;
        return value;
    }

    public int ReadInt()
    {
        CheckCanRead(4);
        int value = BinaryPrimitives.ReadInt32LittleEndian(_buffer.AsSpan(_position));
        _position += 4;
        return value;
    }

    public uint ReadUInt()
    {
        CheckCanRead(4);
        uint value = BinaryPrimitives.ReadUInt32LittleEndian(_buffer.AsSpan(_position));
        _position += 4;
        return value;
    }

    public long ReadLong()
    {
        CheckCanRead(8);
        long value = BinaryPrimitives.ReadInt64LittleEndian(_buffer.AsSpan(_position));
        _position += 8;
        return value;
    }

    public ulong ReadULong()
    {
        CheckCanRead(8);
        ulong value = BinaryPrimitives.ReadUInt64LittleEndian(_buffer.AsSpan(_position));
        _position += 8;
        return value;
    }

    public float ReadFloat() => BitConverter.Int32BitsToSingle(ReadInt());

    // 문자열: [ushort 바이트길이] + UTF8 바이트
    public string ReadString()
    {
        int byteCount = ReadUShort();
        if (byteCount == 0)
            return string.Empty;

        CheckCanRead(byteCount);
        string value = Encoding.UTF8.GetString(_buffer, _position, byteCount);
        _position += byteCount;
        return value;
    }

    // 원시 바이트를 그대로 읽어 반환
    public byte[] ReadBytes(int count)
    {
        CheckCanRead(count);
        var result = new byte[count];
        Array.Copy(_buffer, _position, result, 0, count);
        _position += count;
        return result;
    }
}