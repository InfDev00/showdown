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
        _network.OnLoginResult += result =>
        {
            _lastMessage = $"성공={result.Success}, 메시지={result.Message}";
        };
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