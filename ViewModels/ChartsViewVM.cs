using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetMVVM.Data;
using BudgetMvvm.Models;
using BudgetMVVM.Service;
using BudgetMVVM.ViewModels.Base;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;

namespace BudgetMVVM.ViewModels;

class ChartsViewVM : BaseViewModel
{
    #region Properties
    #region Axes formatters
    private Func<double, string>? _commonYFormatter;
    public Func<double, string>? CommonYFormatter
    {
        get => _commonYFormatter;
        set => Set(ref _commonYFormatter, value);
    }

    private Func<double, string>? _dayXFormatter;
    public Func<double, string>? DayXFormatter
    {
        get => _dayXFormatter;
        set => Set(ref _dayXFormatter, value);
    }

    private Func<double, string>? _monthXFormatter;
    public Func<double, string>? MonthXFormatter
    {
        get => _monthXFormatter;
        set => Set(ref _monthXFormatter, value);
    }

    private Func<double, string>? _yearXFormatter;
    public Func<double, string>? YearXFormatter
    {
        get => _yearXFormatter;
        set => Set(ref _yearXFormatter, value);
    }
    #endregion

    #region Series collections
    private SeriesCollection _currentDayLineChartSeries;
    public SeriesCollection CurrentDayLineChartSeries
    {
        get => _currentDayLineChartSeries;
        set => Set(ref _currentDayLineChartSeries, value);
    }

    private SeriesCollection _currentMonthLineChartSeries;
    public SeriesCollection CurrentMonthLineChartSeries
    {
        get => _currentMonthLineChartSeries;
        set => Set(ref _currentMonthLineChartSeries, value);
    }

    private SeriesCollection _currentYearLineChartSeries;
    public SeriesCollection CurrentYearLineChartSeries
    {
        get => _currentYearLineChartSeries;
        set => Set(ref _currentYearLineChartSeries, value);
    }

    private SeriesCollection _incomePieChartSeries;
    public SeriesCollection IncomePieChartSeries
    {
        get => _incomePieChartSeries;
        set => Set(ref _incomePieChartSeries, value);
    }

    private SeriesCollection _expensePieChartSeries;
    public SeriesCollection ExpensePieChartSeries
    {
        get => _expensePieChartSeries;
        set => Set(ref _expensePieChartSeries, value);
    }
    #endregion

    #region Datasets
    private List<TimeBalance>? _currentDayBalance;
    public List<TimeBalance>? CurrentDayBalance
    {
        get => _currentDayBalance;
        set => Set(ref _currentDayBalance, value);
    }

    private List<DateBalance>? _currentMonthBalance;
    public List<DateBalance>? CurrentMonthBalance
    {
        get => _currentMonthBalance;
        set => Set(ref _currentMonthBalance, value);
    }

    private List<DateBalance>? _currentYearBalance;
    public List<DateBalance>? CurrentYearBalance
    {
        get => _currentYearBalance;
        set => Set(ref _currentYearBalance, value);
    }

    private Dictionary<Category, decimal> _incomeCategoriesDictionary;
    public Dictionary<Category, decimal> IncomeCategoriesDictionary
    {
        get => _incomeCategoriesDictionary;
        set => Set(ref _incomeCategoriesDictionary, value);
    }

    private Dictionary<Category, decimal> _expenseCategoriesDictionary;
    public Dictionary<Category, decimal> ExpenseCategoriesDictionary
    {
        get => _expenseCategoriesDictionary;
        set => Set(ref _expenseCategoriesDictionary, value);
    }
    #endregion
    #endregion

    #region Series Methods
    public SeriesCollection GetCurrentDayLineSeries()
    {
        var dayConfig = Mappers.Xy<TimeBalance>()
            .X(dateModel => dateModel.Time.Ticks / TimeSpan.FromMinutes(60).Ticks)
            .Y(dateModel => (double)dateModel.Value);

        var chartSeries = new SeriesCollection(dayConfig);

        var lineSeries = new LineSeries
        {
            LineSmoothness = 0.0,
            Title = "Balance",
            Values = new ChartValues<TimeBalance>(_currentDayBalance)
        };

        chartSeries.Add(lineSeries);

        return chartSeries;
    }

    public SeriesCollection GetCurrentMonthLineSeries()
    {
        var monthConfig = Mappers.Xy<DateBalance>()
            .X(dateModel => dateModel.DateTime.Ticks / TimeSpan.FromDays(1).Ticks)
            .Y(dateModel => (double)dateModel.Value);

        var chartSeries = new SeriesCollection(monthConfig);

        var lineSeries = new LineSeries
        {
            LineSmoothness = 0.0,
            Title = "Balance",
            Values = new ChartValues<DateBalance>(_currentMonthBalance)
        };

        chartSeries.Add(lineSeries);

        return chartSeries;
    }

    public SeriesCollection GetCurrentYearLineSeries()
    {
        var yearConfig = Mappers.Xy<DateBalance>()
            .X(dateModel => dateModel.DateTime.Ticks / (TimeSpan.FromDays(1).Ticks * 30.43))
            .Y(dateModel => (double)dateModel.Value);

        var chartSeries = new SeriesCollection(yearConfig);

        var lineSeries = new LineSeries
        {
            LineSmoothness = 0.0,
            Title = "Balance",
            Values = new ChartValues<DateBalance>(_currentYearBalance)
        };

        chartSeries.Add(lineSeries);

        return chartSeries;
    }

    public SeriesCollection GetPieSeries(OperationType type)
    {
        var chartSeries = new SeriesCollection();

        if (type == OperationType.Income)
        {
            foreach (var item in _incomeCategoriesDictionary)
            {
                var pieSeries = new PieSeries
                {
                    Title = item.Key.Name,
                    Values = new ChartValues<decimal> { item.Value },
                    DataLabels = true
                };

                chartSeries.Add(pieSeries);
            }
        }
        else
        {
            foreach (var item in _expenseCategoriesDictionary)
            {
                var series = new PieSeries
                {
                    Title = item.Key.Name,
                    Values = new ChartValues<decimal> { item.Value },
                    DataLabels = true
                };

                chartSeries.Add(series);
            }
        }

        return chartSeries;
    }
    #endregion

    private static async Task<(List<TimeBalance>, List<DateBalance>, List<DateBalance>, 
        Dictionary<Category, decimal>, Dictionary<Category, decimal>)> GetDataParallelAsync()
    {
        var currentDayBalanceTask = StatisticsInfo.GetCurrentDayBalanceTask();
        var currentMonthBalanceTask = StatisticsInfo.GetCurrentMonthBalanceTask();
        var currentYearBalanceTask = StatisticsInfo.GetCurrentYearBalanceTask();
        var incomeCategoriesDictionaryTask = StatisticsInfo.GetCategoryAmountTask(OperationType.Income);
        var expenseCategoriesDictionaryTask = StatisticsInfo.GetCategoryAmountTask(OperationType.Expense);

        await Task.WhenAll(currentDayBalanceTask, currentMonthBalanceTask, currentYearBalanceTask, 
            incomeCategoriesDictionaryTask, expenseCategoriesDictionaryTask);

        return (currentDayBalanceTask.Result, currentMonthBalanceTask.Result, currentYearBalanceTask.Result, 
            incomeCategoriesDictionaryTask.Result, expenseCategoriesDictionaryTask.Result);
    }

    private async void OnUpdateOperationsAsync()
    {
        var data = await GetDataParallelAsync();

        _currentDayBalance = data.Item1;
        _currentMonthBalance = data.Item2;
        _currentYearBalance = data.Item3;
        _incomeCategoriesDictionary = data.Item4;
        _expenseCategoriesDictionary = data.Item5;
        _currentDayLineChartSeries = GetCurrentDayLineSeries();
        _currentMonthLineChartSeries = GetCurrentMonthLineSeries();
        _currentYearLineChartSeries = GetCurrentYearLineSeries();
        _incomePieChartSeries = GetPieSeries(OperationType.Income);
        _expensePieChartSeries = GetPieSeries(OperationType.Expense);

        OnPropertyChanged(nameof(CurrentDayBalance));
        OnPropertyChanged(nameof(CurrentMonthBalance));
        OnPropertyChanged(nameof(CurrentYearBalance));
        OnPropertyChanged(nameof(IncomeCategoriesDictionary));
        OnPropertyChanged(nameof(ExpenseCategoriesDictionary));
        OnPropertyChanged(nameof(CurrentDayLineChartSeries));
        OnPropertyChanged(nameof(CurrentMonthLineChartSeries));
        OnPropertyChanged(nameof(CurrentYearLineChartSeries));
        OnPropertyChanged(nameof(IncomePieChartSeries));
        OnPropertyChanged(nameof(ExpensePieChartSeries));
    }

    public static async Task<ChartsViewVM> BuildChartsViewModelAsync()
    {
        var data = await GetDataParallelAsync();

        return new ChartsViewVM(data.Item1, data.Item2, data.Item3,
            data.Item4, data.Item5);
    }

    private ChartsViewVM(List<TimeBalance> currentDayBalance, List<DateBalance> currentMonthBalance,
        List<DateBalance> currentYearBalance, Dictionary<Category, decimal> incomeCategoriesDictionary,
        Dictionary<Category, decimal> expenseCategoriesDictionary)
    {
        DbRepository.OperationsChanged += OnUpdateOperationsAsync;

        _commonYFormatter = value => value.ToString("F");
        _dayXFormatter = value => new DateTime((long)(value * TimeSpan.FromMinutes(60).Ticks)).ToString("t");
        _monthXFormatter = value => new DateTime((long)(value * TimeSpan.FromDays(1).Ticks)).ToString("d");
        _yearXFormatter = value => new DateTime((long)(value * TimeSpan.FromDays(1).Ticks * 30.43)).ToString("MMM");

        _currentDayBalance = currentDayBalance;
        _currentMonthBalance = currentMonthBalance;
        _currentYearBalance = currentYearBalance;
        _incomeCategoriesDictionary = incomeCategoriesDictionary;
        _expenseCategoriesDictionary = expenseCategoriesDictionary;
        _currentDayLineChartSeries = GetCurrentDayLineSeries();
        _currentMonthLineChartSeries = GetCurrentMonthLineSeries();
        _currentYearLineChartSeries = GetCurrentYearLineSeries();
        _incomePieChartSeries = GetPieSeries(OperationType.Income);
        _expensePieChartSeries = GetPieSeries(OperationType.Expense);
    }
}