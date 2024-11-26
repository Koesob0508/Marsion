using UnityEngine;

namespace Practice
{
    public class Player
    {
        public void UseCard(string card)
        {
            Debug.Log($"Use {card}");
        }

        public void UndoUseCard(string card)
        {
            Debug.Log($"Undo use {card}");
        }
    }

    public class UseCardCommand : ICommand
    {
        private readonly string _card;
        private readonly Player _player;
        private readonly float _effectDuration = 1.5f;

        public UseCardCommand(string card, Player player)
        {
            _card = card;
            _player = player;
        }

        public void Execute()
        {
            _player.UseCard(_card);
            ShowEffect();
        }

        public void Undo()
        {
            _player.UndoUseCard(_card);
            HideEffect();
        }

        public float GetEffectDuration()
        {
            return _effectDuration;
        }

        private void ShowEffect()
        {
            Debug.Log($"Effect started for card {_card}");
        }

        private void HideEffect()
        {
            Debug.Log($"Effect ended for card {_card}");
        }
    }
}