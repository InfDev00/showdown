using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// 서버와의 TCP 연결을 관리한다.
// 수신은 백그라운드 스레드(ReceiveLoopAsync)에서 돌고, 실제 패킷 처리는
// 메인 스레드(Update)에서 한다 — Unity API는 메인 스레드에서만 호출 가능하기 때문.
public class NetworkManager : MonoBehaviour
{
    [SerializeField] private string _host = "127.0.0.1";
    [SerializeField] private int _port = 8080;

    private TcpClient _client;
    private NetworkStream _stream;
    private CancellationTokenSource _cts;   // 연결 종료 시 수신 루프를 취소시키는 데 사용
    private PacketChannel _channel;          // 프레이밍 수신 루프 + 송신 (shared)

    // 수신 스레드 → 메인 스레드로 패킷을 넘기는 다리. 양쪽 스레드가 접근하므로 ConcurrentQueue.
    private readonly ConcurrentQueue<byte[]> _recvQueue = new ConcurrentQueue<byte[]>();
    public event Action<LoginResultPacket> OnLoginResult;
    public bool IsConnected => _client != null && _client.Connected;

    public async void Connect()
    {
        try
        {
            _client = new TcpClient();
            await _client.ConnectAsync(_host, _port);
            _stream = _client.GetStream();
            _cts = new CancellationTokenSource();

            _channel = new PacketChannel(_stream, OnPacketReceived);

            _recvQueue.Enqueue(null);        // null = "접속됨" 신호. 메인 스레드(Update)에서 로그를 찍기 위함
            _ = ReceiveLoopAsync(_cts.Token); // 수신 루프는 백그라운드에서 돌린다 (await 하지 않음)
        }
        catch (Exception e)
        {
            Debug.LogError($"접속 실패: {e.Message}");
        }
    }

    public async void Send(IPacket packet)
    {
        if (!IsConnected) return;
        await _channel.SendAsync(packet);
    }

    // 채널이 패킷을 주면 메인 스레드 큐에 적재 (여기서도 Unity API 호출 금지)
    private Task OnPacketReceived(byte[] data)
    {
        _recvQueue.Enqueue(data);
        return Task.CompletedTask;
    }

    private async Task ReceiveLoopAsync(CancellationToken token)
    {
        try
        {
            await _channel.RunAsync(token);   // 연결이 끊길 때까지 백그라운드에서 패킷을 받아 큐에 쌓는다
        }
        catch (Exception) when (token.IsCancellationRequested)
        {
            // 우리가 끊은 것 — 무시. 그 외 예외는 그대로 드러나게 둔다
        }
    }

    // 메인 스레드: 매 프레임 큐에 쌓인 패킷을 꺼내 처리한다 (Unity API 호출은 여기서만)
    private void Update()
    {
        while (_recvQueue.TryDequeue(out var data))
        {
            if (data == null)
            {
                Debug.Log("접속 성공");
                continue;
            }
            HandlePacket(data);
        }
    }

    // Play 정지/오브젝트 파괴 시 정리.
    // _client.Close()가 서버로 FIN을 보내야 서버의 ReadAsync가 0을 반환해 접속 종료를 감지한다.
    private void OnDestroy()
    {
        _cts?.Cancel();   // 수신 루프 취소
        _client?.Close(); // 소켓 닫기 → 서버에 연결 종료 통지
    }

    // 패킷 id로 종류를 구분해 해당 처리를 한다 (PeekId → Deserialize<T>)
    private void HandlePacket(byte[] data)
    {
        var id = Serializer.PeekId(data, 0, data.Length);
        switch (id)
        {
            case PacketId.LoginResult:
                {
                    var result = Serializer.Deserialize<LoginResultPacket>(data, 0, data.Length);
                    OnLoginResult?.Invoke(result);
                }
                break;

            default:
                Debug.LogError($"처리되지 않는 패킷: {id}");
                break;
        }
    }
}