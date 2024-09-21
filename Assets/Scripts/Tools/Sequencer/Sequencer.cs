using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Marsion.Tool
{
    public class Sequencer : MonoBehaviour
    {
        public bool IsPlaying;
        public string Name;
        public string CurrentSequence;
        public List<string> CurrentClip;
        public List<string> Track;

        private Sequence _currentSequence;
        private Queue<Sequence> Sequences;

        public void Init()
        {
            IsPlaying = false;
            Track = new List<string>();
            Sequences = new Queue<Sequence>();
            CurrentClip = new List<string>();
        }

        public void Append(Sequence sequence)
        {
            sequence.OnComplete += CompleteSequence;
            Sequences.Enqueue(sequence);
            UpdateTrack();
        }

        private void UpdateTrack()
        {
            Track.Clear();

            if (_currentSequence != null)
            {
                Track.Add($"Sequence : {_currentSequence.Name}");

                if (_currentSequence._currentClip != null)
                    Track.Add($"Clip : {_currentSequence._currentClip.Name}");

                foreach (Clip clip in _currentSequence.Clips)
                {
                    Track.Add($"Clip : {clip.Name}");
                }
            }

            foreach (Sequence sequence in Sequences)
            {
                Track.Add($"Sequence : {sequence.Name}");

                foreach (Clip clip in sequence.Clips)
                {
                    Track.Add($"Clip : {clip.Name}");
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
                Debug.Log($"[{Name}] {sequence.Name} Sequence : Play");
                SetCurrentSequence(sequence.Name);
                sequence.Play();
            }
        }

        private void SetCurrentSequence(string name)
        {
            CurrentSequence = name;
        }

        private void SetCurrentClip(string name)
        {
            CurrentClip.Clear();
            CurrentClip.Add(name);
        }

        private void SetCurrentClip(List<string> titles)
        {
            CurrentClip.Clear();
            CurrentClip = titles;
        }

        private void CompleteSequence()
        {
            IsPlaying = false;
            _currentSequence = null;
            SetCurrentSequence("IDLE");
            SetCurrentClip("IDLE");
            UpdateTrack();
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
                    Debug.Log($"[{Handler.Name}] {Name} sequence : {clip.Name} clip(Append) completed");
                    if(!CompleteClip(clip.ID))
                        PlayNext();
                };

                Clips.Enqueue(clip);
                Checks.Add(clip.ID, false);
            }

            public void Play()
            {
                if (isPlaying)
                {
                    Debug.Log($"[{Handler.Name}] {Name} sequence : Sequence is playing");
                    return;
                }

                isPlaying = true;

                Clips.TryDequeue(out var clip);
                _currentClip = clip;

                Debug.Log($"[{Handler.Name}] {Name} sequence : {clip.Name} clip(Append) play");

                Handler.SetCurrentClip(_currentClip.Name);
                clip.Play();
                
                
                Handler.UpdateTrack();
            }

            private void PlayNext()
            {
                Debug.Log($"[{Handler.Name}] {Name} sequence : Try to play next");

                if (Clips.Count == 0)
                {
                    Debug.Log($"[{Handler.Name}] {Name} sequence : Sequence has reach the end of the sequene");
                    return;
                }

                Play();
            }

            // Joined를 염두에 두고 따로 구현함. 현재는 NextPlay와 기능 거의 동일
            public void Complete()
            {
                Debug.Log($"[{Handler.Name}] {Name} sequence : {Name} sequence complete");
                OnComplete?.Invoke();
                Clear();
            }

            public void Clear()
            {
                Clips.Clear();
                Checks.Clear();
                OnComplete = null;
            }

            private bool CompleteClip(string id)
            {
                Checks[id] = true;
                isPlaying = false;
                _currentClip = null;

                if (CheckEndSequence())
                {
                    Complete();
                    return true;
                }
                else
                {
                    return false;
                }
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

            public void Play()
            {
                OnPlay?.Invoke();

                if (IsAutoComplete)
                    Complete();
            }

            public void Complete()
            {
                OnComplete?.Invoke();
                OnComplete = null;
            }
        }
    }
}