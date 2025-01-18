namespace Marsion
{
    public class PayManaCommand : BaseCommand
    {
        public PayManaCommand(IGameLogic logic, EventData data, EventType trigger) : base(logic, data, trigger) { }
        protected override void Implement()
        {
            Logger.Log<IGameLogic>($"Player {EventData.PlayerID} pay mana {EventData.IntValue}", colorName: ColorCodes.Logic);

            Logic.DataHandler.PayMana(EventData.PlayerID, EventData.IntValue);
            //_gameLogic.Trigger.TriggerEvent("UpdateData", _data);
            //_gameLogic.Trigger.TriggerEvent("ChangeMana", _data);
        }
    }
}