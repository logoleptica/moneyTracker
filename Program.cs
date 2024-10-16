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
        Console.BackgroundColor = color;
        Console.WriteLine(message); // WriteLine to avoid staying on the same line
        Console.ResetColor();
    }

    public static void Greeting()
    {
        Print("\n>>> Track Your Money Console\n", Color.DarkCyan);
    }

    public static void DisplayMenu()
    {
        Print("\n>>> Choose an option:");
        Print("1) Add income", Color.Gray);
        Print("2) Add expense", Color.Gray);
        Print("3) View items", Color.Gray);
        Print("4) Delete item", Color.Gray);
        Print("5) Exit", Color.Blue);
        Print("\nEnter your option number: ", Color.Blue);
    }

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
                DeleteItem();
                break;
            default:
                Print("\nInvalid choice. Please try again.", Color.Red);
                break;
        }
    }

    public static void AddIncome()
    {
        Print("\nEnter income title: ", Color.Gray);
        string title = Console.ReadLine();
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

    public static void ViewItems()
    {
        if (items.Count == 0)
        {
            Print("\nNo items available.\n", Color.Yellow);
            return;
        }

        Print("\nCurrent Items:\n", Color.DarkGreen);

        decimal totalIncome = 0;
        decimal totalExpense = 0;

        foreach (var item in items)
        {
            Print($"- {item.Title}: ${item.Amount:F2} ({item.Month})", item.IsIncome ? Color.Green : Color.Red);

            // Sum based on whether the item is income or expense
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
    }


    public static void DeleteItem()
    {
        Print("\nEnter the title of the item you want to delete: ", Color.Gray);
        string title = Console.ReadLine();
        var itemToRemove = items.FirstOrDefault(i => i.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (itemToRemove != null)
        {
            items.Remove(itemToRemove);
            Print("\nItem deleted successfully!", Color.Red);
        }
        else
        {
            Print("\nItem not found.", Color.Red);
        }
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
