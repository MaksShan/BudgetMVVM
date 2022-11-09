using System.Windows.Input;
using BudgetMVVM.Infrastructure.Commands;
using BudgetMVVM.Services;
using BudgetMVVM.ViewModels.Base;
using BudgetMVVM.Views;

namespace BudgetMVVM.ViewModels;

internal class MainWindowVM : BaseViewModel
{
    #region Properties
    private BaseViewModel? _currentViewModel;
    public BaseViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        private set => Set(ref _currentViewModel, value);
    }
    #endregion

    #region Commands
    public ICommand ShowDefaultViewCommand { get; }
    private bool CanShowDefaultViewCommandExecute(object p) => true;
    private async void OnShowDefaultViewCommandExecuted(object p)
    {
        CurrentViewModel ??= await DashboardViewVM.BuildMainViewViewModelAsync();
    }

    public ICommand ShowMainViewCommand { get; }
    private bool CanShowMainViewCommandExecute(object p) => true;
    private async void OnShowMainViewCommandExecuted(object p)
    {
        CurrentViewModel = await DashboardViewVM.BuildMainViewViewModelAsync();
    }

    public ICommand ShowHistoryViewCommand { get; }
    private bool CanShowHistoryViewCommandExecute(object p) => true;
    private async void OnShowHistoryViewCommandExecuted(object p)
    {
        CurrentViewModel = await HistoryViewVM.BuildHistoryViewModelAsync();
    }

    public ICommand ShowChartsViewCommand { get; }
    private bool CanShowChartsViewCommandExecute(object p) => true;
    private async void OnShowChartsViewCommandExecuted(object p)
    {
        CurrentViewModel = await ChartsViewVM.BuildChartsViewModelAsync();
    }

    public ICommand OpenDialogCommand { get; }
    private void OnOpenDialogCommandExecuted(object p)
    {
        var dialog = new DialogService();
        var dialogContent = new AddOperationView();
        var dialogVm = new AddOperationVM();

        dialog.ShowDialog(dialogContent, dialogVm);
    }
    private bool CanOpenDialogCommandExecute(object p) => true;
    #endregion

    public MainWindowVM()
    {
        _currentViewModel = new LoadingViewVM();

        ShowDefaultViewCommand = new LambdaCommand(OnShowDefaultViewCommandExecuted, CanShowDefaultViewCommandExecute);
        ShowMainViewCommand = new LambdaCommand(OnShowMainViewCommandExecuted, CanShowMainViewCommandExecute);
        ShowHistoryViewCommand = new LambdaCommand(OnShowHistoryViewCommandExecuted, CanShowHistoryViewCommandExecute);
        ShowChartsViewCommand = new LambdaCommand(OnShowChartsViewCommandExecuted, CanShowChartsViewCommandExecute);
        OpenDialogCommand = new LambdaCommand(OnOpenDialogCommandExecuted, CanOpenDialogCommandExecute);
    }
}