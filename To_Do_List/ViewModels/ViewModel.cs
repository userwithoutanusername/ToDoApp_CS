using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using To_Do_List.Models;
using To_Do_List.Views;
using To_Do_List.Commands;

namespace To_Do_List.ViewModels
{
    // Třída ViewModel poskytuje logiku pro správu dat a komunikaci mezi UI a databází
    public class ViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService; // Služba pro práci s databází

        // Kolekce kategorií a úkolů pro zobrazení v UI
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<TaskItem> Tasks { get; set; }

        // Aktuálně vybraná kategorie
        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory)); // Notifikace o změně hodnoty
                LoadTasksForCategory(); // Načtení úkolů pro vybranou kategorii
            }
        }

        // Aktuálně vybraný úkol
        private TaskItem _selectedTask;
        public TaskItem SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask)); // Notifikace o změně hodnoty
            }
        }

        // Příkazy pro interakce v UI
        public ICommand AddCategoryCommand { get; set; }
        public ICommand AddTaskCommand { get; set; }
        public ICommand CompleteTaskCommand { get; set; }
        public ICommand DeleteCategoryCommand { get; set; }
        public ICommand DeleteTaskCommand { get; set; }
        public ICommand EditCategoryCommand { get; set; }
        public ICommand EditTaskCommand { get; set; }
        public ICommand OpenTaskCommand { get; set; }

        // Konstruktor inicializuje databázi, kolekce a příkazy
        public ViewModel()
        {
            _databaseService = new DatabaseService();
            Categories = new ObservableCollection<Category>(_databaseService.GetCategories());
            Tasks = new ObservableCollection<TaskItem>();

            // Inicializace příkazů s odpovídajícími metodami
            AddCategoryCommand = new RelayCommand(AddCategory);
            AddTaskCommand = new RelayCommand(AddTask);
            CompleteTaskCommand = new RelayCommand<TaskItem>(CompleteTask);
            DeleteCategoryCommand = new RelayCommand(DeleteCategory);
            DeleteTaskCommand = new RelayCommand(DeleteTask);
            EditCategoryCommand = new RelayCommand(EditCategory);
            EditTaskCommand = new RelayCommand(EditTask);
            OpenTaskCommand = new RelayCommand<TaskItem>(OpenTask);
        }

        // Otevření okna pro zobrazení úkolu v režimu pouze pro čtení
        private void OpenTask(TaskItem task)
        {
            if (task != null)
            {
                var viewTaskWindow = new AddTaskWindow(task, isReadOnly: true);
                viewTaskWindow.ShowDialog();
            }
        }

        // Načtení úkolů pro vybranou kategorii
        private void LoadTasksForCategory()
        {
            if (SelectedCategory != null)
            {
                Tasks.Clear();
                var tasks = _databaseService.GetTasksForCategory(SelectedCategory.Id);
                foreach (var task in tasks)
                {
                    Tasks.Add(task);
                }
            }
        }

        // Přidání nové kategorie pomocí dialogového okna
        private void AddCategory()
        {
            var addCategoryWindow = new AddCategoryWindow();
            if (addCategoryWindow.ShowDialog() == true)
            {
                var newCategory = new Category
                {
                    Name = addCategoryWindow.CategoryName
                };

                _databaseService.AddCategory(newCategory);
                Categories.Clear();
                foreach (var category in _databaseService.GetCategories())
                {
                    Categories.Add(category);
                }
            }
        }

        // Přidání nového úkolu do vybrané kategorie
        private void AddTask()
        {
            if (SelectedCategory != null)
            {
                var addTaskWindow = new AddTaskWindow();
                if (addTaskWindow.ShowDialog() == true)
                {
                    var newTask = new TaskItem
                    {
                        Title = addTaskWindow.TaskTitle,
                        Description = addTaskWindow.TaskDescription,
                        Priority = addTaskWindow.TaskPriority,
                        CreationDate = DateTime.Now,
                        IsCompleted = false,
                        Deadline = addTaskWindow.TaskDeadline,
                        CategoryId = SelectedCategory.Id
                    };

                    _databaseService.AddTask(newTask);
                    LoadTasksForCategory(); // Aktualizace seznamu úkolů
                }
            }
        }

        // Označení úkolu jako dokončeného
        private void CompleteTask(TaskItem task)
        {
            if (task != null)
            {
                _databaseService.UpdateTask(task); // Aktualizace úkolu v databázi
            }
        }

        // Smazání vybrané kategorie a všech jejích úkolů
        private void DeleteCategory()
        {
            if (SelectedCategory != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete category '{SelectedCategory.Name}' and all its tasks?", "Delete Category", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _databaseService.DeleteCategory(SelectedCategory.Id);
                    Categories.Remove(SelectedCategory); // Odebrání kategorie z kolekce
                    Tasks.Clear(); // Vyčištění seznamu úkolů
                }
            }
        }

        // Smazání vybraného úkolu
        private void DeleteTask()
        {
            if (SelectedTask != null)
            {
                _databaseService.DeleteTask(SelectedTask.Id);
                Tasks.Remove(SelectedTask); // Odebrání úkolu z kolekce
            }
        }

        // Úprava vybrané kategorie
        private void EditCategory()
        {
            if (SelectedCategory != null)
            {
                var editCategoryWindow = new AddCategoryWindow
                {
                    Title = "Edit Category"
                };
                editCategoryWindow.CategoryNameTextBox.Text = SelectedCategory.Name;

                if (editCategoryWindow.ShowDialog() == true)
                {
                    SelectedCategory.Name = editCategoryWindow.CategoryName;
                    _databaseService.UpdateCategory(SelectedCategory);
                    LoadCategories(); // Aktualizace seznamu kategorií
                }
            }
        }

        // Úprava vybraného úkolu
        private void EditTask()
        {
            if (SelectedTask != null)
            {
                var editTaskWindow = new AddTaskWindow
                {
                    Title = "Edit Task"
                };
                editTaskWindow.TaskTitleTextBox.Text = SelectedTask.Title; // Zadaný název úkolu
                editTaskWindow.TaskDescriptionTextBox.Text = SelectedTask.Description; // Popis úkolu
                editTaskWindow.PriorityComboBox.SelectedIndex = (int)SelectedTask.Priority; // Nastavení priority

                // Načítání data a času úkolu
                if (SelectedTask.Deadline.HasValue)
                {
                    editTaskWindow.DeadlineDatePicker.SelectedDate = SelectedTask.Deadline.Value.Date; // Datum dříve nastaveného deadline
                    editTaskWindow.DeadlineTimeTextBox.Text = SelectedTask.Deadline.Value.ToString("HH:mm"); // Čas deadline ve formátu HH:mm
                }

                // Uložení aktualizovaných hodnot
                if (editTaskWindow.ShowDialog() == true)
                {
                    SelectedTask.Title = editTaskWindow.TaskTitle;
                    SelectedTask.Description = editTaskWindow.TaskDescription;
                    SelectedTask.Priority = editTaskWindow.TaskPriority;
                    SelectedTask.Deadline = editTaskWindow.TaskDeadline;
                    _databaseService.UpdateTask(SelectedTask);
                    LoadTasksForCategory(); // Aktualizace seznamu úkolů
                }
            }
        }

        // Načtení seznamu kategorií z databáze
        private void LoadCategories()
        {
            Categories.Clear();
            foreach (var category in _databaseService.GetCategories())
            {
                Categories.Add(category);
            }
        }
    }
}
