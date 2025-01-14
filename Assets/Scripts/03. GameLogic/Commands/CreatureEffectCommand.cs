namespace Marsion
{
    public class CreatureEffectCommand : BaseCommand
    {
        private readonly ICard card;
        private EffectSO _effect;

        public CreatureEffectCommand(IGameLogic logic, CommandData data, TriggerType trigger) : base(logic, data, trigger)
        {
            card = GameLogic.DataHandler.GetCardFromField(data.PlayerID, data.CardUID);
        }

        public void Register(EffectSO effect)
        {
            _effect = effect;
        }

        protected override void Implement()
        {
            _effect.Execute(GameLogic, card);
        }
    }
}