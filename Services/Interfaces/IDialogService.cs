using System.Windows;
using BudgetMVVM.ViewModels.Base;

namespace BudgetMVVM.Services.Interfaces
{
    public interface IDialogService
    {
       void ShowDialog(FrameworkElement content, BaseViewModel vm);
    }
}
