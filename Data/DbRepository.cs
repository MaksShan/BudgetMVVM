using System;
using System.Collections.ObjectModel;
using System.Linq;
using BudgetMVVM.Infrastructure.Converters;
using BudgetMvvm.Models;

namespace BudgetMVVM.Data;

public static class DbRepository
{
    public static Action? CategoriesChanged;
    private static void OnCategoriesChanged() => CategoriesChanged?.Invoke();

    public static Action? OperationsChanged;
    private static void OnOperationsChanged() => OperationsChanged?.Invoke();

    public static bool Create(string name)
    {
        bool result;

        using (var context = new ApplicationContext())
        {
            bool isExists = context.Categories != null && context.Categories.Any(cat => cat.Name == name);

            if (isExists == false)
            {
                var newCategory = new Category() { Name = name };
                if (context.Categories != null) context.Categories.Add(newCategory);
            }

            context.SaveChanges();
            result = true;
        }

        OnCategoriesChanged();

        return result;
    }

    public static bool Create(OperationType type, Category category, decimal amount, DateTime date, string? description)
    {
        bool result;

        using (var context = new ApplicationContext())
        {
            bool isExists = context.Operations != null && context.Operations.Any(op =>
                op.Type == type && op.CategoryId == category.Id && op.Amount == amount && op.Date == date);

            if (isExists == false)
            {
                var newOperation = new Operation()
                {
                    Type = type,
                    CategoryId = category.Id,
                    Amount = ExpenseValueConverter.CheckExpenseValue(amount, type),
                    Date = date,
                    Description = description
                };

                if (context.Operations != null) context.Operations.Add(newOperation);
            }

            context.SaveChanges();
            result = true;
        }

        OnOperationsChanged();

        return result;
    }

    public static bool Edit(Category categoryToEdit, string newName)
    {
        bool result;

        using (var context = new ApplicationContext())
        {
            var category = context.Categories?.FirstOrDefault(cat => cat.Id == categoryToEdit.Id);

            if (category != null)
            {
                category.Name = newName;
            }

            context.SaveChanges();
            result = true;
        }

        OnCategoriesChanged();
        OnOperationsChanged();

        return result;
    }

    public static bool Edit(Operation operationToEdit, OperationType newType, Category? newCategory, decimal newAmount, DateTime newDate, string? newDescription)
    {
        bool result;

        using (var context = new ApplicationContext())
        {
            var operation = context.Operations?.FirstOrDefault(op => op.Id == operationToEdit.Id);

            if (operation != null)
            {
                operation.Type = newType;
                if (newCategory != null) operation.CategoryId = newCategory.Id;
                operation.Amount = ExpenseValueConverter.CheckExpenseValue(newAmount, newType);
                operation.Date = newDate;
                operation.Description = newDescription;
            }

            context.SaveChanges();
            result = true;
        }

        OnOperationsChanged();

        return result;
    }

    public static bool Remove(Category category)
    {
        bool result;

        using (var context = new ApplicationContext())
        {
            if (context.Categories != null) context.Categories.Remove(category);

            result = true;
            context.SaveChanges();
        }

        OnCategoriesChanged();
        OnOperationsChanged();

        return result;
    }

    public static bool Remove(Operation? operation)
    {
        bool result;

        using (var context = new ApplicationContext())
        {
            if (operation != null)
            {
                if (context.Operations != null)
                {
                    context.Operations.Remove(operation);
                }
            }

            result = true;
            context.SaveChanges();
        }

        OnOperationsChanged();

        return result;
    }

    public static ObservableCollection<Category> GetCategories()
    {
        ObservableCollection<Category> categories=new ObservableCollection<Category>();

        using (var context = new ApplicationContext())
        {
            if (context.Categories != null)
            {
                categories = new ObservableCollection<Category>(context.Categories.ToList());
            }
        }

        return categories;
    }

    public static Category? GetCategory(int id)
    {
        Category? category;

        using (var context = new ApplicationContext())
        {
            category = context.Categories?.FirstOrDefault(cat => cat.Id == id);
        }

        return category;
    }

    public static ObservableCollection<Operation> GetOperations()
    {
        ObservableCollection<Operation> operations=new ObservableCollection<Operation>();

        using (var context = new ApplicationContext())
        {
            var operationsList = context.Operations?.ToList();

            if (operationsList != null)
            {
                operations = new ObservableCollection<Operation>(operationsList);
            }
        }

        return operations;
    }
}