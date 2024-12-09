using Marsion.Client;
using Marsion.Server;
using Unity.Netcode;
using UnityEngine;

namespace Marsion
{
    public class Managers : MonoBehaviour
    {
        public static Managers Instance { get; private set; }

        [SerializeField] MarsNetwork _network;
        [SerializeField] ServerManager _server;
        [SerializeField] ClientManager _client;

        public IResourceManager Resource { get; private set; }
        public static UIUtility UI { get; private set; }
        public static CardManager Card { get; private set; }

        public static DataManager Data { get; private set; }
        public static MarsNetwork Network { get { return Instance._network; } }
        public static ServerManager Server { get { return Instance._server; } }
        public static ClientManager Client { get { return Instance._client; } }

        public static void Init(IManagerFactory factory)
        {
            if (Instance == null)
            {
                var obj = GameObject.Find("@Managers");
                if (obj == null)
                {
                    obj = new GameObject { name = "@Managers" };
                    obj.AddComponent<Managers>();
                }
                DontDestroyOnLoad(obj);
                Instance = obj.GetComponent<Managers>();

                Logger.Log<Managers>("Managers initialized", colorName: ColorCodes.Managers);

                IResourceLoader resourceLoader = factory.CreateResourceLoader();
                IAddressableLoader addressableLoader = factory.CreateAddressableLoader();
                Instance.Resource = factory.CreateResource(resourceLoader, addressableLoader);

                UI = new UIUtility();
                Card = new CardManager();
                Data = new DataManager();

                Data.Init();
                Network.Init();
                Server.Init();
                Client.Init();
            }
        }

        public static void Clear()
        {
            UI.Clear();
        }
    }
}