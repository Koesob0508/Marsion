using System;
using UnityEngine;

namespace Marsion.Tool
{
    public enum ClipType
    {
        Append,
        Join,
    }

    public class Clip
    {
        public ClipType Type { get; set; }
        public string Name { get; private set; }
        public event Action Action;
        public event Action OnComplete;
        public bool AutoComplete { get; private set; }

        public Clip(string name, bool autoComplete = false)
        {
            Name = name;
            AutoComplete = autoComplete;
        }

        public void Play()
        {
            Action?.Invoke();
        }

        public void Complete()
        {
            OnComplete?.Invoke();

            Action = null;
            OnComplete = null;
        }
    }
}