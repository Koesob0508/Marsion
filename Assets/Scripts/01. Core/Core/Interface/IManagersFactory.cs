using System;
using Unity.Netcode;

namespace Marsion
{
    public interface IManagersFactory
    {
        IResourceLoader CreateResourceLoader();
        IAddressableLoader CreateAddressableLoader();
        IResourceManager CreateResource(IResourceLoader resourceLoader, IAddressableLoader addressableLoader);
        IUIManager CreateUI(IResourceManager resourceManager);
        IDataManager CreateData(IResourceManager resourceManager);
        NetworkManager CreateNetwork();
        INetworkManagerWrapper CreateNetworkWrapper(NetworkManager networkManager);
        INetworkManagerWrapper CreateNetworkWrapper();
        INetworkManagerEx CreateNetworkEx(INetworkManagerWrapper networkManagerWrapper);
        IServerManager CreateServer();
        IServerManagerFactory CreateServerFactory(IManagers managers);
        IClientManager CreateClient();
    }

    public class DefaultManagersFactory : IManagersFactory
    {
        public IResourceLoader CreateResourceLoader() => new DefaultResourceLoader();
        public IAddressableLoader CreateAddressableLoader() => new DefaultAddressableLoader();
        public IResourceManager CreateResource(IResourceLoader resourceLoader, IAddressableLoader addressableLoader) => new ResourceManager(resourceLoader, addressableLoader);
        public IUIManager CreateUI(IResourceManager resourceManager) => new UIManager(resourceManager);
        public IDataManager CreateData(IResourceManager resourceManager) => new DataManager(resourceManager);
        public NetworkManager CreateNetwork()
        {
            var existingManager = UnityEngine.Object.FindAnyObjectByType<NetworkManager>();
            if (existingManager == null)
            {
                throw new InvalidOperationException("NetworkManager is not found in the scene. " +
                    "Ensure a NetworkManager is pre-placed in the scene or properly configured in the environment.");
            }
            return existingManager;
        }
        public INetworkManagerWrapper CreateNetworkWrapper(NetworkManager networkManager) => new NetworkManagerWrapper(networkManager);
        public INetworkManagerWrapper CreateNetworkWrapper()
        {
            var networkManager = CreateNetwork();

            if (networkManager == null)
            {
                throw new InvalidOperationException("NetworkManager is not found in the scene. " +
                    "Ensure a NetworkManager is pre-placed in the scene or properly configured in the nvironment.");
            }

            return CreateNetworkWrapper(networkManager);
        }
        public INetworkManagerEx CreateNetworkEx(INetworkManagerWrapper networkManagerWrapper) => new NetworkManagerEx(networkManagerWrapper);
        public IServerManager CreateServer()
        {
            var existingManager = UnityEngine.Object.FindAnyObjectByType<ServerManager>();
            if (existingManager == null)
            {
                throw new InvalidOperationException("ServerManager is not found in the scene. " +
                    "Ensure a ServerManager is pre-placed in the scene or properly configured in environment.");
            }
            return existingManager;
        }
        public IServerManagerFactory CreateServerFactory(IManagers managers) => new DefaultServerFactory(managers);
        public IClientManager CreateClient()
        {
            var existingManager = UnityEngine.Object.FindAnyObjectByType<ClientManager>();
            if(existingManager == null)
            {
                throw new InvalidOperationException("ClientManager is not found in the scene. " +
                    "Ensure a ClientManager is pre-placed in the scene or properly configured in environment.");
            }
            return existingManager;
        }
    }
}