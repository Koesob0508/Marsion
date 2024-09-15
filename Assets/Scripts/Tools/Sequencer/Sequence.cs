using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Marsion.Tool
{
    public class Sequence
    {
        Queue<Clip> Clips = new Queue<Clip>();
        public Action OnComplete;

        public void Append(Clip clip)
        {
            clip.Type = ClipType.Append;
            clip.OnComplete -= Play;
            clip.OnComplete += Play;
            Clips.Enqueue(clip);
        }

        public void Join(Clip clip)
        {
            clip.Type = ClipType.Join;
            clip.OnComplete -= Play;
            clip.OnComplete += Play;
            Clips.Enqueue(clip);
        }

        public void Play()
        {
            if (Clips.Count == 0)
            {
                OnComplete?.Invoke();
            }
            else
            {
                var clip = Clips.Dequeue();
                clip.Play();

                List<Clip> joinedClips = new List<Clip>();

                Clip nextClip;
                Clips.TryPeek(out nextClip);

                while (nextClip.Type == ClipType.Join)
                {
                    joinedClips.Add(nextClip);

                    Clips.TryPeek(out nextClip);
                }

                foreach (Clip joinedClip in joinedClips)
                {
                    joinedClip.Play();
                }

                joinedClips.Clear();

                if (clip.AutoComplete)
                    clip.Complete();

            }
        }
    }
}