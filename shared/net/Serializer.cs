public interface IPacket
{
    PacketId ID { get; }
    void Write(PacketWriter writer);
    void Read(PacketReader reader);
}

public static class Serializer
{
    public static byte[] Serialize(IPacket packet)
    {
        var body = new PacketWriter();
        packet.Write(body);
        byte[] payload = body.ToArray();

        var frame = new PacketWriter();
        frame.Write((short)(4 + payload.Length));
        frame.Write((short)packet.ID);
        frame.WriteBytes(payload);
        return frame.ToArray();
    }

    public static PacketId PeekId(byte[] data, int offset, int count)
    {
        var reader = new PacketReader();
        reader.SetBuffer(data, offset, count);
        reader.ReadShort();                 // size 건너뜀
        return (PacketId)reader.ReadShort();
    }

    public static T Deserialize<T>(byte[] data, int offset, int count) where T : IPacket, new()
    {
        var reader = new PacketReader();
        reader.SetBuffer(data, offset, count);
        reader.ReadShort();                 // size 건너뜀
        reader.ReadShort();                 // id 건너뜀 (타입을 이미 알고 있음)

        var packet = new T();
        packet.Read(reader);
        return packet;
    }
}