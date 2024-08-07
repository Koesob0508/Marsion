using Marsion.Client;
using Marsion.Server;
using Unity.Netcode;
using UnityEngine;

namespace Marsion
{
    public class Managers : MonoBehaviour
    {
        static Managers s_instance;
        public static Managers Instance { get { Init(); return s_instance; } }

        [SerializeField] ServerManager _server;
        [SerializeField] ClientManager _client;

        Logger _logger = new Logger();

        public static NetworkManager Network { get { return NetworkManager.Singleton; } }

        public static ServerManager Server { get { return Instance._server; } }
        public static ClientManager Client { get { return Instance._client; } }


        public static Logger Logger { get { return Instance._logger; } }

        private void Start()
        {
            Init();
        }

        private static void Init()
        {
            if (s_instance == null)
            {
                GameObject obj = GameObject.Find("@Managers");

                if (obj == null)
                {
                    obj = new GameObject { name = "@Managers" };
                    obj.AddComponent<Managers>();
                }

                DontDestroyOnLoad(obj);
                s_instance = obj.GetComponent<Managers>();

                s_instance._server.Init();
                s_instance._client.Init();
            }
        }

        public static void Clear()
        {

        }
    }
}