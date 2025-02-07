using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace To_Do_List
{
    public partial class ValidationErrorWindow : Window
    {
        public ValidationErrorWindow(string message)
        {
            InitializeComponent();
            MessageTextBlock.Text = message; 
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }
    }

}
