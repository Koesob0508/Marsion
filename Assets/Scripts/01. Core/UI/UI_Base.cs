using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Marsion
{
    public abstract class UI_Base : MonoBehaviour
    {
        public abstract void Init();
        private void Start()
        {
            Init();
        }
    }
}