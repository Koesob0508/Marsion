using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace Marsion
{
    public class CustomMessagingManagerWrapper : ICustomMessagingManager
    {
        private readonly CustomMessagingManager messagingManager;

        public CustomMessagingManagerWrapper(CustomMessagingManager messagingManager)
        {
            this.messagingManager = messagingManager ?? throw new ArgumentNullException(nameof(messagingManager));
        }

        public void RegisterNamedMessageHandler(string name, Action<ulong, FastBufferReader> handler)
        {
            // 래퍼를 통해 Action을 HandleNamedMessageDelegate로 변환
            messagingManager.RegisterNamedMessageHandler(name, (senderClientId, payload) =>
            {
                handler?.Invoke(senderClientId, payload);
            });
        }

        public void UnregisterNamedMessageHandler(string name)
        {
            messagingManager.UnregisterNamedMessageHandler(name);
        }

        public void SendMessage(string name, ulong targetClientId, FastBufferWriter writer, NetworkDelivery delivery)
        {
            messagingManager.SendNamedMessage(name, targetClientId, writer, delivery);
        }

        public void SendMessage(string name, IReadOnlyList<ulong> targetClientIds, FastBufferWriter writer, NetworkDelivery delivery)
        {
            messagingManager.SendNamedMessage(name, targetClientIds, writer, delivery);
        }
    }
}