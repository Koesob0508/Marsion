using System.Collections.Generic;

namespace Marsion
{
    public class GameCommandHandler
    {
        private readonly IGameLogic _logic;
        private readonly Queue<ICommand> commandQueue;
        private bool _isExecuting;

        public GameCommandHandler(IGameLogic logic)
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
            }

            _isExecuting = false;
        }

        public void ClearCommands()
        {
            commandQueue.Clear();
        }
    }
}