using System.Collections.Generic;

namespace Marsion
{
    public class CommandHandler
    {
        private readonly IGameLogic _logic;
        private readonly Queue<ICommand> commandQueue;
        private bool _isExecuting;

        public CommandHandler(IGameLogic logic)
        {
            _logic = logic;
            commandQueue = new();
            _isExecuting = false;
        }

        public void AddCommand(ICommand command)
        {
            commandQueue.Enqueue(command);
            ExecuteCommands();
        }

        private void ExecuteCommands()
        {
            if (_isExecuting) return;

            _isExecuting = true;

            while(commandQueue.Count > 0)
            {
                var command = commandQueue.Dequeue();
                command.Execute();
                command.Clear();
            }

            _isExecuting = false;
        }

        public void ClearCommands()
        {
            commandQueue.Clear();
        }
    }
}