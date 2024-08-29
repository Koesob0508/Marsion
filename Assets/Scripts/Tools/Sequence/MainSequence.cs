using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Marsion.Tool
{
    public partial class MyTween
    {
        public class MainSequence
        {
            Queue<Sequence> Sequences = new Queue<Sequence>();

            public void Append(Sequence sequence)
            {
                sequence.OnComplete -= Play;
                sequence.OnComplete += Play;
                Sequences.Enqueue(sequence);
            }

            public void Play()
            {
                if (Sequences.Count == 0) return;

                Sequences.Dequeue().Play();
            }
        }
    }
}