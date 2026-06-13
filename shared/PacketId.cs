public enum PacketId : ushort
{
    None = 0,
    Login = 1,
    LoginResult = 2,
    MatchResult = 3,
}

public enum Status : ushort
{
    None = 0,
    Waiting = 1,
    Success = 2,
}