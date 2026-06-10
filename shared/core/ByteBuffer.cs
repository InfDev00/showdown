using System;

public class ByteBuffer
{
    protected byte[] _buffer = new byte[256];
    protected int _position = 0;

    public int Length => _position;

    protected void EnsureCapacity(int additional)
    {
        int required = _position + additional;
        if (required > _buffer.Length)
        {
            int newSize = _buffer.Length * 2;
            while (newSize < required)
                newSize *= 2;
            Array.Resize(ref _buffer, newSize);
        }
    }
}