using System.Collections.Generic;

namespace Marsion
{
    public class CommandInvoker
    {
        private readonly Queue<ICommand> commandQueue = new();

        public void AddCommand(ICommand command)
        {
            commandQueue.Enqueue(command);
        }

        public void ExecuteCommands()
        {
            while(commandQueue.Count > 0)
            {
                var command = commandQueue.Dequeue();
                command.Execute();
            }
        }

        public void ClearCommands()
        {
            commandQueue.Clear();
        }
    }
}