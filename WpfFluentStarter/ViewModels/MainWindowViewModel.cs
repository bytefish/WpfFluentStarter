// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using WpfFluentStarter.Models;
using WpfFluentStarter.Services;

namespace WpfFluentStarter.ViewModels;

/// <summary>
/// View model for the main window.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    /// <summary>
    /// Used for logging operations.
    /// </summary>
    private readonly ILogger<MainWindowViewModel> _logger;

    /// <summary>
    /// Undo-Redo Manager based on the Memento Pattern.
    /// </summary>
    private readonly IUndoRedoManager<ObservableCollection<FurnitureItem>> _undoRedoManager;

    /// <summary>
    /// Storage service for saving and loading items.
    /// </summary>
    private readonly IFileStorageService<ObservableCollection<FurnitureItem>> _fileStorageService;

    /// <summary>
    /// The dialog service for showing dialogs.
    /// </summary>
    private readonly IDialogService _dialogService;

    /// <summary>
    /// Random Number Generator.
    /// </summary>
    private readonly Random _rng = new();

    /// <summary>
    /// Name of the current design.
    /// </summary>
    [ObservableProperty]
    private string _name = "my-design";

    /// <summary>
    /// The State of Furniture Items in the room.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<FurnitureItem> _items = new();

    /// <summary>
    /// Creates a new instance of the MainWindowViewModel.
    /// </summary>
    /// <param name="logger">The logger used for logging operations</param>
    /// <param name="dialogService">The dialog service for showing dialogs</param>
    /// <param name="undoRedoManager">The Undo-Redo Manager based on the Memento Pattern</param>
    /// <param name="fileStorageService">The storage service for saving and loading items</param>
    public MainWindowViewModel(ILogger<MainWindowViewModel> logger, 
        IDialogService dialogService, 
        IUndoRedoManager<ObservableCollection<FurnitureItem>> undoRedoManager, 
        IFileStorageService<ObservableCollection<FurnitureItem>> fileStorageService)
    {
        _logger = logger;
        _dialogService = dialogService;
        _undoRedoManager = undoRedoManager;
        _fileStorageService = fileStorageService;
    }

    /// <summary>
    /// Checks if we can undo the last action.
    /// </summary>
    /// <returns><see cref="true"/>, if Undo is allowed; else <see cref="false"/></returns>
    private bool CanUndo() => _undoRedoManager.CanUndo;

    /// <summary>
    /// Checks if we can redo the undone action.
    /// </summary>
    /// <returns><see cref="true"/>, if Red is allowed; else <see cref="false"/></returns>    
    private bool CanRedo() => _undoRedoManager.CanRedo;

    /// <summary>
    /// Undos the last action.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void Undo()
    {
        Items = _undoRedoManager.Undo(Items) ?? new();
        UpdateCommandStates();
    }

    /// <summary>
    /// Redos the last undone action.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRedo))]
    private void Redo()
    {
        Items = _undoRedoManager.Redo(Items) ?? new();
        UpdateCommandStates();
    }

    /// <summary>
    /// Updates the command states for Undo and Redo.
    /// </summary>
    private void UpdateCommandStates()
    {
        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Adds a new Table to the room.
    /// </summary>
    [RelayCommand]
    private void AddTable()
    {
        _undoRedoManager.Snapshot(Items);

        Items.Add(new FurnitureItem
        {
            Name = "Table",
            X = _rng.Next(0, 300),
            Y = _rng.Next(0, 300),
            ColorHex = "#8B4513"
        });

        UpdateCommandStates();
    }

    [RelayCommand]
    private void Scramble()
    {
        if (Items.Count == 0)
        {
            return;
        }

        _undoRedoManager.Snapshot(Items);

        foreach (var item in Items)
        {
            item.X = _rng.Next(0, 300);
            item.Y = _rng.Next(0, 300);
        }

        UpdateCommandStates();
    }

    /// <summary>
    /// Quit Command, but checks for unsaved changes first.
    /// </summary>
    [RelayCommand]
    private void ShowAboutWindow()
    {
        _dialogService.ShowAboutWindow();
    }

    /// <summary>
    /// Quit Command, but checks for unsaved changes first.
    /// </summary>
    [RelayCommand]
    private void Quit()
    {
        Application.Current.MainWindow?.Close();
    }

    /// <summary>
    /// Checks for unsaved changes and prompts the user.
    /// </summary>
    public bool CanClose()
    {
        if (_undoRedoManager.CanUndo)
        {
            // Show prompt to save changes
            MessageBoxResult shouldSaveChanges = _dialogService.ShowMessageBox(
                messageBoxCaption: "Unsaved Changes",
                messageBoxText: "Do you want to save changes before quitting?",
                messageBoxButton: MessageBoxButton.YesNoCancel,
                messageBoxImage: MessageBoxImage.Warning);

            if (shouldSaveChanges == MessageBoxResult.Yes)
            {
                return PerformSave();
            }
            else if (shouldSaveChanges == MessageBoxResult.No)
            {
                return true;
            }

            return false;
        }

        return true;
    }

    /// <summary>
    /// Reusable method to perform the save operation.
    /// </summary>
    /// <returns>true, if successfully saved</returns>
    private bool PerformSave()
    {
        SaveFileDialog d = new SaveFileDialog
        {
            Filter = "JSON Layout (*.json)|*.json",
            DefaultExt = "json",
            FileName = $"{Name}.json"
        };

        if (d.ShowDialog() == true)
        {
            _fileStorageService.Save(d.FileName, Items);
            
            UndoRedoReset(); // Reset Undo-Redo after saving?

            return true;
        }

        return false;
    }

    /// <summary>
    /// Command to load a layout from a JSON file.
    /// </summary>
    [RelayCommand]
    private void NewDocument()
    {
        if(CanClose())
        {
            _logger.LogDebug("Creating new layout.");

            Items = new ObservableCollection<FurnitureItem>();

            UndoRedoReset();
        }
    }

    /// <summary>
    /// Command to load a layout from a JSON file.
    /// </summary>
    [RelayCommand]
    private void LoadDocument()
    {
        if (CanClose())
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = "JSON Layout (*.json)|*.json", DefaultExt = "json" };

            if (dialog.ShowDialog() == true)
            {
                _logger.LogDebug("Loading layout from file: {FileName}", dialog.FileName);

                try
                {
                    var loadedDoc = _fileStorageService.Load(dialog.FileName);

                    Items = loadedDoc ?? new();

                    UndoRedoReset();
                }
                catch (Exception ex)
                {
                    _dialogService.ShowMessageBox(
                        messageBoxCaption: "An Error occured",
                        messageBoxText: $"Error: {ex.Message}",
                        messageBoxButton: MessageBoxButton.OK,
                        messageBoxImage: MessageBoxImage.Error);
                }
            }
        }
    }

    /// <summary>
    /// Command to save the current document to a JSON file.
    /// </summary>
    [RelayCommand]
    private void SaveDocument()
    {
        PerformSave();
    }

    private void UndoRedoReset()
    {
        _undoRedoManager.Reset();

        UpdateCommandStates();
    }
}