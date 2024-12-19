using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace Marsion
{
    public class CustomMessagingManagerWrapper : ICustomMessagingManager
    {
        private readonly INetworkManagerWrapper networkManager;
        private readonly Dictionary<string, List<Action<ulong, FastBufferReader>>> messageHandlers;

        public CustomMessagingManagerWrapper(INetworkManagerWrapper networkManager)
        {
            messageHandlers = new();
            this.networkManager = networkManager ?? throw new ArgumentNullException(nameof(networkManager));
            this.networkManager.OnConnect += OnConnect;
        }

        private void OnConnect()
        {
            var keys = new List<string>(messageHandlers.Keys);
            foreach (var type in keys)
            {
                RegisterMessage(type);
            }
        }

        public void SubscribeMessage(string type, Action<ulong, FastBufferReader> handler)
        {
            if (!messageHandlers.TryGetValue(type, out var handlers))
            {
                handlers = new List<Action<ulong, FastBufferReader>>();
                messageHandlers[type] = handlers;

                RegisterMessage(type);
            }

            if(!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }

        public void UnsubscribeMessage(string type, Action<ulong, FastBufferReader> handler)
        {
            if (messageHandlers.TryGetValue(type, out var handlers))
            {
                handlers.Remove(handler);

                if (handlers.Count == 0) // 더 이상 핸들러가 없으면 메시지 등록 해제
                {
                    messageHandlers.Remove(type);
                    networkManager.CustomMessagingManager.UnregisterNamedMessageHandler(type);
                }
            }
        }

        public void UnsubscribeAllMessages()
        {
            var keys = new List<string>(messageHandlers.Keys);
            foreach (var type in keys)
            {
                try
                {
                    networkManager.CustomMessagingManager.UnregisterNamedMessageHandler(type);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning<NetworkManagerEx>($"Failed to unregister message handler for {type}: {ex.Message}");

                }
            }

            messageHandlers.Clear();
        }

        private void RegisterMessage(string type)
        {
            try
            {
                if (networkManager.IsConnected)
                {
                    networkManager.CustomMessagingManager.RegisterNamedMessageHandler(type, (ulong clientID, FastBufferReader reader) =>
                    {
                        if(messageHandlers.TryGetValue(type, out var handlers))
                        {
                            foreach(var handler in handlers)
                            {
                                handler?.Invoke(clientID, reader);
                            }
                        }
                    });
                }
            }
            catch(Exception ex)
            {
                Logger.LogError<NetworkManagerEx>($"Failed to register message handler for {type}: {ex.Message}");
            }
        }
    }
}