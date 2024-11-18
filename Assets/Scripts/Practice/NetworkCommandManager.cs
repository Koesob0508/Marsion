using Unity.Netcode;

namespace Practice
{
    public class NetworkCommandManager : NetworkBehaviour
    {
        private CommandManager _commandManager;

        private void Awake()
        {
            _commandManager = new CommandManager();
        }

        [ServerRpc]
        public void UseCardServerRpc(ulong playerID, string cardID)
        {
            var player = FindPlayerByID(playerID);
            var card = FindCardByID(cardID);

            if(player != null && card != null)
            {
                var command = new UseCardCommand(card, player);
                _commandManager.ExecuteCommand(command);

                UseCardClientRpc(playerID, cardID);
            }
        }

        [ClientRpc]
        public void UseCardClientRpc(ulong playerID, string cardID)
        {
            if (IsServer) return;

            var player = FindPlayerByID(playerID);
            var card = FindCardByID(cardID);

            if(player != null && card != null)
            {
                var command = new UseCardCommand(card, player);
                _commandManager.ExecuteCommand(command);
            }
        }

        private Player FindPlayerByID(ulong playerID)
        {
            return null;
        }

        private string FindCardByID(string cardID)
        {
            return null;
        }
    }
}