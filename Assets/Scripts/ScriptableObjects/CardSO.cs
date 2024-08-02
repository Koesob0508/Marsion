﻿using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "CardSO", menuName = "CardSO")]
    public class CardSO : ScriptableObject
    {
        public int ID;
        public string Name;
        public Sprite Sprite;
        public int Mana;
        public int Marsion;
        public int Attack;
        public int Health;
        public string Explanation;
        public string VanilaText;
    }
}