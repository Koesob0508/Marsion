namespace Marsion
{
    public interface IManagerFactory
    {
        IResourceLoader CreateResourceLoader();
        IAddressableLoader CreateAddressableLoader();
        IResourceManager CreateResource(IResourceLoader resourceLoader, IAddressableLoader addressableLoader);
    }

    public class DefaultManagerFactory : IManagerFactory
    {
        public IResourceLoader CreateResourceLoader() => new DefaultResourceLoader();
        public IAddressableLoader CreateAddressableLoader() => new DefaultAddressableLoader();
        public IResourceManager CreateResource(IResourceLoader resourceLoader, IAddressableLoader addressableLoader)
            => new ResourceManager(resourceLoader, addressableLoader);
    }
}