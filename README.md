# showdown

C#으로 클라이언트/서버 게임 네트워킹을 익히는 학습 프로젝트. 직접 만든 바이너리
패킷 프로토콜을 client/server가 공유한다.

## 구조 (monorepo)

| 폴더 | 역할 | 런타임 |
|---|---|---|
| `shared/` | 와이어 프로토콜 | netstandard2.1 / C#9 |
| `server/` | 권위 서버 | .NET 10 콘솔 |
| `client/` | 게임 클라이언트 | Unity 6000.3 |

`shared`는 DLL(`Showdown.Shared.dll`)로 빌드되어 server와 client(`Assets/Plugins/`)
양쪽이 참조한다. Unity 호환을 위해 netstandard2.1 / C#9 한도를 지킨다.

## Shared

client와 server가 공유하는 와이어 프로토콜. little-endian 바이너리 직렬화와
TCP 스트림에서 패킷을 분리/조립하는 로직을 담는다.

## Server

권위 서버. TCP 연결을 받아 세션 단위로 관리하고, 들어온 패킷을 핸들러로
디스패치해 응답한다.

## Client

Unity 게임 클라이언트. 서버에 연결해 패킷을 주고받으며, 게임 로직은 갖지 않고
"표시 + 입력" 역할만 한다.

## 빌드 & 실행

```bash
dotnet build shared/shared.csproj      # DLL을 client/Assets/Plugins로 배포
dotnet run --project server/server.csproj
```

클라이언트는 Unity 에디터(6000.3.10f1)에서 실행.
