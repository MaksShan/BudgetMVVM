using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using BudgetMVVM.Data;
using BudgetMVVM.Infrastructure.Commands;
using BudgetMvvm.Models;
using BudgetMVVM.Services;
using BudgetMVVM.ViewModels.Base;
using BudgetMVVM.Views;

namespace BudgetMVVM.ViewModels;

class HistoryViewVM : BaseViewModel
{
    #region Fields
    private ObservableCollection<Operation> _operations;
    public ObservableCollection<Operation> Operations
    {
        get => _operations;
        set => Set(ref _operations, value);
    }

    private Operation? _selectedOperation;
    public Operation? SelectedOperation
    {
        get => _selectedOperation;
        set => Set(ref _selectedOperation, value);
    }
    #endregion

    #region Commands
    public ICommand DeleteOperationCommand { get; }
    private bool CanDeleteOperationCommandExecute(object obj) => true;
    private void OnDeleteOperationCommandExecuted(object obj)
    {
        DbRepository.Remove(_selectedOperation);
    }

    public ICommand OpenEditDialogCommand { get; }
    private bool CanOpenEditDialogCommandExecute(object obj) => true;
    private void OnOpenEditDialogCommandExecuted(object obj)
    {
        var dialog = new DialogService();
        var dialogContent = new EditOperationView();

        if (_selectedOperation != null)
        {
            var dialogVm = new EditOperationVM(_selectedOperation);
            dialog.ShowDialog(dialogContent, dialogVm);
        }
    }
    #endregion

    private void OnUpdateOperations()
    {
        _operations = DbRepository.GetOperations();
        OnPropertyChanged(nameof(Operations));
    }

    public static async Task<HistoryViewVM> BuildHistoryViewModelAsync()
    {
        ObservableCollection<Operation> operations = await Task.Run(DbRepository.GetOperations);

        return new HistoryViewVM(operations);
    }

    private HistoryViewVM(ObservableCollection<Operation> operations)
    {
        DbRepository.OperationsChanged += OnUpdateOperations;

        _operations = operations;

        DeleteOperationCommand = new LambdaCommand(OnDeleteOperationCommandExecuted, CanDeleteOperationCommandExecute);
        OpenEditDialogCommand = new LambdaCommand(OnOpenEditDialogCommandExecuted, CanOpenEditDialogCommandExecute);
    }
}