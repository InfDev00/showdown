public class MatchResultPacket : IPacket
{
    public PacketId ID => PacketId.MatchResult;

    public Status MatchStatus;
    public string? OpponentName;
    public bool IsMyTurnFirst;   // 선공 여부 (1v1이라 둘 중 하나)

    public void Write(PacketWriter writer)
    {
        writer.Write((ushort)MatchStatus);
        writer.Write(OpponentName);
        writer.Write(IsMyTurnFirst);
    }

    public void Read(PacketReader reader)
    {
        MatchStatus = (Status)reader.ReadUShort();
        OpponentName = reader.ReadString();
        IsMyTurnFirst = reader.ReadBool();
    }
}