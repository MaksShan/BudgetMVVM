using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using BudgetMVVM.Data;
using BudgetMVVM.Infrastructure.Commands;
using BudgetMvvm.Models;
using BudgetMVVM.Service;
using BudgetMVVM.Services;
using BudgetMVVM.ViewModels.Base;
using BudgetMVVM.Views;

namespace BudgetMVVM.ViewModels;

internal class CategoryListViewVM : BaseViewModel
{
    #region Properties
    private Dictionary<Category, int> _categoriesOperationsAmountDictionary;
    public Dictionary<Category, int> CategoriesOperationsAmountDictionary
    {
        get => _categoriesOperationsAmountDictionary;
        set => Set(ref _categoriesOperationsAmountDictionary, value);
    }

    private KeyValuePair<Category, int> _selectedCategory;
    public KeyValuePair<Category, int> SelectedCategory
    {
        get => _selectedCategory;
        set => Set(ref _selectedCategory, value);
    }
    #endregion

    #region Commands
    public ICommand DeleteCategoryCommand { get; }
    private bool CanDeleteCategoryCommandExecute(object obj) => true;
    private void OnDeleteCategoryCommandExecuted(object obj)
    {
        DbRepository.Remove(_selectedCategory.Key);
    }

    public ICommand OpenEditDialogCommand { get; }
    private bool CanOpenEditDialogCommandExecute(object obj) => true;
    private void OnOpenEditDialogCommandExecuted(object obj)
    {
        var dialog = new DialogService();
        var dialogContent = new AddCategoryView();
        var dialogVm = new EditCategoryVM(_selectedCategory.Key);

        dialog.ShowDialog(dialogContent, dialogVm);
    }
    #endregion

    private void OnUpdateCategories()
    {
        _categoriesOperationsAmountDictionary = StatisticsInfo.GetCategoryOperationsAmountDictionary();
        OnPropertyChanged(nameof(CategoriesOperationsAmountDictionary));
    }

    public static async Task<CategoryListViewVM> BuildCategoryListViewVmAsync()
    {
        ObservableCollection<Category> categories = await Task.Run(DbRepository.GetCategories);

        return new CategoryListViewVM(categories);
    }

    private CategoryListViewVM(ObservableCollection<Category> categories)
    {
        DbRepository.CategoriesChanged += OnUpdateCategories;

        _categoriesOperationsAmountDictionary = StatisticsInfo.GetCategoryOperationsAmountDictionary();

        DeleteCategoryCommand = new LambdaCommand(OnDeleteCategoryCommandExecuted, CanDeleteCategoryCommandExecute);
        OpenEditDialogCommand = new LambdaCommand(OnOpenEditDialogCommandExecuted, CanOpenEditDialogCommandExecute);
    }
}