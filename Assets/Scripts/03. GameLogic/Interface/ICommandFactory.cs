using System;
using System.Collections.Generic;

namespace Marsion
{
    public interface ICommandFactory
    {
        ICommand CreateStartGame();
        ICommand CreateCheckDead();
        ICommand CreateDraw(ulong commanderID, ulong targetID, int count);
        ICommand CreateStartTurn(ulong commanderID);
        ICommand CreateEndTurn(ulong commanderID);
        ICommand CreatePlayCreature(ulong commanderID, string commanderCardUID, int index);
        ICommand CreatePayMana(ulong commanderID, int amount);
        ICommand CreateCastSpell(ulong commanderID, string commanderCardUID, BaseAbility ability, AbilityType type);
        ICommand CreateSpawnFromHand(ulong commanderID, string commanderCardUID, int index);
        ICommand CreateAfterSpawn(ulong commanderID, string commanderCardUID);
        ICommand CreateKillCreature(ulong commanderID, string commanderCardUID, ulong targetPlayerID, string targetCardUID);
        ICommand CreateAttackCreature(ulong commanderID, string commanderCardUID, ulong targetPlayerID, string targetCardUID);
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

        public ICommand CreateCheckDead()
        {
            return new CheckDeadCommand(_gameLogic);
        }

        public ICommand CreateDraw(ulong commanderID, ulong targetID, int count)
        {

            return new DrawCommand(_gameLogic, commanderID, targetID, count);
        }

        public ICommand CreateStartTurn(ulong commanderID)
        {

            return new StartTurnCommand(_gameLogic, commanderID);
        }

        public ICommand CreateEndTurn(ulong commanderID)
        {
            return new EndTurnCommand(_gameLogic, commanderID);
        }
        public ICommand CreatePlayCreature(ulong commanderID, string commanderCardUID, int index)
        {
            return new PlayCreatureCommand(_gameLogic, commanderID, commanderCardUID, index);
        }

        public ICommand CreatePayMana(ulong commanderID, int amount)
        {
            return new PayManaCommand(_gameLogic, commanderID, amount);
        }

        public ICommand CreateCastSpell(ulong commanderID, string commanderCardUID, BaseAbility ability, AbilityType type)
        {
            return new CastSpellCommand(_gameLogic, commanderID, commanderCardUID, ability, type);
        }

        // 손에서 낼 때, 덱에서 소환할 때랑 아예 새로운 카드가 소환되는건 다르게 구현해야할듯
        public ICommand CreateSpawnFromHand(ulong commanderID, string commanderCardUID, int index)
        {
            return new SpawnCardFromHandCommand(_gameLogic, commanderID, commanderCardUID, index);
        }

        public ICommand CreateAfterSpawn(ulong commanderID, string commanderCardUID)
        {
            return new AfterSpawnCommand(_gameLogic, commanderID, commanderCardUID);
        }

        public ICommand CreateKillCreature(ulong commanderID, string commanderCardUID, ulong targetPlayerID, string targetCardUID)
        {
            return new KillCommand(_gameLogic, commanderID, commanderCardUID, targetPlayerID, targetCardUID);
        }

        public ICommand CreateAttackCreature(ulong commanderID, string commanderCardUID, ulong targetPlayerID, string targetCardUID)
        {
            return new AttackCommand(_gameLogic, commanderID, commanderCardUID, targetPlayerID, targetCardUID);
        }
    }
}