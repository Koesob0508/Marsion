using System.Collections.Generic;

namespace Marsion
{
    public class RGameData : IGameData
    {
        public Dictionary<ulong, DefaultPlayer> Players => throw new System.NotImplementedException();

        public DefaultPlayer CurrentPlayer { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public int TurnCount => throw new System.NotImplementedException();

        public void AdvanceTurn()
        {
            throw new System.NotImplementedException();
        }

        public void ChangeCurrentPlayer()
        {
            throw new System.NotImplementedException();
        }

        public DefaultPlayer GetPlayer(ulong PlayerID)
        {
            throw new System.NotImplementedException();
        }

        public void Init(IGameLogicConfig config)
        {
            throw new System.NotImplementedException();
        }

        public void SetCurrentPlayer(ulong playerID)
        {
            throw new System.NotImplementedException();
        }

        public void SetPlayer(DefaultPlayer player)
        {
            throw new System.NotImplementedException();
        }
    }
}