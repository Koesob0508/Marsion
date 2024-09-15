using System;

namespace Marsion.Tool
{
    public partial class MyTween
    {
        public enum TaskType
        {
            Append,
            Join,
        }

        public class Task
        {
            public TaskType Type;
            public Action Action;
            public Action OnComplete;
            public bool AutoComplete = false;

            public Task(bool autoComplete = false)
            {
                AutoComplete = autoComplete;
            }
        }
    }
}