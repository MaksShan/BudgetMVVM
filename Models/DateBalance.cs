using System;

namespace BudgetMvvm.Models;

public class DateBalance
{
    public DateTime DateTime { get; set; }
    public decimal Value { get; set; }

    public DateBalance(DateTime dateTime, decimal value)
    {
        DateTime = dateTime;
        Value = value;
    }
}