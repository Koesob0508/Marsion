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
                task.Type = TaskType.Append;
                task.OnComplete -= Play;
                task.OnComplete += Play;
                Tasks.Enqueue(task);
            }

            public void Join(Task task)
            {
                task.Type = TaskType.Join;
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
                    var task = Tasks.Dequeue();
                    task.Action.Invoke();

                    if(Tasks.TryPeek(out var result))
                    {
                        if(result.Type == TaskType.Join)
                        {
                            Tasks.Dequeue().Action.Invoke();
                            if (result.AutoComplete)
                                task.OnComplete?.Invoke();
                        }
                    }

                    if(task.AutoComplete)
                        task.OnComplete?.Invoke();
                }
            }
        }
    }
}