using UnityEngine.Events;

namespace Marsion.Clinet
{
    public interface IClientLogic
    {
        UnityAction OnStartGame { get; }
    }
}