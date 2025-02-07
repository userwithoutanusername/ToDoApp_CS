using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace To_Do_List.Views
{
    // Okno pro přidání nové kategorie
    public partial class AddCategoryWindow : Window
    {
        // Vlastnost pro uchování názvu kategorie
        public string CategoryName { get; private set; }

        // Konstruktor inicializuje komponenty okna
        public AddCategoryWindow()
        {
            InitializeComponent();
        }

        // Metoda pro zpracování události kliknutí na tlačítko "Add"
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // Kontrola, zda je textové pole prázdné
            if (string.IsNullOrWhiteSpace(CategoryNameTextBox.Text))
            {
                // Zobrazení varování, pokud pole není vyplněno
                ShowValidationError("Category name cannot be empty.");
                return;
            }

            // Uložení názvu kategorie do vlastnosti
            CategoryName = CategoryNameTextBox.Text;
            DialogResult = true; // Nastavení výsledku dialogu na úspěch
            this.Close(); // Uzavření okna
        }

        // Metoda pro zobrazení chybové zprávy
        private void ShowValidationError(string message)
        {
            var errorWindow = new ValidationErrorWindow(message); // Vytvoření nového okna pro chybu
            errorWindow.ShowDialog(); // Zobrazení chybového okna
        }

        // Metoda pro zpracování události kliknutí na tlačítko "Cancel"
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Nastavení výsledku dialogu na zrušení
            this.Close(); // Uzavření okna
        }
    }
}
