using System;
using System.Buffers.Binary;
using System.Text;

public class PacketWriter : ByteBuffer
{
    public byte[] ToArray()
    {
        var result = new byte[_position];
        Array.Copy(_buffer, result, _position);
        return result;
    }

    public void Write(byte value)
    {
        EnsureCapacity(1);
        _buffer[_position++] = value;
    }

    public void Write(bool value) => Write((byte)(value ? 1 : 0));

    public void Write(short value)
    {
        EnsureCapacity(2);
        BinaryPrimitives.WriteInt16LittleEndian(_buffer.AsSpan(_position), value);
        _position += 2;
    }

    public void Write(ushort value)
    {
        EnsureCapacity(2);
        BinaryPrimitives.WriteUInt16LittleEndian(_buffer.AsSpan(_position), value);
        _position += 2;
    }

    public void Write(int value)
    {
        EnsureCapacity(4);
        BinaryPrimitives.WriteInt32LittleEndian(_buffer.AsSpan(_position), value);
        _position += 4;
    }

    public void Write(uint value)
    {
        EnsureCapacity(4);
        BinaryPrimitives.WriteUInt32LittleEndian(_buffer.AsSpan(_position), value);
        _position += 4;
    }

    public void Write(long value)
    {
        EnsureCapacity(8);
        BinaryPrimitives.WriteInt64LittleEndian(_buffer.AsSpan(_position), value);
        _position += 8;
    }

    public void Write(ulong value)
    {
        EnsureCapacity(8);
        BinaryPrimitives.WriteUInt64LittleEndian(_buffer.AsSpan(_position), value);
        _position += 8;
    }

    // float은 netstandard2.1에 LE 전용 메서드가 없어 int 비트로 변환해 기록
    public void Write(float value) => Write(BitConverter.SingleToInt32Bits(value));

    // 문자열: [ushort 바이트길이] + UTF8 바이트
    public void Write(string? value)
    {
        value ??= string.Empty;
        int byteCount = Encoding.UTF8.GetByteCount(value);
        if (byteCount > ushort.MaxValue)
            throw new ArgumentException($"문자열이 너무 깁니다: {byteCount} bytes (최대 {ushort.MaxValue})");

        Write((ushort)byteCount);
        EnsureCapacity(byteCount);
        Encoding.UTF8.GetBytes(value, 0, value.Length, _buffer, _position);
        _position += byteCount;
    }

    // 원시 바이트를 그대로 이어붙인다 (프레임 payload 조립 등)
    public void WriteBytes(byte[] value)
    {
        if (value == null || value.Length == 0)
            return;

        EnsureCapacity(value.Length);
        Array.Copy(value, 0, _buffer, _position, value.Length);
        _position += value.Length;
    }
}