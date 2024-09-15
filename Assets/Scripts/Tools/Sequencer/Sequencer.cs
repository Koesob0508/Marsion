using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Marsion.Tool
{
    public class Sequencer : MonoBehaviour
    {
        public bool IsPlaying;
        public string CurrentSequence;
        public string CurrentClip;
        public List<string> Titles;

        private Queue<Sequence> Sequences;

        public void Init()
        {
            IsPlaying = false;
            Titles = new List<string>();
            Sequences = new Queue<Sequence>();
        }

        public void Append(Sequence sequence)
        {
            sequence.onClipPlay += SetCurrentClip;
            sequence.OnComplete += CompleteSequence;
            Sequences.Enqueue(sequence);
            UpdateSequence();
        }

        private void UpdateSequence()
        {
            Titles.Clear();

            foreach (Sequence sequence in Sequences)
            {
                Titles.Add($"Sequence : {sequence.Name}");

                foreach (Clip clip in sequence.Clips)
                {
                    Titles.Add($"Clip : {clip.Name}");
                }
            }
        }

        private void Play()
        {
            if (IsPlaying) return;

            Debug.Log("Sequencer Play");

            IsPlaying = true;

            if (Sequences.TryDequeue(out var sequence))
            {
                sequence.Play();
                SetCurrentSequence(sequence.Name);
            }
            else
            {
                Debug.Log("Sequence 없다는 오류");
            }
        }

        private void SetCurrentSequence(string name)
        {
            CurrentSequence = name;
        }

        private void SetCurrentClip(string name)
        {
            CurrentClip = name;
        }

        private void CompleteSequence()
        {
            IsPlaying = false;
            UpdateSequence();
        }

        private void Update()
        {
            if (Sequences.Count > 0)
            {
                Play();
            }
        }

        public class Sequence
        {
            bool isPlaying;

            public string ID { get; private set; }
            public string Name { get; private set; }
            public Queue<Clip> Clips;
            public Dictionary<string, bool> Checks;
            public event Action<string> onClipPlay;
            public event Action OnComplete;

            public Sequence(string name)
            {
                isPlaying = false;

                ID = Guid.NewGuid().ToString();
                Name = name;

                Clips = new Queue<Clip>();
                Checks = new Dictionary<string, bool>();
            }

            public void Append(Clip clip)
            {
                clip.OnComplete += () =>
                {
                    CompleteClip(clip.ID);
                    Play();
                };

                Clips.Enqueue(clip);
                Checks.Add(clip.ID, false);
            }

            public void Play()
            {
                if (Clips.Count == 0)
                    Complete();

                if (isPlaying)
                    return;

                isPlaying = true;

                Clips.TryDequeue(out var clip);
                clip.Play();
                onClipPlay?.Invoke(clip.Name);
            }

            public void Complete()
            {
                OnComplete?.Invoke();
                Clear();
            }

            public void Clear()
            {
                Clips.Clear();
                Checks.Clear();
                OnComplete = null;
            }

            private void CompleteClip(string id)
            {
                Checks[id] = true;

                if (CheckEndSequence())
                    Complete();
            }

            private bool CheckEndSequence()
            {
                foreach (var isCompleted in Checks.Values)
                {
                    if (!isCompleted)
                        return false;
                }

                return true;
            }
        }

        public class Clip
        {
            public string ID { get; private set; }
            public string Name { get; private set; }
            public event Action OnPlay;
            public event Action OnComplete;
            public bool IsAutoComplete { get; private set; }

            public Clip(string name, bool isAutoComplete = true)
            {
                ID = Guid.NewGuid().ToString();
                Name = name;
                IsAutoComplete = isAutoComplete;
            }

            public Action Play()
            {
                OnPlay?.Invoke();

                if (IsAutoComplete)
                {
                    Complete();
                    return null;
                }
                else
                {
                    return OnComplete;
                }
            }

            public void Complete()
            {
                OnComplete?.Invoke();
                OnComplete = null;
            }
        }
    }
}