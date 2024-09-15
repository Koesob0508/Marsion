using DG.Tweening;
using UnityEngine;

namespace Marsion.Tool
{
    public class SequencerTest : MonoBehaviour
    {
        [SerializeField] Sequencer Sequencer;
        public int sequenceCount = 0;
        public int clipCount = 0;

        private void Start()
        {
            Sequencer.Init();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Sequencer.Clip clip = new Sequencer.Clip($"Test {clipCount} Clip ", false);
                clipCount++;
                clip.OnPlay += () =>
                {
                    DOTween.Sequence()
                            .AppendInterval(3f)
                            .AppendCallback(() =>
                            {
                                Debug.Log($"Test {clipCount} Clip");
                            })
                            .OnComplete(() =>
                            {
                                clip.Complete();
                            });
                };

                Sequencer.Sequence sequence = new Sequencer.Sequence($"Test {sequenceCount} Sequence");
                sequenceCount++;
                sequence.Append(clip);

                Sequencer.Append(sequence);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                Sequencer.Clip clip1 = new Sequencer.Clip($"Test {clipCount} Clip ", false);
                clipCount++;
                clip1.OnPlay += () =>
                {
                    DOTween.Sequence()
                            .AppendInterval(3f)
                            .AppendCallback(() =>
                            {
                                Debug.Log($"Test {clipCount} Clip");
                            })
                            .OnComplete(() =>
                            {
                                clip1.Complete();
                            });
                };

                Sequencer.Clip clip2 = new Sequencer.Clip($"Test {clipCount} Clip ", false);
                clipCount++;
                clip2.OnPlay += () =>
                {
                    DOTween.Sequence()
                            .AppendInterval(3f)
                            .AppendCallback(() =>
                            {
                                Debug.Log($"Test {clipCount} Clip");
                            })
                            .OnComplete(() =>
                            {
                                clip2.Complete();
                            });
                };

                Sequencer.Sequence sequence = new Sequencer.Sequence($"Test {sequenceCount} Sequence");
                sequenceCount++;
                sequence.Append(clip1);
                sequence.Append(clip2);

                Sequencer.Append(sequence);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Sequencer.Clip clip1 = new Sequencer.Clip($"Test {clipCount} Clip ", false);
                clipCount++;
                clip1.OnPlay += () =>
                {
                    DOTween.Sequence()
                            .AppendInterval(3f)
                            .AppendCallback(() =>
                            {
                                Debug.Log($"Test {clipCount} Clip");
                            })
                            .OnComplete(() =>
                            {
                                clip1.Complete();
                            });
                };

                Sequencer.Clip clip2 = new Sequencer.Clip($"Test {clipCount} Clip ", false);
                clipCount++;
                clip2.OnPlay += () =>
                {
                    DOTween.Sequence()
                            .AppendInterval(3f)
                            .AppendCallback(() =>
                            {
                                Debug.Log($"Test {clipCount} Clip");
                            })
                            .OnComplete(() =>
                            {
                                clip2.Complete();
                            });
                };

                Sequencer.Clip clip3 = new Sequencer.Clip($"Test {clipCount} Clip ", false);
                clipCount++;
                clip3.OnPlay += () =>
                {
                    DOTween.Sequence()
                            .AppendInterval(3f)
                            .AppendCallback(() =>
                            {
                                Debug.Log($"Test {clipCount} Clip");
                            })
                            .OnComplete(() =>
                            {
                                clip3.Complete();
                            });
                };

                Sequencer.Sequence sequence = new Sequencer.Sequence($"Test {sequenceCount} Sequence");
                sequenceCount++;
                sequence.Append(clip1);
                sequence.Append(clip2);
                sequence.Append(clip3);

                Sequencer.Append(sequence);
            }
        }
    }
}