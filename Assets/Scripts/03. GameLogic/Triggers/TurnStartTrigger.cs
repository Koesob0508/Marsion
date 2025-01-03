namespace Marsion
{
    public class TurnStartTrigger : ITrigger
    {
        private readonly Card _card;
        private readonly ICardEffect _effect;

        public void Register()
        {
            //GameEventHandler.RegisterEvent("TurnStart", OnTurnStart);
        }

        public void Unregister()
        {
            //GameEventHandler.UnregisterEvent("TurnStart", OnTurnStart);
        }

        private void OnTurnStart(GameCommandData eventData)
        {
            if (eventData.PlayerID == _card.PlayerID)
            {
                Execute();
            }
        }

        public void Execute()
        {
            //_effect.Apply(_card, Managers.Instance.Data, Managers.Instance.GameLogic);
        }
    }
}