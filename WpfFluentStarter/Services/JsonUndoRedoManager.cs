// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;

namespace WpfFluentStarter.Services;

public class JsonUndoRedoManager<T> : IUndoRedoManager<T>
{
    private readonly Stack<string> _undoStack = new();
    private readonly Stack<string> _redoStack = new();

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;


    public void Snapshot(T currentState)
    {
        string json = JsonSerializer.Serialize(currentState);
        _undoStack.Push(json);
        _redoStack.Clear(); 
    }

    public T? Undo(T currentState)
    {
        if (_undoStack.Count == 0)
        {
            return currentState;
        }

        string currentJson = JsonSerializer.Serialize(currentState);

        _redoStack.Push(currentJson);

        string historyJson = _undoStack.Pop();

        return JsonSerializer.Deserialize<T>(historyJson);
    }

    public T? Redo(T currentState)
    {
        if (_redoStack.Count == 0) return currentState;

        string currentJson = JsonSerializer.Serialize(currentState);

        _undoStack.Push(currentJson);

        string futureJson = _redoStack.Pop();

        return JsonSerializer.Deserialize<T>(futureJson);
    }

    public void Reset()
    {
        _undoStack.Clear();
        _redoStack.Clear();
    }
}
