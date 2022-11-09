using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BudgetMVVM.Data;
using BudgetMVVM.Infrastructure.Commands;
using BudgetMvvm.Models;
using BudgetMVVM.Services;
using BudgetMVVM.ViewModels.Base;
using BudgetMVVM.Views;

namespace BudgetMVVM.ViewModels;

class EditOperationVM : BaseViewModel
{
    #region Fields
    private readonly Operation _operationToEdit;

    private OperationType _operationType;
    public OperationType OperationType
    {
        get => _operationType;
        set => Set(ref _operationType, value);
    }

    private ObservableCollection<Category> _categories;
    public ObservableCollection<Category> Categories
    {
        get => _categories;
        set => Set(ref _categories, value);
    }

    private Category? _selectedCategory;
    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set => Set(ref _selectedCategory, value);
    }

    private int _index;
    public int Index
    {
        get => _index;
        set => Set(ref _index, value);
    }

    private decimal _amount;
    public decimal Amount
    {
        get => _amount;
        set => Set(ref _amount, value);
    }

    private DateTime _date;
    public DateTime Date
    {
        get => _date;
        set => Set(ref _date, value);
    }

    private string? _description;
    public string? Description
    {
        get => _description;
        set => Set(ref _description, value);
    }
    #endregion

    #region Commands

    public ICommand EditOperationCommand { get; }
    private bool CanEditOperationCommandExecute(object obj) => true;
    private void OnEditOperationCommandExecuted(object obj)
    {
        DbRepository.Edit(_operationToEdit, _operationType, _selectedCategory, _amount, _date, _description);
        App.CurrentWindow.Close();
    }

    public ICommand CallCategoryWindowCommand { get; }
    private bool CanCallCategoryWindowCommandExecute(object p) => true;
    private void OnCallCategoryWindowCommandExecuted(object p)
    {
        var dialog = new DialogService();

        var dialogContent = new AddCategoryView();
        var dialogVm = new AddCategoryVM();

        dialog.ShowDialog(dialogContent, dialogVm);
    }

    #endregion
    public EditOperationVM(Operation operation)
    {
        _operationToEdit = operation;
        _categories = DbRepository.GetCategories();
        _selectedCategory = operation.Category;
        if (_selectedCategory != null) _index = _categories.IndexOf(_selectedCategory);
        _operationType = operation.Type;
        _amount = operation.Amount;
        _date = operation.Date;
        _description = operation.Description;

        EditOperationCommand = new LambdaCommand(OnEditOperationCommandExecuted, CanEditOperationCommandExecute);
        CallCategoryWindowCommand = new LambdaCommand(OnCallCategoryWindowCommandExecuted, CanCallCategoryWindowCommandExecute);
    }
}