using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Marsion.Tool
{
    public partial class MyTween
    {
        public class Sequence
        {
            Queue<Task> Tasks = new Queue<Task>();
            public Action OnComplete;

            public void Append(Task task)
            {
                task.OnComplete -= Play;
                task.OnComplete += Play;
                Tasks.Enqueue(task);
            }

            public void Play()
            {
                if (Tasks.Count == 0)
                {
                    OnComplete?.Invoke();
                }
                else
                {
                    Tasks.Dequeue().Action.Invoke();
                }
            }
        }
    }
}