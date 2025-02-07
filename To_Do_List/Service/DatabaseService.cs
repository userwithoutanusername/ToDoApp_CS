using System;
using System.Collections.Generic;
using System.Data.SQLite;
using To_Do_List.Models;

public class DatabaseService
{
    // Konstantní řetězec pro připojení k databázi SQLite
    private const string ConnectionString = "Data Source=tasks.db;Version=3;";

    // Konstruktor třídy DatabaseService
    public DatabaseService()
    {
        InitializeDatabase(); // Inicializace databáze při vytváření instance třídy
    }

    // Metoda pro inicializaci databáze a vytváření tabulek, pokud neexistují
    public void InitializeDatabase()
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open(); // Otevření spojení s databází

            // SQL příkaz pro vytváření tabulky kategorií
            string createCategoryTable = @"CREATE TABLE IF NOT EXISTS Categories (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Name TEXT NOT NULL)";

            // SQL příkaz pro vytváření tabulky úkolů
            string createTaskTable = @"CREATE TABLE IF NOT EXISTS Tasks (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Title TEXT NOT NULL,
                                    Description TEXT,
                                    IsCompleted INTEGER,
                                    CreationDate DATETIME,
                                    Deadline DATETIME,  
                                    Priority INTEGER,
                                    CategoryId INTEGER,
                                    FOREIGN KEY(CategoryId) REFERENCES Categories(Id))";

            var command = new SQLiteCommand(createCategoryTable, connection);
            command.ExecuteNonQuery(); // Vytvoření tabulky kategorií
            command.CommandText = createTaskTable;
            command.ExecuteNonQuery(); // Vytvoření tabulky úkolů
        }
    }

    // Metoda pro získání seznamu kategorií z databáze
    public List<Category> GetCategories()
    {
        var categories = new List<Category>(); // Seznam pro uložení kategorií
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open(); // Otevření spojení s databází
            var command = new SQLiteCommand("SELECT * FROM Categories", connection);
            var reader = command.ExecuteReader(); // Načtení dat z tabulky

            while (reader.Read()) // Procházení výsledků dotazu
            {
                categories.Add(new Category
                {
                    Id = reader.GetInt32(0), // Načtení ID kategorie
                    Name = reader.GetString(1) // Načtení názvu kategorie
                });
            }
        }
        return categories; // Vrácení seznamu kategorií
    }

    // Metoda pro získání úkolů pro konkrétní kategorii
    public List<TaskItem> GetTasksForCategory(int categoryId)
    {
        var tasks = new List<TaskItem>(); // Seznam pro uložení úkolů
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = new SQLiteCommand("SELECT * FROM Tasks WHERE CategoryId = @CategoryId", connection);
            command.Parameters.AddWithValue("@CategoryId", categoryId);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                tasks.Add(new TaskItem
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    IsCompleted = reader.GetInt32(3) == 1,
                    CreationDate = reader.GetDateTime(4),
                    Deadline = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                    Priority = (PriorityLevel)reader.GetInt32(6),
                    CategoryId = reader.GetInt32(7)
                });
            }
        }
        return tasks; // Vrácení seznamu úkolů
    }

    // Metoda pro přidání nové kategorie do databáze
    public void AddCategory(Category category)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = new SQLiteCommand("INSERT INTO Categories (Name) VALUES (@Name)", connection);
            command.Parameters.AddWithValue("@Name", category.Name);
            command.ExecuteNonQuery(); // Vložení kategorie do tabulky
        }
    }

    // Metoda pro přidání nového úkolu do databáze
    public void AddTask(TaskItem task)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = new SQLiteCommand("INSERT INTO Tasks (Title, Description, IsCompleted, CreationDate, Deadline, Priority, CategoryId) VALUES (@Title, @Description, @IsCompleted, @CreationDate, @Deadline, @Priority, @CategoryId)", connection);
            command.Parameters.AddWithValue("@Title", task.Title);
            command.Parameters.AddWithValue("@Description", task.Description);
            command.Parameters.AddWithValue("@IsCompleted", task.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@CreationDate", task.CreationDate);
            command.Parameters.AddWithValue("@Deadline", task.Deadline ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Priority", (int)task.Priority);
            command.Parameters.AddWithValue("@CategoryId", task.CategoryId);
            command.ExecuteNonQuery(); // Vložení úkolu do tabulky
        }
    }

    // Metoda pro aktualizaci existujícího úkolu v databázi
    public void UpdateTask(TaskItem task)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = new SQLiteCommand("UPDATE Tasks SET Title = @Title, Description = @Description, Priority = @Priority, IsCompleted = @IsCompleted, Deadline = @Deadline WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Title", task.Title);
            command.Parameters.AddWithValue("@Description", task.Description);
            command.Parameters.AddWithValue("@Priority", (int)task.Priority);
            command.Parameters.AddWithValue("@IsCompleted", task.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@Deadline", task.Deadline ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Id", task.Id);
            command.ExecuteNonQuery(); // Aktualizace úkolu v databázi
        }
    }

    // Metoda pro aktualizaci existující kategorie v databázi
    public void UpdateCategory(Category category)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = new SQLiteCommand("UPDATE Categories SET Name = @Name WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Name", category.Name);
            command.Parameters.AddWithValue("@Id", category.Id);
            command.ExecuteNonQuery(); // Aktualizace kategorie v databázi
        }
    }

    // Metoda pro smazání kategorie a všech jejích úkolů
    public void DeleteCategory(int categoryId)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = new SQLiteCommand("DELETE FROM Tasks WHERE CategoryId = @CategoryId", connection);
            command.Parameters.AddWithValue("@CategoryId", categoryId);
            command.ExecuteNonQuery(); // Smazání úkolů spojených s kategorií

            command.CommandText = "DELETE FROM Categories WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", categoryId);
            command.ExecuteNonQuery(); // Smazání kategorie z databáze
        }
    }

    // Metoda pro smazání konkrétního úkolu z databáze
    public void DeleteTask(int taskId)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = new SQLiteCommand("DELETE FROM Tasks WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", taskId);
            command.ExecuteNonQuery(); // Smazání úkolu z databáze
        }
    }
}
