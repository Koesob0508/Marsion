using Marsion.Client;
using Marsion.Server;
using Unity.Netcode;
using UnityEngine;

namespace Marsion
{
    public class Managers : NetworkBehaviour
    {
        private static Managers s_instance;
        public static Managers Instance { get { Init(); return s_instance; } }

        private ServerManager _server = new ServerManager();
        private ClientManager _client = new ClientManager();

        private Logger _logger = new Logger();

        public static NetworkManager Network { get { return NetworkManager.Singleton; } }

        public static ServerManager Server { get { return Instance._server; } }
        public static ClientManager Client { get { return Instance._client; } }


        public static Logger Logger { get { return Instance._logger; } }

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            Client.Update();
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

                Server.Init();
                Client.Init();
            }
        }

        public static void Clear()
        {

        }
    }
}