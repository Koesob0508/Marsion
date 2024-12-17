using UnityEngine;

namespace Marsion
{
    public class Managers : MonoBehaviour, IManagers
    {
        public static Managers Instance { get; private set; }

        [SerializeField] ServerManager _server;
        [SerializeField] ClientManager _client;

        private IResourceManager _resource;
        private IUIManager _ui;
        private IDataManager _data;
        private INetworkManager _network;

        public IResourceManager Resource => _resource;
        public IUIManager UI => _ui;
        public static CardManager Card { get; private set; }
        public IDataManager Data => _data;
        public INetworkManager Network => _network;
        public static ServerManager Server { get { return Instance._server; } }
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

                IResourceLoader resourceLoader = factory.CreateResourceLoader();
                IAddressableLoader addressableLoader = factory.CreateAddressableLoader();
                Instance._resource = factory.CreateResource(resourceLoader, addressableLoader);

                Instance._ui = factory.CreateUI(Instance.Resource);
                Instance._data = factory.CreateData(Instance.Resource);

                INetworkManagerWrapper networkWrapper = factory.CreateNetworkManagerWrapper();
                Instance._network = factory.CreateNetworkManager(networkWrapper);

                Card = new CardManager();

                Instance._data.Init();
                Server.Init();
                Client.Init();
            }
        }

        public static void Clear()
        {
            Instance.UI.Clear();
        }
    }
}