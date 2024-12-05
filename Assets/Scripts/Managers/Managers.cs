using System.Collections.Generic;
using UnityEngine;

namespace Marsion
{
    public class Managers : MonoBehaviour
    {
        public static Managers Instance { get; private set; }
        public IUIManager UI { get; private set; }
        public IResourceManager Resource { get; private set; }
        public CardManager Card { get; private set; }

        public DataManager Data { get; private set; }
        public MarsNetwork Network { get; private set; }
        public ServerManager Server { get; private set; }
        public ClientManager Client { get; private set; }

        public static void Init(IManagerFactory factory)
        {
            if (Instance == null)
            {
                var obj = new GameObject { name = "@Managers" };
                DontDestroyOnLoad(obj);
                Instance = obj.AddComponent<Managers>();

                IResourceLoader loader = factory.CreateResourceLoader();
                Instance.Resource = factory.CreateResource(loader);
                Instance.UI = factory.CreateUI(Instance.Resource);
                Instance.Card = factory.CreateCard();

                Instance.Data = factory.CreateData();
                Instance.Network = factory.CreateNetwork();
                Instance.Server = factory.CreateServer();
                Instance.Client = factory.CreateClient();

                Instance.InitSubManagers();
            }
        }

        private void InitSubManagers()
        {
            Logger.Log<Managers>("Managers initialized", colorName: ColorCodes.Managers);
            Data?.Init();
            Network?.Init();
            Server?.Init();
            Client?.Init();
        }

        public static void Clear()
        {
            Instance?.UI?.Clear();
        }
    }

    /*
     * 2024.12.02.
     * 기존 Managers는 다른 Manager에 대해 강한 결합을 갖고 있었음
     * 따라서, 의존성 주입을 사용하여 외부에서 Manager를 초기화하도록 변경
     * 이를 통해 Mock을 하는 것이 쉽게 됨
     * 이 때, Factory 패턴을 사용하여 의존성을 생성하는 것 또한 분리
     * 그에 따라 다음과 같은 형태로 책임이 분리 됨
     * 
     * ManagerFactory : 의존성(여러 Manager) 생성 책임
     * Initializer : 초기화 흐름 제어 책임
     * Managers : 생성된 의존성 관리 책임
     */
}