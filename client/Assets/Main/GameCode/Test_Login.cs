using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class Test_Login : MonoBehaviour
{
    private NetworkManager _network;
    private string _userName = "tester";
    private string _lastMessage = "";

    private void Awake()
    {
        _network = GetComponent<NetworkManager>();
        _network.Dispatcher.RegisterEvent<LoginResultPacket>(PacketId.LoginResult, LoginResultCallback);
        _network.Dispatcher.RegisterEvent<MatchResultPacket>(PacketId.MatchResult, MatchFoundCallback);
    }

    private void LoginResultCallback(LoginResultPacket packet)
    {
        // 로그인 성공 시 매칭 대기열에 들어가므로 "대기중" 상태로 표시.
        // (서버가 별도 대기 패킷을 보내지 않으니 현재는 로컬에서 추정)
        _lastMessage = packet.Success
            ? $"{packet.Message}\n상대를 기다리는 중..."
            : $"로그인 실패: {packet.Message}";
    }

    private void MatchFoundCallback(MatchResultPacket packet)
    {
        if (packet.MatchStatus == Status.Success)
            _lastMessage = $"매칭됨! 상대={packet.OpponentName} / 선공={(packet.IsMyTurnFirst ? "나" : "상대")}";
        else
            _lastMessage = "대기중...";
    }


    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));

        if (!_network.IsConnected)
        {
            if (GUILayout.Button("서버 접속"))
                _network.Connect();
        }
        else
        {
            _userName = GUILayout.TextField(_userName);
            if (GUILayout.Button("로그인"))
                _network.Send(new LoginPacket { UserName = _userName, Password = "pw" });
        }

        GUILayout.Label(_lastMessage);
        GUILayout.EndArea();
    }
}