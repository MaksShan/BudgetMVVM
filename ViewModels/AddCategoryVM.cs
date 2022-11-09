using System.Windows.Input;
using BudgetMVVM.Data;
using BudgetMVVM.Infrastructure.Commands;
using BudgetMVVM.ViewModels.Base;
using BudgetMVVM.Service;

namespace BudgetMVVM.ViewModels;

internal class AddCategoryVM : BaseViewModel
{
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
            DbRepository.Create(_categoryName);
            App.CurrentWindow.Close();
        }
    }

    private bool CanAddCategoryCommandExecute(object p) => true;

    public AddCategoryVM()
    {
        AddCategoryCommand = new LambdaCommand(OnAddCategoryCommandExecuted, CanAddCategoryCommandExecute);
    }
}