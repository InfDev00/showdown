public class LoginResultPacket : IPacket
{
    public PacketId ID => PacketId.LoginResult;

    public bool Success;
    public string? Message;

    public void Read(PacketReader reader)
    {
        Success = reader.ReadBool();
        Message = reader.ReadString();
    }

    public void Write(PacketWriter writer)
    {
        writer.Write(Success);
        writer.Write(Message);
    }
}