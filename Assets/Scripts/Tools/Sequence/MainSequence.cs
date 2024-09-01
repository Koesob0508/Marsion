using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Marsion.Tool
{
    public partial class MyTween
    {
        public class MainSequence
        {
            bool isPlaying = false;

            Queue<Sequence> Sequences = new Queue<Sequence>();

            public void Append(Sequence sequence)
            {
                sequence.OnComplete -= PlayNext;
                sequence.OnComplete += PlayNext;
                Sequences.Enqueue(sequence);
            }

            public void Play()
            {
                if (isPlaying)
                    return;
                else
                    isPlaying = true;

                PlayNext();
            }

            private void PlayNext()
            {
                if (Sequences.Count == 0)
                {
                    isPlaying = false;

                    return;
                }

                Sequences.Dequeue().Play();
            }
        }
    }
}