using UnityEngine;

namespace Marsion
{
    public class Managers : MonoBehaviour, IManagers
    {
        public static Managers Instance { get; private set; }

        private IResourceManager _resource;
        private IUIManager _ui;
        private IDataManager _data;
        private INetworkManagerEx _network;
        private IServerManager _server;
        private IClientManager _client;

        public IResourceManager Resource => Instance._resource;
        public IUIManager UI => Instance._ui;
        public static CardManager Card { get; private set; }
        public IDataManager Data => Instance._data;
        public INetworkManagerEx Network => Instance._network;
        public IServerManager Server => Instance._server;
        public IClientManager Client => Instance._client;

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
                Instance._client = factory.CreateClient();

                Instance._data.Init();

                var serverFactory = factory.CreateServerFactory(Instance);
                Instance._server.Init(serverFactory);
                Instance._client.Init();
            }
        }

        public static void Clear()
        {
            Instance.UI.Clear();
        }
    }
}