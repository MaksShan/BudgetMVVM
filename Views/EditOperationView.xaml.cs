﻿using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace BudgetMVVM.Views
{
    public partial class EditOperationView : UserControl
    {
        public EditOperationView()
        {
            InitializeComponent();
        }

        private void AmountTb_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
