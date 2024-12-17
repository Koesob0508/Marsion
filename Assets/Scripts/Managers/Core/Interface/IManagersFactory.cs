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
        NetworkManager CreateNetworkManager();
        INetworkManagerWrapper CreateNetworkManagerWrapper(NetworkManager networkManager);
        INetworkManagerWrapper CreateNetworkManagerWrapper();
        INetworkManager CreateNetworkManager(INetworkManagerWrapper networkManagerWrapper);
    }

    public class DefaultManagersFactory : IManagersFactory
    {
        public IResourceLoader CreateResourceLoader() => new DefaultResourceLoader();
        public IAddressableLoader CreateAddressableLoader() => new DefaultAddressableLoader();
        public IResourceManager CreateResource(IResourceLoader resourceLoader, IAddressableLoader addressableLoader) => new ResourceManager(resourceLoader, addressableLoader);
        public IUIManager CreateUI(IResourceManager resourceManager) => new UIManager(resourceManager);
        public IDataManager CreateData(IResourceManager resourceManager) => new DataManager(resourceManager);
        public NetworkManager CreateNetworkManager()
        {
            var existingManager = UnityEngine.Object.FindAnyObjectByType<NetworkManager>();
            if (existingManager == null)
            {
                throw new InvalidOperationException("NetworkManager is not found in the scene. " +
                    "Ensure a NetworkManager is pre-placed in the scene or properly configured in the test environment.");
            }
            return existingManager;
        }
        public INetworkManagerWrapper CreateNetworkManagerWrapper(NetworkManager networkManager)
        {
            return new NetworkManagerWrapper(networkManager);
        }
        public INetworkManagerWrapper CreateNetworkManagerWrapper()
        {
            var networkManager = CreateNetworkManager();

            if (networkManager == null)
            {
                throw new InvalidOperationException("NetworkManager is not found in the scene. " +
                    "Ensure a NetworkManager is pre-placed in the scene or properly configured in the test environment.");
            }

            return CreateNetworkManagerWrapper(networkManager);
        }
        public INetworkManager CreateNetworkManager(INetworkManagerWrapper networkManagerWrapper)
        {
            return new NetworkManagerEx(networkManagerWrapper);
        }
    }
}