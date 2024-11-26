using System.Collections.Generic;

namespace Practice
{
    public class CommandManager
    {
        private readonly Stack<ICommand> _commandHistory = new();
        private readonly Stack<ICommand> _redoStack = new();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _commandHistory.Push(command);
            _redoStack.Clear();
        }

        public void Undo()
        {
            if(_commandHistory.Count> 0)
            {
                var command = _commandHistory.Pop();
                command.Undo();
                _redoStack.Push(command);
            }    
        }

        public void Redo()
        {
            if(_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
                command.Execute();
                _commandHistory.Push(command);
            }
        }
    }
}