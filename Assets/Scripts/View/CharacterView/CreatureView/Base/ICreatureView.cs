using UnityEngine;

namespace Marsion.CardView
{
    public interface ICreatureView : ICharacterView
    {
        void Init(Card card, IFieldView field);
    }
}