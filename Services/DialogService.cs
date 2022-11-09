using System.Windows;
using BudgetMVVM.Services.Interfaces;
using BudgetMVVM.ViewModels.Base;
using BudgetMVVM.Views.Windows;

namespace BudgetMVVM.Services;

public class DialogService : IDialogService
{
    public void ShowDialog(FrameworkElement content, BaseViewModel vm)
    {
        var dialog = new DialogWindow();

        content.DataContext = vm;
        dialog.Content = content;

        dialog.ShowDialog();
    }
} 