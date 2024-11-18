using Unity.Netcode;
using UnityEngine;

namespace Practice
{
    public class NetworkGameManager : NetworkBehaviour
    {
        public static NetworkGameManager Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Host started");
        }

        public void StartClient()
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client started");
        }

        public void Disconnect()
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Disconnected");
        }
    }

}