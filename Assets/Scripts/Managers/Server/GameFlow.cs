using UnityEngine.Events;

namespace Marsion.Server
{
    public class GameFlow
    {
        public UnityAction onGameStart;

        public void StartGame()
        {
            onGameStart?.Invoke();
        }
    }
}