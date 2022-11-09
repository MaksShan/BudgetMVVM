using System;
using BudgetMvvm.Models;

namespace BudgetMVVM.Infrastructure.Converters;

public static class ExpenseValueConverter
{
    public static decimal CheckExpenseValue(decimal value, OperationType type)
    {
        if (type == OperationType.Expense)
        {
            return value > 0 ? -value : value;
        }

        return value < 0 ? Math.Abs(value) : value;
    }
}