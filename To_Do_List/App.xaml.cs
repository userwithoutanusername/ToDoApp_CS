using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using To_Do_List.Views;
using To_Do_List.ViewModels;


namespace To_Do_List
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var databaseService = new DatabaseService();
            databaseService.InitializeDatabase();

            var mainWindow = new MainWindow();
            var viewModel = new ViewModel();
            mainWindow.DataContext = viewModel;
            mainWindow.Show();
        }
    }
}
