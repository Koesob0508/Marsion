using UnityEngine;

namespace Marsion.CardView
{
    public interface ICreatureView : ICharacterView
    {
        void Init(ICard card, IFieldView field);
    }
}