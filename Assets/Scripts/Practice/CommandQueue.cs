using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Practice
{
    public class CommandQueue : MonoBehaviour
    {
        private readonly Queue<ICommand> _commandQueue = new();
        private bool _isExecuting = false;

        public void EnqueueCommand(ICommand command)
        {
            _commandQueue.Enqueue(command);
            if (!_isExecuting)
            {
                ExecuteNextCommand();
            }
        }

        private void ExecuteNextCommand()
        {
            if (_commandQueue.Count == 0)
            {
                _isExecuting = false;
                return;
            }

            _isExecuting = true;
            var command = _commandQueue.Dequeue();
            command.Execute();

            StartCoroutine(WaitForEffectToFinish(command));
        }

        private IEnumerator WaitForEffectToFinish(ICommand command)
        {
            yield return new WaitForSeconds(command.GetEffectDuration());

            ExecuteNextCommand();
        }
    }
}