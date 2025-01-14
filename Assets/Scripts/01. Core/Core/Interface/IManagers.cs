namespace Marsion
{
    /// <summary>
    /// Mocking 지원을 위한 IManagers
    /// </summary>
    public interface IManagers
    {
        static Managers Instance { get; }
        IResourceManager Resource { get; }
        IUIManager UI { get; }
        INetworkManagerEx Network { get; }
    }
}