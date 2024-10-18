using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Color = System.ConsoleColor;

class Program
{
    // List to store all items
    static List<Item> items = new List<Item>();

    static void Main()
    {
        string filePath = "items.csv"; // Define your file path
        LoadItemsFromFile(filePath); // Load items at the start

        Print("\n>>> Track Your Money Console\n", Color.DarkCyan); // Greeting

        while (true)
        {
            DisplayMenu();
            string choice = Console.ReadLine();

            if (choice == "6") // Exit option
            {
                SaveItemsToFile(filePath); // Save items before exiting
                Print("\nExiting the program...");
                break;
            }

            HandleInput(choice); // Handle user input
        }
    }

    public static void Print(string message, Color color = Color.White)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ResetColor();
    }

    // Calculating and displaying the current balance
    public static decimal CalculateCurrentBalance()
    {
        decimal totalIncome = items.Where(i => i.IsIncome).Sum(i => i.Amount);
        decimal totalExpense = items.Where(i => !i.IsIncome).Sum(i => i.Amount);
        return totalIncome - totalExpense;
    }

    // Method to display menu options
    public static void DisplayMenu()
    {
        Print($"\nCurrent Balance: ${CalculateCurrentBalance():F2}\n", Color.Yellow); // Display current balance
        Print("\n>>> Choose an option:");
        Print("\n>>> ", Color.DarkCyan); Print("1) "); Print("Add income", Color.DarkYellow);
        Print("\n>>> ", Color.DarkCyan); Print("2) "); Print("Add expense", Color.DarkYellow);
        Print("\n>>> ", Color.DarkCyan); Print("3) "); Print("View items", Color.DarkYellow);
        Print("\n>>> ", Color.DarkCyan); Print("4) "); Print("Delete or edit item", Color.DarkYellow);
        Print("\n>>> ", Color.DarkCyan); Print("5) "); Print("Delete all items", Color.DarkYellow);
        Print("\n>>> ", Color.DarkCyan); Print("6) "); Print("Exit", Color.DarkYellow);
        Print("\nEnter your option number: ", Color.DarkGreen);
    }

    // Method to handle input choices
    public static void HandleInput(string choice)
    {
        switch (choice)
        {
            case "1":
                AddIncome();
                break;
            case "2":
                AddExpense();
                break;
            case "3":
                ViewItems();
                break;
            case "4":
                DeleteOrEditItem();
                break;
            case "5":
                DeleteAllItems();
                break;
            default:
                Print("\nInvalid choice. Please try again.\n", Color.Red);
                break;
        }
    }

    // Validating title to allow spaces
    public static bool IsValidTitle(string title)
    {
        return title.All(c => char.IsLetter(c) || c == ' '); // Allow letters and spaces
    }

    public static string GetValidatedTitle()
    {
        string title;
        do
        {
            Print("\nEnter title: ", Color.DarkYellow);
            title = Console.ReadLine();

            if (!IsValidTitle(title))
            {
                Print("Invalid title. Please use only letters (A-Z) and spaces.\n", Color.Red);
            }
        } while (!IsValidTitle(title));

        return title;
    }

    // Method to add income and validate characters
    public static void AddIncome()
    {
        string title = GetValidatedTitle();

        Print("\nEnter income amount: ", Color.DarkYellow);
        decimal amount;
        while (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
        {
            Print("Invalid amount. Please enter a valid positive number.\n", Color.Red);
        }

        Print("\nEnter income month: ", Color.DarkYellow);
        string month = ValidateMonthInput(Console.ReadLine());
        items.Add(new Item(title, amount, month, true));
        Print("\nIncome added successfully!", Color.DarkGreen);
    }

    // Method to add expense items
    public static void AddExpense()
    {
        string title = GetValidatedTitle();

        Print("\nEnter expense amount: ", Color.DarkYellow);
        decimal amount;
        while (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
        {
            Print("Invalid amount. Please enter a valid positive number.\n", Color.Red);
        }

        Print("\nEnter expense month: ", Color.DarkYellow);
        string month = ValidateMonthInput(Console.ReadLine());
        items.Add(new Item(title, amount, month, false));
        Print("\nExpense added successfully!", Color.DarkGreen);
    }

    public static void ViewItems()
    {
        if (items.Count == 0)
        {
            Print("\nNo items available.\n", Color.Yellow);
            return;
        }

        Print("\n>>>Choose a filter option:");
        Print("\n>>> "); Print("1) ", Color.DarkCyan); Print("View all items", Color.DarkYellow);
        Print("\n>>> "); Print("2) ", Color.DarkCyan); Print("View only income", Color.DarkYellow);
        Print("\n>>> "); Print("3) ", Color.DarkCyan); Print("View only expenses", Color.DarkYellow);
        Print("\nEnter your choice: ", Color.DarkGreen);

        string filterChoice = Console.ReadLine();
        IEnumerable<Item> filteredItems;

        switch (filterChoice)
        {
            case "1":
                filteredItems = items; // All items
                break;
            case "2":
                filteredItems = items.Where(i => i.IsIncome); // Only incomes
                break;
            case "3":
                filteredItems = items.Where(i => !i.IsIncome); // Only expenses
                break;
            default:
                Print("\nInvalid choice. Returning to menu.", Color.Red);
                return;
        }

        // Prompt for sorting options
        Print("\n>>>Choose a sorting option:");
        Print("\n>>> "); Print("1) ", Color.DarkCyan); Print("Sort by title", Color.DarkYellow);
        Print("\n>>> "); Print("2) ", Color.DarkCyan); Print("Sort by amount", Color.DarkYellow);
        Print("\n>>> "); Print("3) ", Color.DarkCyan); Print("Sort by month", Color.DarkYellow); // NEW OPTION
        Print("\nEnter your choice: ", Color.DarkGreen);

        string sortChoice = Console.ReadLine();
        bool ascending = true; // Default sorting order is ascending

        // Only ask for sorting order if not sorting by month
        if (sortChoice != "3")
        {
            Print("\nChoose sorting order: "); Print("(1) Ascending, (2) Descending: ", Color.DarkYellow);
            string sortOrder = Console.ReadLine();
            ascending = sortOrder == "1"; // If "1", sort ascending; else descending
        }

        switch (sortChoice)
        {
            case "1": // Sort by title
                filteredItems = ascending
                    ? filteredItems.OrderBy(i => i.Title)
                    : filteredItems.OrderByDescending(i => i.Title);
                break;
            case "2": // Sort by amount
                filteredItems = ascending
                    ? filteredItems.OrderBy(i => i.Amount)
                    : filteredItems.OrderByDescending(i => i.Amount);
                break;
            case "3": // Sort by month (new option)
                filteredItems = filteredItems.OrderBy(i => ConvertMonthToNumber(i.Month)); // Always ascending by month
                break;
            default:
                Print("\nInvalid sorting choice. Returning to menu.", Color.Red);
                return;
        }

        // Display filtered and sorted items
        Print("\nSorted Items:\n\n");
        Print("\tTitle\t\tAmount\t\tMonth:\n\n");
        decimal totalIncome = 0;
        decimal totalExpense = 0;

        foreach (var item in filteredItems)
        {
            Print($"\t- {item.Title.PadRight(15)} ${item.Amount:F2}\t({item.Month})\n", item.IsIncome ? Color.DarkGreen : Color.DarkYellow);
            // Sum income and expenses
            if (item.IsIncome)
            {
                totalIncome += item.Amount;
            }
            else
            {
                totalExpense += item.Amount;
            }
        }

        // Display totals
        Print($"\nTotal income: ${totalIncome:F2}", Color.DarkGreen);
        Print($"\nTotal expenses: ${totalExpense:F2}\n\n", Color.DarkYellow);
    }

    // Delete or edit items
    public static void DeleteOrEditItem()
    {
        while (true) // Add loop to allow returning to the main menu
        {
            Print("\nEnter the title of the item you want to delete or edit (or 'R' to return to the menu): ", Color.DarkGreen);
            string title = Console.ReadLine();

            if (title.Trim().ToUpper() == "R") // If user enters 'R', return to menu
            {
                return;
            }

            var matchingItems = items.Where(i => i.Title.Equals(title, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!matchingItems.Any())
            {
                Print("\nNo items found with that title. Try again or press 'R' to return to the menu.", Color.Red);
                continue;
            }

            Print($"\nFound {matchingItems.Count} matching item(s):\n", Color.Yellow);

            for (int i = 0; i < matchingItems.Count; i++)
            {
                var item = matchingItems[i];
                Print($"{i + 1}) {item.Title} - ${item.Amount:F2} ({item.Month})\n", Color.Yellow);
            }

            Print("\nEnter the number of the item you want to delete or edit (or 'R' to return to the menu): ", Color.DarkGreen);
            string input = Console.ReadLine();

            if (input.Trim().ToUpper() == "R")
            {
                return;
            }

            if (!int.TryParse(input, out int itemNumber) || itemNumber < 1 || itemNumber > matchingItems.Count)
            {
                Print("\nInvalid selection. Try again or press 'R' to return to the menu.", Color.Red);
                continue;
            }

            var selectedItem = matchingItems[itemNumber - 1];

            Print("\nChoose an action: ", Color.DarkGreen);
            Print("(1) Delete, (2) Edit, or 'R' to return to menu: ");
            string action = Console.ReadLine();

            if (action.Trim().ToUpper() == "R")
            {
                return;
            }

            if (action == "1") // Delete item
            {
                items.Remove(selectedItem);
                Print("\nItem deleted successfully!", Color.DarkGreen);
            }
            else if (action == "2") // Edit item
            {
                Print("\nEnter new title "); Print("(or press Enter to keep current): ", Color.Gray);
                string newTitle = Console.ReadLine();
                Print("\nEnter new amount "); Print("(or press Enter to keep current): ", Color.Gray);
                string amountInput = Console.ReadLine();
                Print("\nEnter new month "); Print("(or press Enter to keep current): ", Color.Gray);
                string newMonth = Console.ReadLine();

                if (!string.IsNullOrEmpty(newTitle) && IsValidTitle(newTitle))
                    selectedItem.Title = newTitle;
                if (decimal.TryParse(amountInput, out var newAmount) && newAmount > 0)
                    selectedItem.Amount = newAmount;
                if (!string.IsNullOrEmpty(newMonth))
                    selectedItem.Month = ValidateMonthInput(newMonth);

                Print("\nItem updated successfully!", Color.Green);
            }
            else
            {
                Print("\nInvalid choice.", Color.Red);
            }
        }
    }


    // Delete all items
    public static void DeleteAllItems()
    {
        Print("\nAre you sure you want to delete all items? (Y/N): ", Color.Red);
        if (Console.ReadLine().Trim().ToUpper() == "Y")
        {
            items.Clear();
            Print("\nAll items deleted successfully!", Color.DarkGreen);
        }
        else
        {
            Print("\nDeletion canceled.", Color.DarkYellow);
        }
    }

    public static string ValidateMonthInput(string inputMonth)
    {
        string[] validMonths = new[]
        {
            "January", "February", "March", "April", "May", "June", "July",
            "August", "September", "October", "November", "December"
        };

        while (!validMonths.Contains(inputMonth, StringComparer.OrdinalIgnoreCase))
        {
            Print("\nInvalid month. Please enter a valid month (e.g., January): ", Color.Red);
            inputMonth = Console.ReadLine();
        }

        return validMonths.First(m => m.Equals(inputMonth, StringComparison.OrdinalIgnoreCase));
    }

    public static int ConvertMonthToNumber(string month)
    {
        string[] validMonths = new[]
        {
            "January", "February", "March", "April", "May", "June", "July",
            "August", "September", "October", "November", "December"
        };

        return Array.IndexOf(validMonths, month) + 1; // Convert month name to its number
    }

    // Method to save items to a file
    public static void SaveItemsToFile(string filePath)
    {
        using (StreamWriter sw = new StreamWriter(filePath))
        {
            foreach (var item in items)
            {
                sw.WriteLine($"\"{item.Title}\",{item.Amount},{item.Month},{item.IsIncome}");
            }
        }

        Print("\nItems saved successfully!\n", Color.Green);
    }

    // Method to load items from a file
    public static void LoadItemsFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        using (StreamReader sr = new StreamReader(filePath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 4)
                {
                    string title = parts[0].Trim('"'); // Remove quotes from title
                    decimal amount = decimal.Parse(parts[1]);
                    string month = parts[2];
                    bool isIncome = bool.Parse(parts[3]);

                    items.Add(new Item(title, amount, month, isIncome));
                }
            }
        }
    }
}

// Item class for representing financial entries
public class Item
{
    public string Title { get; set; }
    public decimal Amount { get; set; }
    public string Month { get; set; }
    public bool IsIncome { get; set; }

    public Item(string title, decimal amount, string month, bool isIncome)
    {
        Title = title;
        Amount = amount;
        Month = month;
        IsIncome = isIncome;
    }
}
