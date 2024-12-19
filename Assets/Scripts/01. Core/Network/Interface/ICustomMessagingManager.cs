using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace Marsion
{
    public interface ICustomMessagingManager
    {
        void SubscribeMessage(string type, Action<ulong, FastBufferReader> handler);
        void UnsubscribeMessage(string type, Action<ulong, FastBufferReader> handler);
        void UnsubscribeAllMessages();
    }
}