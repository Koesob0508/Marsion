using System.Collections.Generic;
using Unity.Netcode;
using Unity.Collections;
using System;

namespace Marsion
{
    public class NetworkMessaging
    {
        private MarsNetwork network;
        private Dictionary<string, System.Action<ulong, FastBufferReader>> messageDictionary = new Dictionary<string, System.Action<ulong, FastBufferReader>>();
        public bool IsOnline { get { return network.IsOnline; } }
        public bool IsServer { get { return network.IsServer; } }
        public ulong ServerID { get { return network.ServerID; } }
        public ulong ClientID { get { return network.ClientID; } }
        public IReadOnlyList<ulong> ClientList { get { return network.GetClientIDs(); } }

        public NetworkMessaging Get()
        {
            return network.Get().Messaging;
        }

        public NetworkMessaging(MarsNetwork _network)
        {
            network = _network;
            network.OnConnect += OnConnect;
        }

        private void OnConnect()
        {
            foreach (KeyValuePair<string, System.Action<ulong, FastBufferReader>> pair in messageDictionary)
            {
                RegistMessage(pair.Key, pair.Value);
            }
        }

        private void RegistMessage(string type, System.Action<ulong, FastBufferReader> callback)
        {
            if (!IsOnline) return;

            network.NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler(type, (ulong clientID, FastBufferReader reader) =>
            {
                ReceiveMessage(type, clientID, reader);
            });
        }

        private void ReceiveMessage(string type, ulong clientID, FastBufferReader reader)
        {
            if (messageDictionary.TryGetValue(type, out Action<ulong, FastBufferReader> callback))
            {
                if (IsOnline)
                {
                    callback(clientID, reader);
                }
            }
        }

        public void SubscribeMessage(string type, Action<ulong, FastBufferReader> callback)
        {
            messageDictionary[type] = callback;
            RegistMessage(type, callback);
        }

        public void UnsubscribeMessage(string type)
        {
            messageDictionary.Remove(type);

            if (network.NetworkManager.CustomMessagingManager != null)
                network.NetworkManager.CustomMessagingManager.UnregisterNamedMessageHandler(type);
        }

        // Generic Send
        public void Send(string type, ulong target, FastBufferWriter writer, NetworkDelivery delivery)
        {
            if (IsOnline)
            {
                SendOnline(type, target, writer, delivery);
            }
            else if (target == ClientID)
            {
                SendOffline(type, writer);
            }
        }

        public void Send(string type, IReadOnlyList<ulong> targets, FastBufferWriter writer, NetworkDelivery delivery)
        {
            if (IsOnline)
                SendOnline(type, targets, writer, delivery);
            else if (Contains(targets, ClientID))
                SendOffline(type, writer);
        }

        public void SendAll(string type, FastBufferWriter writer, NetworkDelivery delivery)
        {
            Send(type, ClientList, writer, delivery);
        }

        private bool Contains(IReadOnlyList<ulong> list, ulong clientID)
        {
            foreach(ulong id in list)
            {
                if (id == clientID)
                    return true;
            }

            return false;
        }

        private void SendOnline(string type, ulong target, FastBufferWriter writer, NetworkDelivery delivery)
        {
            network.NetworkManager.CustomMessagingManager.SendNamedMessage(type, target, writer, delivery);
        }

        private void SendOnline(string type, IReadOnlyList<ulong> targets, FastBufferWriter writer, NetworkDelivery delivery)
        {
            network.NetworkManager.CustomMessagingManager.SendNamedMessage(type, targets, writer, delivery);
        }

        private void SendOffline(string type, FastBufferWriter writer)
        {
            bool found = messageDictionary.TryGetValue(type, out Action<ulong, FastBufferReader> callback);
            if (found)
            {
                FastBufferReader reader = new FastBufferReader(writer, Allocator.Temp);
                callback?.Invoke(ClientID, reader);
                reader.Dispose();
            }
        }

        // Send Single

        public void SendEmpty(string type, ulong target, NetworkDelivery delivery)
        {
            FastBufferWriter writer = new FastBufferWriter(0, Allocator.Temp);
            Send(type, target, writer, delivery);
            writer.Dispose();
        }

        public void SendBytes(string type, ulong target, byte[] message, NetworkDelivery delivery)
        {
            FastBufferWriter writer = new FastBufferWriter(message.Length, Allocator.Temp);
            writer.WriteBytesSafe(message, message.Length);
            Send(type, target, writer, delivery);
            writer.Dispose();
        }

        public void SendString(string type, ulong target, string message, NetworkDelivery delivery)
        {
            FastBufferWriter writer = new FastBufferWriter(message.Length, Allocator.Temp, MarsNetwork.MessageSizeMax);
            writer.WriteValueSafe(message);
            Send(type, target, writer, delivery);
            writer.Dispose();
        }

        public void SendInt(string type, ulong target, int message, NetworkDelivery delivery)
        {
            FastBufferWriter writer = new FastBufferWriter(4, Allocator.Temp);
            writer.WriteValueSafe(message);
            Send(type, target, writer, delivery);
            writer.Dispose();
        }

        public void SendUInt64(string type, ulong target, ulong message, NetworkDelivery delivery)
        {
            FastBufferWriter writer = new FastBufferWriter(8, Allocator.Temp);
            writer.WriteValueSafe(message);
            Send(type, target, writer, delivery);
            writer.Dispose();
        }

        public void SendFloat(string type, ulong target, float message, NetworkDelivery delivery)
        {
            FastBufferWriter writer = new FastBufferWriter(4, Allocator.Temp);
            writer.WriteValueSafe(message);
            Send(type, target, writer, delivery);
            writer.Dispose();
        }

        public void SendObject<T>(string type, ulong target, T message, NetworkDelivery delivery) where T : INetworkSerializable
        {
            FastBufferWriter writer = new FastBufferWriter(256, Allocator.Temp, MarsNetwork.MessageSizeMax);
            writer.WriteNetworkSerializable(message);
            Send(type, target, writer, delivery);
            writer.Dispose();
        }

        // Send Multi
        public void SendEmpty(string type, IReadOnlyList<ulong> targets, NetworkDelivery delivery)
        {
            if(IsServer)
            {
                FastBufferWriter writer = new FastBufferWriter(0, Allocator.Temp);
                Send(type, targets, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendBytes(string type, IReadOnlyList<ulong> targets, byte[] message, NetworkDelivery delivery)
        {
            if (IsServer)
            {
                FastBufferWriter writer = new FastBufferWriter(message.Length, Allocator.Temp);
                writer.WriteBytesSafe(message, message.Length);
                Send(type, targets, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendString(string type, IReadOnlyList<ulong> targets, string message, NetworkDelivery delivery)
        {
            if (IsServer)
            {
                FastBufferWriter writer = new FastBufferWriter(message.Length, Allocator.Temp, MarsNetwork.MessageSizeMax);
                writer.WriteValueSafe(message);
                Send(type, targets, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendInt(string type, IReadOnlyList<ulong> targets, int message, NetworkDelivery delivery)
        {
            if (IsServer)
            {
                FastBufferWriter writer = new FastBufferWriter(4, Allocator.Temp);
                writer.WriteValueSafe(message);
                Send(type, targets, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendUInt64(string type, IReadOnlyList<ulong> targets, ulong message, NetworkDelivery delivery)
        {
            if (IsServer)
            {
                FastBufferWriter writer = new FastBufferWriter(8, Allocator.Temp);
                writer.WriteValueSafe(message);
                Send(type, targets, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendFloat(string type, IReadOnlyList<ulong> targets, float message, NetworkDelivery delivery)
        {
            if (IsServer)
            {
                FastBufferWriter writer = new FastBufferWriter(4, Allocator.Temp);
                writer.WriteValueSafe(message);
                Send(type, targets, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendObject<T>(string type, IReadOnlyList<ulong> targets, T message, NetworkDelivery delivery) where T : INetworkSerializable
        {
            if (IsServer)
            {
                FastBufferWriter writer = new FastBufferWriter(256, Allocator.Temp, MarsNetwork.MessageSizeMax);
                writer.WriteNetworkSerializable(message);
                Send(type, targets, writer, delivery);
                writer.Dispose();
            }
        }

        // Send All
        public void SendEmptyAll(string type, NetworkDelivery delivery)
        {
            if (IsServer)
            {
                FastBufferWriter writer = new FastBufferWriter(0, Allocator.Temp);
                SendAll(type, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendBytesAll(string type, byte[] message, NetworkDelivery delivery)
        {
            if (IsServer)
            {
                FastBufferWriter writer = new FastBufferWriter(message.Length, Allocator.Temp);
                writer.WriteBytesSafe(message, message.Length);
                SendAll(type, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendStringAll(string type, string message, NetworkDelivery delivery)
        {
            if (IsServer)
            {
                FastBufferWriter writer = new FastBufferWriter(message.Length, Allocator.Temp, MarsNetwork.MessageSizeMax);
                writer.WriteValueSafe(message);
                SendAll(type, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendIntAll(string type, int message, NetworkDelivery delivery)
        {
            if (IsServer)
            {
                FastBufferWriter writer = new FastBufferWriter(4, Allocator.Temp);
                writer.WriteValueSafe(message);
                SendAll(type, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendUInt64All(string type, ulong message, NetworkDelivery delivery)
        {
            if (IsServer)
            {
                FastBufferWriter writer = new FastBufferWriter(8, Allocator.Temp);
                writer.WriteValueSafe(message);
                SendAll(type, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendFloatAll(string type, float message, NetworkDelivery delivery)
        {
            if (IsServer)
            {
                FastBufferWriter writer = new FastBufferWriter(4, Allocator.Temp);
                writer.WriteValueSafe(message);
                SendAll(type, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendObjectAll<T>(string type, T message, NetworkDelivery delivery) where T : INetworkSerializable
        {
            if (IsServer)
            {
                FastBufferWriter writer = new FastBufferWriter(256, Allocator.Temp, MarsNetwork.MessageSizeMax);
                writer.WriteNetworkSerializable(message);
                SendAll(type, writer, delivery);
                writer.Dispose();
            }
        }
    }
}
