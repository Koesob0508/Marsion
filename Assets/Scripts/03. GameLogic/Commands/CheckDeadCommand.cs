using System.Collections.Generic;

namespace Marsion
{
    public class CheckDeadCommand : BaseCommand
    {
        public CheckDeadCommand(IGameLogic logic) : base(logic) { }

        protected override EventData Implement()
        {
            // 필드 등록된 순서대로 탐색
            // 죽은 하수인 제거
            List<ICard> deadCards = new();

            foreach(var card in Logic.GameData.FieldCards)
            {
                if(card.IsDead)
                {
                    deadCards.Add(card);   
                }
            }

            foreach(var card in deadCards)
            {
                card.CastDecompositionAbility();
                Logic.DataHandler.RemoveCardFromField(card.PlayerID, card.UID);
            }

            deadCards.Clear();

            return new EventData { Type = EventType.None };
        }
    }
}