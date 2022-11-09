using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BudgetMVVM.Data;
using BudgetMVVM.Infrastructure.Commands;
using BudgetMvvm.Models;
using BudgetMVVM.Services;
using BudgetMVVM.ViewModels.Base;
using BudgetMVVM.Views;
using BudgetMVVM.Service;

namespace BudgetMVVM.ViewModels;

internal class AddOperationVM : BaseViewModel
{
    #region Fields
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
    public ICommand AddItemCommand { get; }
    private bool CanAddItemExecute(object p) => true;
    private void OnAddItemCommandExecuted(object p)
    {
        if (_selectedCategory == null)
        {
            EmptyFieldMarker.SetRedFieldControl(App.CurrentWindow, "CategoryField");
        }
        else
        {
            DbRepository.Create(_operationType, _selectedCategory, _amount, _date, _description);
            App.CurrentWindow.Close();
        }
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

    public ICommand CallCategoryListWindowCommand { get; }
    private bool CanCallCategoryListWindowCommandExecute(object p) => true;
    private async void OnCallCategoryListWindowCommandExecuted(object p)
    {
        var dialog = new DialogService();

        var dialogContent = new CategoriesListView();
        var dialogVm = await CategoryListViewVM.BuildCategoryListViewVmAsync();

        dialog.ShowDialog(dialogContent, dialogVm);
    }
    #endregion

    private void OnCategoryChanged()
    {
        _categories = DbRepository.GetCategories();
        OnPropertyChanged(nameof(Categories));
    }

    public AddOperationVM()
    {
        DbRepository.CategoriesChanged += OnCategoryChanged;

        _date = DateTime.Now;
        _categories = DbRepository.GetCategories();

        AddItemCommand = new LambdaCommand(OnAddItemCommandExecuted, CanAddItemExecute);
        CallCategoryWindowCommand = new LambdaCommand(OnCallCategoryWindowCommandExecuted, CanCallCategoryWindowCommandExecute);
        CallCategoryListWindowCommand = new LambdaCommand(OnCallCategoryListWindowCommandExecuted, CanCallCategoryListWindowCommandExecute);
    }
}