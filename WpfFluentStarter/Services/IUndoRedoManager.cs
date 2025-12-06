// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace WpfFluentStarter.Services;

/// <summary>
/// Memento Pattern based Undo-Redo Manager.
/// </summary>
/// <typeparam name="T">Type of the State</typeparam>
public interface IUndoRedoManager<T>
{
    /// <summary>
    /// Checks, if we can Redo an action.
    /// </summary>
    bool CanRedo { get; }

    /// <summary>
    /// Checks, if we can Undo an action.
    /// </summary>
    bool CanUndo { get; }

    /// <summary>
    /// Redos the last undone action.
    /// </summary>
    /// <param name="currentState">Current State added to the stack</param>
    /// <returns>The future state</returns>
    T? Redo(T currentState);

    /// <summary>
    /// Resets the Undo-Redo history.
    /// </summary>
    void Reset();

    /// <summary>
    /// Snapshots the current state for future Undo operation.
    /// </summary>
    /// <param name="currentState">The current state to snapshot</param>
    void Snapshot(T currentState);

    /// <summary>
    /// Undos the last action.
    /// </summary>
    /// <param name="currentState">The current state added to the stack</param>
    /// <returns>The previous state</returns>
    T? Undo(T currentState);
}