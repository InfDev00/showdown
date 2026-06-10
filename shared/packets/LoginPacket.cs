public class LoginPacket : IPacket
{
    public PacketId ID => PacketId.Login;

    public string? UserName;
    public string? Password;

    public void Read(PacketReader reader)
    {
        UserName = reader.ReadString();
        Password = reader.ReadString();
    }

    public void Write(PacketWriter writer)
    {
        writer.Write(UserName);
        writer.Write(Password);
    }
}