namespace Marsion
{
    public interface IManagerFactory
    {
        IResourceLoader CreateResourceLoader();
        IResourceManager CreateResource(IResourceLoader loader);
        IUIManager CreateUI(IResourceManager resource);
        CardManager CreateCard();
        DataManager CreateData();
        MarsNetwork CreateNetwork();
        ServerManager CreateServer();
        ClientManager CreateClient();
    }

    public class DefaultManagerFactory : IManagerFactory
    {
        public IResourceLoader CreateResourceLoader() => new DefaultResourceLoader();
        public IResourceManager CreateResource(IResourceLoader loader) => new ResourceManager(loader);
        public IUIManager CreateUI(IResourceManager resource) => new UIManager(resource);
        public CardManager CreateCard() => new CardManager();

        public DataManager CreateData() => new DataManager();

        public MarsNetwork CreateNetwork() => new MarsNetwork();

        public ServerManager CreateServer() => new ServerManager();

        public ClientManager CreateClient() => new ClientManager();
    }
}