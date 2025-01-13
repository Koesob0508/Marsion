using System;
using System.Collections.Generic;

namespace Marsion
{
    public interface ICommandFactory
    {
        ICommand CreatePayMana(ulong playerID, int amount);
        ICommand CreatePlayCreature(ulong playerID, string cardUID, int index);
        ICommand CreateAttack(ulong attackerID, string attackerUID, ulong defenderID, string defenderUID);
        ICommand CreateCreatureSpell(ulong playerID, string cardUID, List<AbilitySO> abilities);
        ICommand CreateCreatureEffect(ulong playerID, string cardUID, EffectSO effect);
    }

    public class DefaultCommandFactory : ICommandFactory
    {
        private readonly IGameLogic _gameLogic;

        public DefaultCommandFactory(IGameLogic gameLogic)
        {
            _gameLogic = gameLogic;
        }

        public ICommand CreatePayMana(ulong playerID, int amount)
        {
            var data = new CommandData();
            data.PlayerID = playerID;
            data.IntValue = amount;

            return new PayManaCommand(_gameLogic, data, TriggerType.PayMana);
        }

        public ICommand CreatePlayCreature(ulong playerID, string cardUID, int index)
        {
            var data = new CommandData();
            data.PlayerID = playerID;
            data.CardUID = cardUID;
            data.IntValue = index;

            return new PlayCreatureCommand(_gameLogic, data, TriggerType.PlayCreature);
        }

        public ICommand CreateAttack(ulong attackerID, string attackerUID, ulong defenderID, string defenderUID)
        {
            var data = new CommandData();
            data.PlayerID = attackerID;
            data.CardUID = attackerUID;
            data.TargetPlayerID = defenderID;
            data.TargetCardUID = defenderUID;

            return new AttackCommand(_gameLogic, data, TriggerType.Attack);
        }

        public ICommand CreateCreatureSpell(ulong playerID, string cardUID, List<AbilitySO> abilities)
        {
            var data = new CommandData();
            data.PlayerID = playerID;
            data.CardUID = cardUID;

            var command = new CreatureSpellCommand(_gameLogic, data, TriggerType.CreatureSpell);
            command.Register(abilities);

            return command;
        }

        public ICommand CreateCreatureEffect(ulong playerID, string cardUID, EffectSO effect)
        {
            var data = new CommandData();
            data.PlayerID = playerID;
            data.CardUID = cardUID;
            var command = new CreatureEffectCommand(_gameLogic, data, effect.TriggerType);
            command.Register(effect);

            return command;
        }
    }
}