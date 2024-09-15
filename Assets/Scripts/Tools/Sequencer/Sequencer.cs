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

        private Sequence _currentSequence;
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
            UpdateTitles();
        }

        private void UpdateTitles()
        {
            Titles.Clear();

            if (_currentSequence != null)
            {
                Titles.Add($"Sequence : {_currentSequence.Name}");

                if (_currentSequence._currentClip != null)
                    Titles.Add($"Clip : {_currentSequence._currentClip.Name}");

                foreach (Clip clip in _currentSequence.Clips)
                {
                    Titles.Add($"Clip : {clip.Name}");
                }
            }

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

            IsPlaying = true;

            if (Sequences.TryDequeue(out var sequence))
            {
                _currentSequence = sequence;
                Debug.Log($"{sequence.Name} Play");
                sequence.Play();
                SetCurrentSequence(sequence.Name);
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
            _currentSequence = null;
            SetCurrentSequence("IDLE");
            SetCurrentClip("IDLE");
            UpdateTitles();
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

            public Clip _currentClip { get; private set; }

            public Sequencer Handler { get; private set; }

            public Sequence(string name, Sequencer handler)
            {
                isPlaying = false;

                ID = Guid.NewGuid().ToString();
                Name = name;
                Handler = handler;

                Clips = new Queue<Clip>();
                Checks = new Dictionary<string, bool>();
            }

            public void Append(Clip clip)
            {
                clip.OnComplete += () =>
                {
                    CompleteClip(clip.ID);
                    PlayNext();
                };

                Clips.Enqueue(clip);
                Checks.Add(clip.ID, false);
            }

            public void Play()
            {
                if (isPlaying)
                {
                    Debug.Log("Sequence is playing");
                    return;
                }

                isPlaying = true;

                Clips.TryDequeue(out var clip);
                _currentClip = clip;
                clip.Play();
                Handler.UpdateTitles();

                onClipPlay?.Invoke(clip.Name);
            }

            private void PlayNext()
            {
                Debug.Log("Try play next");

                if (Clips.Count == 0)
                {
                    return;
                }

                Play();
            }

            // Joined�� ���ο� �� �Լ�. ������ PlayNext�� �ٸ��� �ʴ�.
            public void Complete()
            {
                Debug.Log($"{Name} complete");
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
                Debug.Log("Clip completed");
                Checks[id] = true;
                isPlaying = false;
                _currentClip = null;

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
                Debug.Log($"{Name} play");
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