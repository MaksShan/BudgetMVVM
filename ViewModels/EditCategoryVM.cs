using System.Windows.Input;
using BudgetMVVM.Data;
using BudgetMVVM.Infrastructure.Commands;
using BudgetMvvm.Models;
using BudgetMVVM.Service;
using BudgetMVVM.ViewModels.Base;

namespace BudgetMVVM.ViewModels;

internal class EditCategoryVM : BaseViewModel
{
    private Category SelectedCategory { get; }

    private string? _categoryName;
    public string? CategoryName
    {
        get => _categoryName;
        set => Set(ref _categoryName, value);
    }

    public ICommand AddCategoryCommand { get; }
    private void OnAddCategoryCommandExecuted(object p)
    {
        if (string.IsNullOrEmpty(_categoryName))
        {
            EmptyFieldMarker.SetRedFieldControl(App.CurrentWindow, "CategoryNameField");
        }
        else
        {
            DbRepository.Edit(SelectedCategory, _categoryName);
            App.CurrentWindow.Close();
        }
    }
    private bool CanAddCategoryCommandExecute(object p) => true;

    public EditCategoryVM(Category selectedCategory)
    {
        SelectedCategory = selectedCategory;

        AddCategoryCommand = new LambdaCommand(OnAddCategoryCommandExecuted, CanAddCategoryCommandExecute);
    }
}