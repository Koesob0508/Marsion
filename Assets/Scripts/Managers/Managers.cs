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

        [SerializeField] GameServer _server;
        [SerializeField] GameClient _client;
        [SerializeField] CardManager _card;
        ResourceManager _resource = new ResourceManager();
        UIManager _ui = new UIManager();
        DeckBuilder _builder = new DeckBuilder();

        Logger _logger = new Logger();

        public static NetworkManager Network { get { return NetworkManager.Singleton; } }
        public static IGameServer Server { get { return Instance._server; } }
        public static IGameClient Client { get { return Instance._client; } }
        public static CardManager Card { get { return Instance._card; } }
        public static ResourceManager Resource { get { return Instance._resource; } }
        public static UIManager UI { get { return Instance._ui; } }
        public static DeckBuilder Builder { get { return Instance._builder; } }

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
                s_instance._builder.Init();
            }
        }

        public static void Clear()
        {
            s_instance._ui.Clear();
        }
    }
}