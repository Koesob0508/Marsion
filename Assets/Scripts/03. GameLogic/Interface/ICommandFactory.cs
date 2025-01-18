using System;
using System.Collections.Generic;

namespace Marsion
{
    public interface ICommandFactory
    {
        ICommand CreateStartGame();
        ICommand CreateDraw(ulong playerID, int count);
        ICommand CreateStartTurn();
        ICommand CreateEndTurn();
        ICommand CreatePlayCard(ulong playerID, string cardUID, int index);
        ICommand CreatePayMana(ulong playerID, int amount);
        ICommand CreateAddSpell(ulong playerID, string cardUID);
        ICommand CreateCastSpell(AbilitySO ability, EventData eventData);
        ICommand CreateSpawnCardFromHand(ulong playerID, string cardUID, int index);
        ICommand CreateAttack(ulong attackerID, string attackerUID, ulong defenderID, string defenderUID);
    }

    public class DefaultCommandFactory : ICommandFactory
    {
        private readonly IGameLogic _gameLogic;

        public DefaultCommandFactory(IGameLogic gameLogic)
        {
            _gameLogic = gameLogic;
        }

        public ICommand CreateStartGame()
        {
            return new StartGameCommand(_gameLogic);
        }

        public ICommand CreateDraw(ulong playerID, int count)
        {
            return new DrawCommand(_gameLogic, playerID, count);
        }

        public ICommand CreateStartTurn()
        {
            return new StartTurnCommand(_gameLogic);
        }

        public ICommand CreateEndTurn()
        {
            return new EndTurnCommand(_gameLogic);
        }
        public ICommand CreatePlayCard(ulong playerID, string cardUID, int index)
        {
            return new PlayCardCommand(_gameLogic, playerID, cardUID, index);
        }

        public ICommand CreatePayMana(ulong playerID, int amount)
        {
            var data = new EventData();
            data.PlayerID = playerID;
            data.IntValue = amount;

            return new PayManaCommand(_gameLogic, data, EventType.PayMana);
        }

        public ICommand CreateAddSpell(ulong playerID, string cardUID)
        {
            return new AddSpellCommand(_gameLogic, playerID, cardUID);
        }

        public ICommand CreateCastSpell(AbilitySO ability, EventData eventData)
        {
            return new CastSpellCommand(_gameLogic, ability, eventData);
        }

        // 손에서 낼 때, 덱에서 소환할 때랑 아예 새로운 카드가 소환되는건 다르게 구현해야할듯
        public ICommand CreateSpawnCardFromHand(ulong playerID, string cardUID, int index)
        {
            return new SpawnCardFromHandCommand(_gameLogic, playerID, cardUID, index);
        }

        public ICommand CreateAttack(ulong attackerID, string attackerUID, ulong defenderID, string defenderUID)
        {
            var data = new EventData();
            data.PlayerID = attackerID;
            data.CardUID = attackerUID;
            data.TargetPlayerID = defenderID;
            data.TargetCardUID = defenderUID;

            return new AttackCommand(_gameLogic, data, EventType.Attack);
        }
    }
}