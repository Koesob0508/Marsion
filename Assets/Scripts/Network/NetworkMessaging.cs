using System.Collections.Generic;
using Unity.Netcode;

namespace Marsion
{
    public class NetworkMessaging
    {
        private MarsNetwork network;
        private Dictionary<string, System.Action<ulong, FastBufferReader>> messageDictionary = new Dictionary<string, System.Action<ulong, FastBufferReader>>();

        public NetworkMessaging(MarsNetwork _network)
        {
            network = _network;
            network.OnConnect += OnConnect;
        }

        private void OnConnect()
        {

        }

        private void RegisterMessage(string type, System.Action<ulong, FastBufferReader> callback)
        {
            network.NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler(type, (ulong clientID, FastBufferReader reader) =>
            {
                ReceiveMessage(type, clientID, reader);
            });
        }

        private void ReceiveMessage(string type, ulong clientID, FastBufferReader reader)
        {
            if (messageDictionary.TryGetValue(type, out System.Action<ulong, FastBufferReader> callback))
            {
                callback(clientID, reader);
            }
        }

        public void SubscribeMessage(string type, System.Action<ulong, FastBufferReader> callback)
        {
            messageDictionary[type] = callback;
            RegisterMessage(type, callback);
        }

        public void UnsubscribeMessage(string type)
        {
            messageDictionary.Remove(type);

            if (network.NetworkManager.CustomMessagingManager != null)
                network.NetworkManager.CustomMessagingManager.UnregisterNamedMessageHandler(type);
        }
    }
}
