namespace Marsion
{
    public class TurnStartTrigger : ITrigger
    {
        private readonly Card _card;
        private readonly ICardEffect _effect;

        public void Register()
        {
            GameEventManager.RegisterEvent("TurnStart", OnTurnStart);
        }

        public void Unregister()
        {
            GameEventManager.UnregisterEvent("TurnStart", OnTurnStart);
        }

        private void OnTurnStart(object eventData)
        {
            if ((ulong)eventData == _card.PlayerID)
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