using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudgetMVVM.Data;
using BudgetMvvm.Models;

namespace BudgetMVVM.Service;

public class StatisticsInfo
{
    private static List<Operation>? Operations { get; set; }

    #region Dashboard information methods
    public static decimal GetBalance()
    {
        decimal balance = 0;

        if (Operations != null)
        {
            balance += Operations.Sum(operation => operation.Amount);
        }

        return balance;
    }

    public static Task<decimal> GetBalanceTask()
    {
        return Task.Run(GetBalance);
    }

    public static decimal GetBalanceTo(int? operationId)
    {
        decimal balance = 0;

        var orderedByDateOperations = Operations?.OrderBy(op => op.Date);

        if (orderedByDateOperations != null)
        {
            foreach (var operation in orderedByDateOperations)
            {
                if (operation.Id == operationId)
                {
                    break;
                }

                balance += operation.Amount;
            }
        }

        return balance;
    }

    public static decimal GetTotal(OperationType type)
    {
        decimal result = 0;

        if (Operations != null)
        {
            result = Operations.Where(op => op.Type == type).Sum(a => a.Amount);
        }

        return result;
    }

    public static Task<decimal> GetTotalTask(OperationType type)
    {
        return Task.Run(() => GetTotal(type));
    }

    public static decimal GetMaximum(OperationType type)
    {
        decimal result = 0;

        if (Operations != null)
        {
            var typedOperations = Operations.Where(op => op.Type == type).ToList();

            if (typedOperations.Count != 0)
            {
                result = type == OperationType.Income ? typedOperations.Max(a => a.Amount) : typedOperations.Min(a => a.Amount);
            }
        }

        return result;
    }

    public static Task<decimal> GetMaximumTask(OperationType type)
    {
        return Task.Run(() => GetMaximum(type));
    }

    public static decimal GetMinimum(OperationType type)
    {
        decimal result = 0;

        if (Operations != null)
        {
            var typedOperations = Operations.Where(op => op.Type == type).ToList();

            if (typedOperations.Count != 0)
            {
                result = type == OperationType.Income ? typedOperations.Min(a => a.Amount) : typedOperations.Max(a => a.Amount);
            }
        }

        return result;
    }

    public static Task<decimal> GetMinimumTask(OperationType type)
    {
        return Task.Run(() => GetMinimum(type));
    }

    public static string GetBestCategoryName(OperationType operationType)
    {
        var groupsByCategory = Operations?.FindAll(op => op.Type == operationType).GroupBy(op => op.Category);

        Category? bestCategory = default;
        decimal maxAmount = 0;

        if (groupsByCategory != null)
        {
            foreach (var group in groupsByCategory)
            {
                var currentAmount = group.Sum(g => g.Amount);

                if (group.Key != null)
                {
                    if (operationType == OperationType.Income)
                    {
                        if (currentAmount >= maxAmount)
                        {
                            maxAmount = currentAmount;
                            bestCategory = group.Key;
                        }
                    }
                    else
                    {
                        if (currentAmount <= maxAmount)
                        {
                            maxAmount = currentAmount;
                            bestCategory = group.Key;
                        }
                    }
                }
            }
        }

        if (bestCategory == null || bestCategory.Name==null)
        {
            return "";
        }

        return bestCategory.Name;
    }

    public static Task<string> GetBestCategoryNameTask(OperationType operationType)
    {
        return Task.Run(() => GetBestCategoryName(operationType));
    }
    #endregion

    #region Category operations method
    public static Dictionary<Category, int> GetCategoryOperationsAmountDictionary()
    {
        Dictionary<Category, int> categoryOperationsAmountDictionary = new Dictionary<Category, int>();

        var categories = DbRepository.GetCategories();

        int amount = default;

        foreach (var category in categories)
        {
            if (Operations != null)
            {
                foreach (var op in Operations)
                {
                    if (op.Category != null && op.Category.Equals(category))
                    {
                        amount++;
                    }
                }
            }

            categoryOperationsAmountDictionary.Add(category, amount);
            amount = 0;
        }

        return categoryOperationsAmountDictionary;
    }
    #endregion

    #region Charts information methods
    public static List<TimeBalance> GetCurrentDayBalance()
    {
        if (Operations != null)
        {
            var groupsByDate = Operations.Where(op => op.Date.Year == DateTime.Now.Year &&
                                                      op.Date.Month == DateTime.Now.Month && op.Date.Day == DateTime.Now.Day)
                .GroupBy(op => op.Date.Hour).OrderBy(gr => gr.Key);

            var initialOperationId = groupsByDate.FirstOrDefault()?.OrderBy(x => x.Date).FirstOrDefault()?.Id;

            var dailyBalance = new List<TimeBalance>();
            TimeSpan date = default;

            decimal currentAmount = GetBalanceTo(initialOperationId);

            foreach (var group in groupsByDate)
            {
                foreach (var operation in group)
                {
                    currentAmount += operation.Amount;
                    date = operation.Date.TimeOfDay;
                }

                dailyBalance.Add(new TimeBalance(date, currentAmount));
            }

            return dailyBalance;
        }

        return new List<TimeBalance>();
    }

    public static Task<List<TimeBalance>> GetCurrentDayBalanceTask()
    {
        return Task.Run(GetCurrentDayBalance);
    }

    public static List<DateBalance> GetCurrentMonthBalance()
    {
        if (Operations != null)
        {
            var groupsByDate = Operations.Where(op => op.Date.Month == DateTime.Now.Month && op.Date.Year == DateTime.Now.Year)
                .GroupBy(op => op.Date.Day).OrderBy(gr => gr.Key);

            var initialOperationId = groupsByDate.FirstOrDefault()?.OrderBy(x => x.Date).FirstOrDefault()?.Id;

            var dailyBalance = new List<DateBalance>();
            DateTime date = default;

            decimal currentAmount = GetBalanceTo(initialOperationId);

            foreach (var group in groupsByDate)
            {
                foreach (var operation in group)
                {
                    currentAmount += operation.Amount;
                    date = operation.Date;
                }

                dailyBalance.Add(new DateBalance(date, currentAmount));
            }

            return dailyBalance;
        }

        return new List<DateBalance>();
    }

    public static Task<List<DateBalance>> GetCurrentMonthBalanceTask()
    {
        return Task.Run(GetCurrentMonthBalance);
    }

    public static List<DateBalance> GetCurrentYearBalance()
    {
        if (Operations != null)
        {
            var groupsByDate = Operations.Where(op => op.Date.Year == DateTime.Now.Year)
                .GroupBy(op => op.Date.Month).OrderBy(gr => gr.Key);

            var initialOperationId= groupsByDate.FirstOrDefault()?.OrderBy(x => x.Date).FirstOrDefault()?.Id;

            var dailyBalance = new List<DateBalance>();
            DateTime date = default;

            decimal currentAmount = GetBalanceTo(initialOperationId);

            foreach (var group in groupsByDate)
            {
                foreach (var operation in group)
                {
                    currentAmount += operation.Amount;
                    date = operation.Date;
                }

                dailyBalance.Add(new DateBalance(date, currentAmount));
            }

            return dailyBalance;
        }

        return new List<DateBalance>();
    }

    public static Task<List<DateBalance>> GetCurrentYearBalanceTask()
    {
        return Task.Run(GetCurrentYearBalance);
    }

    public static Dictionary<Category, decimal> GetCategoryAmount(OperationType operationType)
    {
        var groupsByCategory = Operations?.FindAll(op => op.Type == operationType).GroupBy(op => op.Category);

        var categoryAmountDictionary = new Dictionary<Category, decimal>();
        decimal currentAmount = 0;

        if (groupsByCategory != null)
            foreach (var group in groupsByCategory)
            {
                foreach (var operation in group)
                {
                    currentAmount += operation.Amount;
                }

                if (group.Key != null)
                {
                    categoryAmountDictionary.Add(group.Key, currentAmount);
                }
            }

        return categoryAmountDictionary;
    }

    public static Task<Dictionary<Category, decimal>> GetCategoryAmountTask(OperationType operationType)
    {
        return Task.Run(() => GetCategoryAmount(operationType));
    }
    #endregion

    private void OnUpdateOperations()
    {
        Operations = DbRepository.GetOperations().ToList();
    }

    public static async Task<StatisticsInfo> BuildDashboardInfoAsync()
    {
        List<Operation> operations = await Task.Run(() => DbRepository.GetOperations().ToList());

        return new StatisticsInfo(operations);
    }

    private StatisticsInfo(List<Operation> operations)
    {
        DbRepository.OperationsChanged += OnUpdateOperations;

        Operations = operations;
    }
}