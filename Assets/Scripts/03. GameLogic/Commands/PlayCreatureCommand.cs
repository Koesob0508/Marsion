using System;

namespace Marsion
{
    public class PlayCreatureCommand : BaseCommand
    {

        public PlayCreatureCommand(IGameLogic logic, CommandData data, TriggerType trigger) : base(logic, data, trigger) { }

        protected override void Implement()
        {
            var card = GameLogic.DataHandler.GetCardFromHand(CommandData.PlayerID, CommandData.CardUID);

            Logger.Log<PlayCreatureCommand>($"Play Creature : {card.Name} at position {CommandData.IntValue}", colorName: ColorCodes.Logic);

            GameLogic.DataHandler.RemoveCardFromHand(CommandData.PlayerID, CommandData.CardUID);
            GameLogic.DataHandler.AddCardToField(CommandData.PlayerID, card, CommandData.IntValue);

            var creatureSpell = card.GetCreatureSpell();

            if (creatureSpell.Count == 0) return;

            var spellCommand = GameLogic.CommandFactory.CreateCreatureSpell(CommandData.PlayerID, CommandData.CardUID, creatureSpell);
            GameLogic.CommandHandler.Add(spellCommand);

            //_gameLogic.Trigger.TriggerEvent("UpdateData", _data);
            //_gameLogic.Trigger.TriggerEvent("PlayCard", _data);
        }
    }
}