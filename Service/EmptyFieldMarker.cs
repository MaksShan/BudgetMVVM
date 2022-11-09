using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BudgetMVVM.Service;

public static class EmptyFieldMarker
{
    public static void SetRedFieldControl(Window currentWindow, string fieldName)
    {
        if (currentWindow.Content is UserControl userControl)
        {
            Control? block = userControl.FindName(fieldName) as Control;

            if (block != null)
            {
                block.BorderBrush = Brushes.Red;
            }
        }
    }
}