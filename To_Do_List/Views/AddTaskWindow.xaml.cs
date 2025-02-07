using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using To_Do_List.Models;
using To_Do_List.Views;

namespace To_Do_List.Views
{
    // Okno pro přidání nebo úpravu úkolu
    public partial class AddTaskWindow : Window
    {
        // Vlastnosti pro uchování dat úkolu
        public string TaskTitle { get; private set; } // Název úkolu
        public string TaskDescription { get; private set; } // Popis úkolu
        public PriorityLevel TaskPriority { get; private set; } // Priorita úkolu
        public DateTime? TaskDeadline { get; private set; }  // Datum a čas dokončení úkolu
        public bool IsReadOnly { get; set; } // Režim pouze pro čtení
        public bool IsPriorityEnabled => !IsReadOnly; // Zajišťuje dostupnost ComboBoxu pro prioritu
        public Visibility AddButtonVisibility => IsReadOnly ? Visibility.Collapsed : Visibility.Visible; // Skrytí tlačítka pro přidání v režimu čtení

        // Konstruktor okna pro přidání nebo úpravu úkolu
        public AddTaskWindow(TaskItem task = null, bool isReadOnly = false)
        {
            InitializeComponent();
            DataContext = this;

            if (task != null)
            {
                // Načtení dat úkolu do odpovídajících polí
                TaskTitleTextBox.Text = task.Title; // Název úkolu
                TaskDescriptionTextBox.Text = task.Description; // Popis úkolu
                PriorityComboBox.SelectedIndex = (int)task.Priority; // Nastavení priority úkolu

                if (task.Deadline.HasValue)
                {
                    // Načtení data a času do příslušných prvků UI
                    DeadlineDatePicker.SelectedDate = task.Deadline.Value.Date; // Datum deadline
                    DeadlineTimeTextBox.Text = task.Deadline.Value.ToString("HH:mm"); // Čas deadline ve formátu HH:mm
                }

                if (isReadOnly)
                {
                    // Nastavení UI do režimu pouze pro čtení
                    TaskTitleTextBox.IsReadOnly = true;
                    TaskDescriptionTextBox.IsReadOnly = true;
                    PriorityComboBox.IsEnabled = false;
                    DeadlineDatePicker.IsEnabled = false;
                    DeadlineTimeTextBox.IsReadOnly = true;
                    AddButton.Visibility = Visibility.Collapsed; // Skrytí tlačítka přidání
                }
            }
        }

        // Metoda pro potvrzení a přidání úkolu
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // Kontrola povinných polí
            if (string.IsNullOrWhiteSpace(TaskTitleTextBox.Text))
            {
                ShowValidationError("Task title cannot be empty."); // Kontrola názvu úkolu
                return;
            }

            if (string.IsNullOrWhiteSpace(TaskDescriptionTextBox.Text))
            {
                ShowValidationError("Task description cannot be empty."); // Kontrola popisu úkolu
                return;
            }

            if (PriorityComboBox.SelectedIndex < 0)
            {
                ShowValidationError("Please select a priority."); // Kontrola výběru priority
                return;
            }

            if (!DeadlineDatePicker.SelectedDate.HasValue)
            {
                ShowValidationError("Please select a deadline date."); // Kontrola výběru data
                return;
            }

            if (string.IsNullOrWhiteSpace(DeadlineTimeTextBox.Text))
            {
                ShowValidationError("Please enter a valid time for the deadline."); // Kontrola času deadline
                return;
            }

            // Zpracování kombinace data a času
            DateTime date = DeadlineDatePicker.SelectedDate.Value;
            if (!TimeSpan.TryParse(DeadlineTimeTextBox.Text, out TimeSpan time))
            {
                ShowValidationError("Invalid time format for deadline. Use HH:mm."); // Kontrola formátu času
                return;
            }

            // Uložení hodnot úkolu po úspěšné validaci
            TaskTitle = TaskTitleTextBox.Text; // Název úkolu
            TaskDescription = TaskDescriptionTextBox.Text; // Popis úkolu

            switch (PriorityComboBox.SelectedIndex)
            {
                case 0:
                    TaskPriority = PriorityLevel.Low; // Nízká priorita
                    break;
                case 1:
                    TaskPriority = PriorityLevel.Medium; // Střední priorita
                    break;
                case 2:
                    TaskPriority = PriorityLevel.High; // Vysoká priorita
                    break;
            }

            TaskDeadline = date + time; // Kombinace data a času pro deadline

            DialogResult = true; // Uzavření okna s výsledkem
            this.Close();
        }

        // Zobrazení chybové zprávy pro validaci
        private void ShowValidationError(string message)
        {
            var errorWindow = new ValidationErrorWindow(message);
            errorWindow.ShowDialog();
        }

        // Zrušení přidávání nebo úpravy úkolu
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Nastavení výsledku na zrušení
            this.Close(); // Uzavření okna
        }
    }
}
