using System.Threading.Tasks;
using BudgetMVVM.Data;
using BudgetMvvm.Models;
using BudgetMVVM.Service;
using BudgetMVVM.ViewModels.Base;

namespace BudgetMVVM.ViewModels;

public class DashboardViewVM : BaseViewModel
{
    #region Properties
    private decimal _balance;
    public decimal Balance
    {
        get => _balance;
        set => Set(ref _balance, value);
    }

    private decimal _totalIncome;
    public decimal TotalIncome
    {
        get => _totalIncome;
        set => Set(ref _totalIncome, value);
    }

    private decimal _totalExpense;
    public decimal TotalExpense
    {
        get => _totalExpense;
        set => Set(ref _totalExpense, value);
    }

    private decimal _maximumIncome;
    public decimal MaximumIncome
    {
        get => _maximumIncome;
        set => Set(ref _maximumIncome, value);
    }

    private decimal _maximumExpense;
    public decimal MaximumExpense
    {
        get => _maximumExpense;
        set => Set(ref _maximumExpense, value);
    }

    private decimal _minimumIncome;
    public decimal MinimumIncome
    {
        get => _minimumIncome;
        set => Set(ref _minimumIncome, value);
    }

    private decimal _minimumExpense;
    public decimal MinimumExpense
    {
        get => _minimumExpense;
        set => Set(ref _minimumExpense, value);
    }

    private string? _bestIncomeCategoryName;
    public string? BestIncomeCategoryName
    {
        get => _bestIncomeCategoryName;
        set => Set(ref _bestIncomeCategoryName, value);
    }

    private string? _bestExpenseCategoryName;
    public string? BestExpenseCategoryName
    {
        get => _bestExpenseCategoryName;
        set => Set(ref _bestExpenseCategoryName, value);
    }
    #endregion

    private static async Task<(decimal, decimal, decimal, decimal, decimal, decimal, decimal, string, string)> GetDataParallelAsync()
    {
        var balanceTask = StatisticsInfo.GetBalanceTask();
        var totalIncomeTask = StatisticsInfo.GetTotalTask(OperationType.Income);
        var totalExpenseTask = StatisticsInfo.GetTotalTask(OperationType.Expense);
        var maximumIncomeTask = StatisticsInfo.GetMaximumTask(OperationType.Income);
        var maximumExpenseTask = StatisticsInfo.GetMaximumTask(OperationType.Expense);
        var minimumIncomeTask = StatisticsInfo.GetMinimumTask(OperationType.Income);
        var minimumExpenseTask = StatisticsInfo.GetMinimumTask(OperationType.Expense);
        var bestCategoryNameIncomeTask = StatisticsInfo.GetBestCategoryNameTask(OperationType.Income);
        var bestCategoryNameExpenseTask = StatisticsInfo.GetBestCategoryNameTask(OperationType.Expense);

        await Task.WhenAll(balanceTask, totalIncomeTask, totalExpenseTask, maximumIncomeTask, maximumExpenseTask, minimumIncomeTask, minimumExpenseTask,
            bestCategoryNameIncomeTask, bestCategoryNameExpenseTask);

        return (balanceTask.Result, totalIncomeTask.Result, totalExpenseTask.Result, maximumIncomeTask.Result, maximumExpenseTask.Result,
            minimumIncomeTask.Result, minimumExpenseTask.Result, bestCategoryNameIncomeTask.Result, bestCategoryNameExpenseTask.Result);
    }

    private async void OnUpdateOperationsAsync()
    {
        var data = await GetDataParallelAsync();

        _balance = data.Item1;
        _totalIncome = data.Item2;
        _totalExpense = data.Item3;
        _maximumIncome = data.Item4;
        _maximumExpense = data.Item5;
        _minimumIncome = data.Item6;
        _minimumExpense = data.Item7;
        _bestIncomeCategoryName = data.Item8;
        _bestExpenseCategoryName = data.Item9;

        OnPropertyChanged(nameof(Balance));
        OnPropertyChanged(nameof(TotalIncome));
        OnPropertyChanged(nameof(TotalExpense));
        OnPropertyChanged(nameof(MaximumIncome));
        OnPropertyChanged(nameof(MaximumExpense));
        OnPropertyChanged(nameof(MinimumIncome));
        OnPropertyChanged(nameof(MinimumExpense));
        OnPropertyChanged(nameof(BestIncomeCategoryName));
        OnPropertyChanged(nameof(BestExpenseCategoryName));
    }

    public static async Task<DashboardViewVM> BuildMainViewViewModelAsync()
    {
        await StatisticsInfo.BuildDashboardInfoAsync();

        var data = await GetDataParallelAsync();

        return new DashboardViewVM(data.Item1, data.Item2, data.Item3,
            data.Item4, data.Item5, data.Item6,
            data.Item7, data.Item8, data.Item9);
    }

    private DashboardViewVM(decimal balance, decimal totalIncome, decimal totalExpense, decimal maximumIncome, decimal maximumExpense,
        decimal minimumIncome, decimal minimumExpense, string bestIncomeCategoryName, string bestExpenseCategoryName)
    {
        DbRepository.OperationsChanged += OnUpdateOperationsAsync;

        _balance = balance;
        _totalIncome = totalIncome;
        _totalExpense = totalExpense;
        _maximumIncome = maximumIncome;
        _maximumExpense = maximumExpense;
        _minimumIncome = minimumIncome;
        _minimumExpense = minimumExpense;
        _bestIncomeCategoryName = bestIncomeCategoryName;
        _bestExpenseCategoryName = bestExpenseCategoryName;
    }
}