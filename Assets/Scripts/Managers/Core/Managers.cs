using UnityEngine;

namespace Marsion
{
    public class Managers : MonoBehaviour, IManagers
    {
        public static Managers Instance { get; private set; }

        public ClientManager _client;

        private IResourceManager _resource;
        private IUIManager _ui;
        private IDataManager _data;
        private INetworkManagerEx _network;
        private IServerManager _server;

        public IResourceManager Resource => _resource;
        public IUIManager UI => _ui;
        public static CardManager Card { get; private set; }
        public IDataManager Data => _data;
        public INetworkManagerEx Network => _network;
        public IServerManager Server => _server;
        public static ClientManager Client { get { return Instance._client; } }

        public static void Init(IManagersFactory factory)
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

                var resourceLoader = factory.CreateResourceLoader();
                var addressableLoader = factory.CreateAddressableLoader();
                Instance._resource = factory.CreateResource(resourceLoader, addressableLoader);

                Instance._ui = factory.CreateUI(Instance.Resource);
                Instance._data = factory.CreateData(Instance.Resource);

                var networkWrapper = factory.CreateNetworkWrapper();
                Instance._network = factory.CreateNetworkEx(networkWrapper);

                Card = new CardManager();

                Instance._server = factory.CreateServer();

                Instance._data.Init();

                var serverFactory = factory.CreateServerFactory();
                Instance._server.Init(Instance.Network, serverFactory);
                
                Client.Init();
            }
        }

        public static void Clear()
        {
            Instance.UI.Clear();
        }
    }
}