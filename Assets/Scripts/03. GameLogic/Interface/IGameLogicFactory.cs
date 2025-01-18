using System.Collections.Generic;

namespace Marsion
{
    public interface IGameLogicFactory
    {
        // Managers
        IResourceManager ProvideResourceManager();

        List<PlayerInfo> ProvidePlayerInfo();
        IEventHandler CreateEventHandler();
        IGameLogicConfig CreateGameLogicConfig();
        IGameDataHandlerFactory CreateGameDataHandlerFactory(IGameLogic gameLogic, IGameLogicConfig logicConfig);
        IGameDataHandler CreateGameDataHandler();
        ICommandHandler CreateCommandHandler(IGameLogic gameLogic);
        ICommandFactory CreateCommandFactory(IGameLogic gameLogic);
    }

    public class DefaultGameLogicFactory : IGameLogicFactory
    {
        private readonly IManagers _managers;
        private readonly List<PlayerInfo> _playerInfos;

        public DefaultGameLogicFactory(IManagers managers, List<PlayerInfo> playerInfos)
        {
            _managers = managers;
            _playerInfos = playerInfos;
        }

        public IResourceManager ProvideResourceManager() => _managers.Resource;
        public IEventHandler CreateEventHandler() => new EventHandler();
        public List<PlayerInfo> ProvidePlayerInfo() => _playerInfos;
        public IGameLogicConfig CreateGameLogicConfig() => new DefaultGameLogicConfig();
        public IGameDataHandlerFactory CreateGameDataHandlerFactory(IGameLogic gameLogic, IGameLogicConfig logicConfig) => new DefaultGameDataHandlerFactory(gameLogic, logicConfig, _playerInfos);
        public IGameDataHandler CreateGameDataHandler() => new DefaultGameDataHandler();
        public ICommandHandler CreateCommandHandler(IGameLogic gameLogic) => new CommandHandler(gameLogic);
        public ICommandFactory CreateCommandFactory(IGameLogic gameLogic) => new DefaultCommandFactory(gameLogic);
    }
}