using System.Linq;
using System.Windows;

namespace BudgetMVVM
{
    public partial class App
    {
        public static Window ActiveWindow => Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.IsActive);

        public static Window FocusedWindow => Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.IsFocused);

        public static Window CurrentWindow => FocusedWindow ?? ActiveWindow;
    }
}