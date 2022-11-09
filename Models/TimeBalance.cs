using System;

namespace BudgetMvvm.Models;

public class TimeBalance
{
    public TimeSpan Time { get; set; }
    public decimal Value { get; set; }

    public TimeBalance(TimeSpan time, decimal value)
    {
        Time = time;
        Value = value;
    }
}