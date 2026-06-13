using System;
using System.Collections.Generic;
using UnityEngine;

public interface IHandler
{
    PacketId Id { get; }
    void Handle(byte[] data);
}

public class PacketHandler<T> : IHandler where T : IPacket, new()
{
    public PacketId Id => new T().ID;
    public event Action<T> OnPacket;

    public void Handle(byte[] data)
    {
        var packet = Serializer.Deserialize<T>(data, 0, data.Length);
        OnPacket?.Invoke(packet);
    }
}

public class PacketDispatcher
{
    private readonly Dictionary<PacketId, IHandler> _handlers = new Dictionary<PacketId, IHandler>();

    public PacketDispatcher()
    {
        AddHandler<LoginResultPacket>();
        AddHandler<MatchResultPacket>();
    }

    private void AddHandler<T>() where T : IPacket, new()
    {
        var handler = new PacketHandler<T>();
        _handlers[handler.Id] = handler;
    }

    public void RegisterEvent<T>(PacketId id, Action<T> callback) where T : IPacket, new()
    {
        if (_handlers.TryGetValue(id, out var handler) && handler is PacketHandler<T> packetHandler)
        {
            packetHandler.OnPacket += callback;
        }
    }

    public void UnregisterEvent<T>(PacketId id, Action<T> callback) where T : IPacket, new()
    {
        if (_handlers.TryGetValue(id, out var handler) && handler is PacketHandler<T> packetHandler)
        {
            packetHandler.OnPacket -= callback;
        }
    }

    public void Dispatch(byte[] data)
    {
        var id = Serializer.PeekId(data, 0, data.Length);
        if (_handlers.TryGetValue(id, out var handler))
        {
            handler.Handle(data);
        }
        else
        {
            Debug.LogError($"처리되지 않는 패킷: {id}");
        }
    }
}