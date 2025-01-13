using System.Collections.Generic;

namespace Marsion
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IGameLogic _logic;
        private readonly Stack<ICommand> _stack;
        private bool _isExecuting;

        public CommandHandler(IGameLogic logic)
        {
            _logic = logic;
            _stack = new();
            _isExecuting = false;
        }

        public void Add(ICommand command)
        {
            Logger.Log<CommandHandler>($"Add Command : {command.GetType().Name}", colorName: ColorCodes.Orange);

            _stack.Push(command);
            ExecuteCommands();
        }

        public void Remove(ICommand command)
        {

        }

        private void ExecuteCommands()
        {
            if (_isExecuting) return;

            _isExecuting = true;

            while(_stack.Count > 0)
            {
                var command = _stack.Pop();
                Logger.Log<CommandHandler>($"Execute Command : {command.GetType().Name}", colorName: ColorCodes.Orange);
                command.Execute();
            }

            _isExecuting = false;
        }

        public void Clear()
        {
            _stack.Clear();
        }
    }
}