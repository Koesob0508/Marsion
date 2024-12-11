using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace Marsion
{
    public interface ICustomMessagingManager
    {
        void RegisterNamedMessageHandler(string name, Action<ulong, FastBufferReader> handler);
        void UnregisterNamedMessageHandler(string name);
        void SendMessage(string name, ulong clientId, FastBufferWriter writer, NetworkDelivery delivery);
        void SendMessage(string messageName, IReadOnlyList<ulong> clientIds, FastBufferWriter writer, NetworkDelivery delivery);
    }
}