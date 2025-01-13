namespace Marsion
{
    public class PayManaCommand : BaseCommand
    {
        public PayManaCommand(IGameLogic logic, CommandData data, TriggerType trigger) : base(logic, data, trigger) { }
        protected override void Implement()
        {
            Logger.Log<PayManaCommand>($"Player {CommandData.PlayerID} pay mana {CommandData.IntValue}", colorName: ColorCodes.Logic);

            GameLogic.DataHandler.PayMana(CommandData.PlayerID, CommandData.IntValue);
            //_gameLogic.Trigger.TriggerEvent("UpdateData", _data);
            //_gameLogic.Trigger.TriggerEvent("ChangeMana", _data);
        }
    }
}