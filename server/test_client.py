import socket, struct

def write_string(s: str) -> bytes:
    b = s.encode("utf-8")
    return struct.pack("<H", len(b)) + b   # <H = ushort LE

# payload: Username, Password
payload = write_string("tester") + write_string("pw")
# frame: [short total][short id=1(Login)] + payload
frame = struct.pack("<hh", 4 + len(payload), 1) + payload   # <h = short LE

s = socket.create_connection(("127.0.0.1", 8080))
s.sendall(frame)

# 응답(LoginResultPacket) 수신: [short total][short id=2][bool success][string message]
resp = s.recv(1024)
total, pid = struct.unpack_from("<hh", resp, 0)
success = resp[4] != 0
msg_len = struct.unpack_from("<H", resp, 5)[0]
msg = resp[7:7+msg_len].decode("utf-8")
print(f"id={pid} success={success} message={msg}")
s.close()