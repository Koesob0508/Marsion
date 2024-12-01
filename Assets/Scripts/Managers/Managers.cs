using UnityEngine;

namespace Marsion
{
    [DefaultExecutionOrder(-10)]
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

        UIUtility _ui = new UIUtility();
        ResourceUtility _resource = new ResourceUtility();
        CardManager _card = new CardManager();

        IDataManager _data = new DataManager();
        [SerializeField] MarsNetwork _network;
        [SerializeField] ServerManager _server;
        [SerializeField] ClientManager _client;

        public static UIUtility UI { get { return Instance._ui; } }
        public static ResourceUtility Resource { get { return Instance._resource; } }
        public static CardManager Card { get { return Instance._card; } }

        public static IDataManager Data { get { return Instance._data; } }
        public static MarsNetwork Network { get { return Instance._network; } }
        public static ServerManager Server { get { return Instance._server; } }
        public static ClientManager Client { get { return Instance._client; } }
        

        private void Start()
        {
            Init();
        }

        public static void Init()
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


                Logger.Log<Managers>("Managers initialized", colorName: ColorCodes.Managers);
                
                s_instance._data.Init();
                s_instance._network.Init();
                s_instance._server.Init();
                s_instance._client.Init();
            }
        }

        public static void Clear()
        {
            s_instance._ui.Clear();
        }
    }
}