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
                Sequencer.Sequence sequence = new Sequencer.Sequence($"Test {sequenceCount} Sequence", Sequencer);
                
                Sequencer.Clip clip = new Sequencer.Clip($"Test {clipCount} Clip ", sequence, false);
                clipCount++;
                clip.OnPlay += () =>
                {
                    DOTween.Sequence()
                            .AppendInterval(3f)
                            .AppendCallback(() =>
                            {
                                Debug.Log($"{clip.Name} on play");
                            })
                            .OnComplete(() =>
                            {
                                clip.Complete();
                            });
                };

                sequenceCount++;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                Sequencer.Sequence sequence = new Sequencer.Sequence($"Test {sequenceCount} Sequence", Sequencer);

                Sequencer.Clip clip1 = new Sequencer.Clip($"Test {clipCount} Clip ", sequence, false);
                clipCount++;
                clip1.OnPlay += () =>
                {
                    DOTween.Sequence()
                            .AppendInterval(3f)
                            .AppendCallback(() =>
                            {
                                Debug.Log($"{clip1.Name} on play");
                            })
                            .OnComplete(() =>
                            {
                                clip1.Complete();
                            });
                };

                Sequencer.Clip clip2 = new Sequencer.Clip($"Test {clipCount} Clip ", sequence, false);
                clipCount++;
                clip2.OnPlay += () =>
                {
                    DOTween.Sequence()
                            .AppendInterval(3f)
                            .AppendCallback(() =>
                            {
                                Debug.Log($"{clip2.Name}  on play");
                            })
                            .OnComplete(() =>
                            {
                                clip2.Complete();
                            });
                };

                sequenceCount++;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Sequencer.Sequence sequence = new Sequencer.Sequence($"Test {sequenceCount} Sequence", Sequencer);

                Sequencer.Clip clip1 = new Sequencer.Clip($"Test {clipCount} Clip ", sequence, false);
                clipCount++;
                clip1.OnPlay += () =>
                {
                    DOTween.Sequence()
                            .AppendInterval(3f)
                            .AppendCallback(() =>
                            {
                                Debug.Log($"{clip1.Name}  on play");
                            })
                            .OnComplete(() =>
                            {
                                clip1.Complete();
                            });
                };

                Sequencer.Clip clip2 = new Sequencer.Clip($"Test {clipCount} Clip ", sequence, false);
                clipCount++;
                clip2.OnPlay += () =>
                {
                    DOTween.Sequence()
                            .AppendInterval(3f)
                            .AppendCallback(() =>
                            {
                                Debug.Log($"{clip2.Name}  on play");
                            })
                            .OnComplete(() =>
                            {
                                clip2.Complete();
                            });
                };

                Sequencer.Clip clip3 = new Sequencer.Clip($"Test {clipCount} Clip ", sequence, false);
                clipCount++;
                clip3.OnPlay += () =>
                {
                    DOTween.Sequence()
                            .AppendInterval(3f)
                            .AppendCallback(() =>
                            {
                                Debug.Log($"{clip3.Name}  on play");
                            })
                            .OnComplete(() =>
                            {
                                clip3.Complete();
                            });
                };

                
                sequenceCount++;
            }
        }
    }
}