using Marsion.Client;
using Marsion.Server;
using Unity.Netcode;
using UnityEngine;

namespace Marsion
{
    public class Managers : MonoBehaviour
    {
        static Managers s_instance;
        public static Managers Instance
        {
            get
            {
                Init();
                return s_instance;
            }
        }

        [SerializeField] MarsNetwork _network;
        [SerializeField] GameServer _server;
        [SerializeField] GameClient _client;

        ResourceManager _resource = new ResourceManager();
        DataManager _data = new DataManager();
        UIManager _ui = new UIManager();
        CardManager _card = new CardManager();

        Logger _logger = new Logger();

        public static MarsNetwork Network { get { return Instance._network; } }
        public static IGameServer Server { get { return Instance._server; } }
        public static IGameClient Client { get { return Instance._client; } }
        public static CardManager Card { get { return Instance._card; } }
        public static ResourceManager Resource { get { return Instance._resource; } }
        public static DataManager Data { get { return Instance._data; } }
        public static UIManager UI { get { return Instance._ui; } }

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

                s_instance._network.Init();
                s_instance._server.Init();
                s_instance._client.Init();
                s_instance._data.Init();
            }
        }

        public static void Clear()
        {
            s_instance._ui.Clear();
        }
    }
}