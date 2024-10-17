using System;
using System.Collections.Generic;
using System.Linq;
using Color = System.ConsoleColor;

class Program
{
    // List to store all items
    static List<Item> items = new List<Item>();

    static void Main()
    {
        Greeting();

        while (true)
        {
            DisplayMenu();
            string choice = Console.ReadLine(); // Read the input here and pass it to HandleInput

            if (choice == "5") // Check if user wants to exit before handling input
            {
                Print("\nExiting the program...");
                break;
            }

            HandleInput(choice); // Pass the user's input to HandleInput
        }
    }

    public static void Print(string message, Color color = Color.White)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message); // WriteLine to avoid staying on the same line
        Console.ResetColor();
    }
    public static decimal CalculateCurrentBalance()
    {
        decimal totalIncome = items.Where(i => i.IsIncome).Sum(i => i.Amount);
        decimal totalExpense = items.Where(i => !i.IsIncome).Sum(i => i.Amount);
        return totalIncome - totalExpense;
    }


    public static void Greeting()
    {
        Print("\n>>> Track Your Money Console\n", Color.DarkCyan);
    }

    public static void DisplayMenu()
    {
        Print($"\nCurrent Balance: ${CalculateCurrentBalance():F2}", Color.Yellow); // Display current balance

        Print("\n>>> Choose an option:");
        Print(">>>1) Add income", Color.Gray);
        Print(">>>2) Add expense", Color.Gray);
        Print(">>>3) View items", Color.Gray);
        Print(">>>4) Delete item", Color.Gray);
        Print(">>>5) Exit");
        Print("\nEnter your option number: ", Color.Blue);
    }

    //choosing option number
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
            default:
                Print("\nInvalid choice. Please try again.", Color.Red);
                break;
        }
    }
    //adding the income 
    public static void AddIncome()
    {
        Print("\nEnter income title: ", Color.Gray);
        Console.ResetColor();
        string title = Console.ReadLine();
        Console.ResetColor();
        Print("\nEnter income amount: ", Color.Gray);
        decimal amount;
        while (!decimal.TryParse(Console.ReadLine(), out amount))
        {
            Print("Invalid amount. Please enter a valid number.", Color.Red);
        }
        Print("\nEnter income month: ", Color.Gray);
        string month = Console.ReadLine();
        items.Add(new Item(title, amount, month));
        Print("\nIncome added successfully!", Color.Green);
    }
    //adding the expense 
    public static void AddExpense()
    {
        Print("\nEnter expense title: ", Color.Gray);
        string title = Console.ReadLine();
        Print("\nEnter expense amount: ", Color.Gray);
        decimal amount;
        while (!decimal.TryParse(Console.ReadLine(), out amount))
        {
            Print("Invalid amount. Please enter a valid number.", Color.Red);
        }
        Print("\nEnter expense month: ", Color.Gray);
        string month = Console.ReadLine();
        items.Add(new Item(title, amount, month, false));
        Print("\nExpense added successfully!", Color.Green);
    }
    //viewing items
    public static void ViewItems()
    {
        if (items.Count == 0)
        {
            Print("\nNo items available.\n", Color.Yellow);
            return;
        }

        Print("\nChoose a filter option:");
        Print(">>> 1) View all items", Color.Gray);
        Print(">>> 2) View only income", Color.Gray);
        Print(">>> 3) View only expenses", Color.Gray);
        Print("Enter your choice: ", Color.Blue);

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

        // Now prompt for sorting options
        Print("\nChoose a sorting option:");
        Print(">>> 1) Sort by title", Color.Gray);
        Print(">>> 2) Sort by amount", Color.Gray);
        Print(">>> 3) Sort by month", Color.Gray);
        Print("Enter your choice: ", Color.Blue);

        string sortChoice = Console.ReadLine();
        switch (sortChoice)
        {
            case "1":
                filteredItems = filteredItems.OrderBy(i => i.Title); // Sort by title
                break;
            case "2":
                filteredItems = filteredItems.OrderBy(i => i.Amount); // Sort by amount
                break;
            case "3":
                filteredItems = filteredItems.OrderBy(i => i.Month); // Sort by month
                break;
            default:
                Print("\nInvalid choice. Returning to menu.", Color.Red);
                return;
        }

        Print("\nCurrent Items:\n", Color.DarkGreen);

        decimal totalIncome = 0;
        decimal totalExpense = 0;

        foreach (var item in filteredItems)
        {
            Print($"- {item.Title}: ${item.Amount:F2} ({item.Month})", item.IsIncome ? Color.Green : Color.Red);

            // Sum based on if the item is income or expense
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
        Print($"\nTotal income: ${totalIncome:F2}", Color.Green);
        Print($"Total expense: ${totalExpense:F2}", Color.Red);
        Print($"Net balance: ${totalIncome - totalExpense:F2}", Color.Yellow);
    }

    //delete or edit
    public static void DeleteOrEditItem()
    {
        Print("\nEnter the title of the item you want to delete or edit: ", Color.Gray);
        string title = Console.ReadLine();
        var itemToEditOrRemove = items.FirstOrDefault(i => i.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

        if (itemToEditOrRemove != null)
        {
            Print($"\nItem found: {itemToEditOrRemove.Title}: ${itemToEditOrRemove.Amount:F2} ({itemToEditOrRemove.Month})", Color.White);
            Print("\nWould you like to (1) Edit or (2) Delete this item? Enter 1 or 2:", Color.Gray);
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                EditItem(itemToEditOrRemove);
            }
            else if (choice == "2")
            {
                items.Remove(itemToEditOrRemove);
                Print("\nItem deleted successfully!", Color.Red);
            }
            else
            {
                Print("\nInvalid choice. Returning to menu.", Color.Red);
            }
        }
        else
        {
            Print("\nItem not found.", Color.Red);
        }
    }

    private static void EditItem(Item itemToEdit)
    {
        Print("\nEditing item...");

        Print("\nEnter new title (leave blank to keep current): ", Color.Gray);
        string newTitle = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newTitle))
        {
            itemToEdit.Title = newTitle;
        }

        Print("\nEnter new amount (leave blank to keep current): ", Color.Gray);
        string newAmountInput = Console.ReadLine();
        if (decimal.TryParse(newAmountInput, out decimal newAmount))
        {
            itemToEdit.Amount = newAmount;
        }

        Print("\nEnter new month (leave blank to keep current): ", Color.Gray);
        string newMonth = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newMonth))
        {
            itemToEdit.Month = newMonth;
        }

        Print("\nItem updated successfully!", Color.Green);
    }



}



class Item
{
    public string Title { get; set; }
    public decimal Amount { get; set; }
    public string Month { get; set; }
    public bool IsIncome { get; set; }

    public Item(string title, decimal amount, string month, bool isIncome = true)
    {
        Title = title;
        Amount = amount;
        Month = month;
        IsIncome = isIncome;
    }

}
