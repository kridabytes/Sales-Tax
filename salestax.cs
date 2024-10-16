// Makkajai Dev challenge task - (Harshit Bhatt)”


using System;
using System.Collections.Generic;


/// <summary>
/// SalesTaxManager - Manages the calculation and display of sales tax and total costs for items in a shopping basket.
/// PrintReceipt - Method to calculate and print the receipt details
/// IsExemptItem - Mehthod to Check if the item belongs to exempt categories
/// ParseInput -  Method to parse the input and generate items
/// </summary>
public class SalesTaxManager
{
    readonly List<string> exemptItems = new List<string> { "book", "chocolate", "pill" };

    public static void Main(string[] args)
    {
        SalesTaxManager manager = new SalesTaxManager();
        
        int billCount = 1; // To keep track of the number of bills
        while (true)
        {
            // Collect user input for one bill
            List<string> inputItems = new List<string>();
            Console.WriteLine($"Enter item details for Input {billCount} (e.g., '1 book at 12.49'). Type 'done' when finished with this bill:");

            while (true)
            {
                string input = Console.ReadLine();
                if (input.Trim().ToLower() == "done")
                    break;

                inputItems.Add(input);
            }

            // Add items from this bill to the overall list
            List<Item> currentItems = manager.ParseInput(inputItems.ToArray());
            manager.PrintReceipt(currentItems, billCount); // Print the receipt for the current bill

            Console.WriteLine("Do you want to enter another bill? (yes/no)");
            string continueInput = Console.ReadLine();
            if (continueInput.Trim().ToLower() != "yes")
                break;

            billCount++; // Increment the bill count
        }
    }

    public void PrintReceipt(List<Item> items, int billCount)
    {
        double totalTaxes = 0;
        double totalCost = 0;

        Console.WriteLine($"\nOutput {billCount}:"); // Output number

        foreach (var item in items)
        {
            double itemTotalPrice = item.GetTotalPrice();
            totalTaxes += item.GetTotalTax();
            totalCost += itemTotalPrice;

            Console.WriteLine($"{item.Quantity} {item.Name}: {itemTotalPrice:F2}");
        }

        // Print Sales Taxes and Total on separate lines
        Console.WriteLine($"Sales Taxes: {totalTaxes:F2}");
        Console.WriteLine($"Total: {totalCost:F2}\n"); // Combined output
    }

    public List<Item> ParseInput(string[] input)
    {
        List<Item> items = new List<Item>();

        foreach (var line in input)
        {
            string[] parts = line.Split(new string[] { " at " }, StringSplitOptions.None);
            if (parts.Length != 2) continue; // Skip lines that do not split correctly

            string[] quantityAndName = parts[0].Trim().Split(' ');
            if (quantityAndName.Length < 2) continue; // Skip lines without quantity or name
            
            if (!int.TryParse(quantityAndName[0], out int quantity)) continue; // Skip if quantity is invalid
            
            string name = string.Join(' ', quantityAndName, 1, quantityAndName.Length - 1).Trim();
            if (!double.TryParse(parts[1].Trim(), out double price)) continue; // Skip if price is invalid

            bool isImported = name.Contains("imported");
            bool isExempt = IsExemptItem(name);

            items.Add(new Item(name, isImported, isExempt, price, quantity));
        }

        return items;
    }

    public bool IsExemptItem(string name)
    {
        foreach (var exempt in exemptItems)
        {
            if (name.Contains(exempt))
            {
                return true;
            }
        }
        return false;
    }
}
/// <summary>
/// Item class to store the properties of each item
/// CalculateTaxedPrice() Calculate the taxed price based on import and sales tax rules
/// GetTotalPrice() Get total price for the item including tax and quantity
/// GetTotalTax() Get total tax for the item including quantity
/// </summary>
public class Item
{
    public string Name;
    public bool IsImported;
    public bool IsExempt;
    public double Price;
    public double TaxedPrice;
    public int Quantity;

    public Item(string name, bool isImported, bool isExempt, double price, int quantity)
    {
        Name = name;
        IsImported = isImported;
        IsExempt = isExempt;
        Price = price;
        Quantity = quantity;
        TaxedPrice = CalculateTaxedPrice();
    }

    private double CalculateTaxedPrice()
    {
        double salesTax = 0;

        if (!IsExempt)
            salesTax += 0.10 * Price;

        if (IsImported)
            salesTax += 0.05 * Price;

        salesTax = Math.Ceiling(salesTax * 20) / 20;

        return Price + salesTax;
    }

    public double GetTotalPrice()
    {
        return TaxedPrice * Quantity;
    }

    public double GetTotalTax()
    {
        return (TaxedPrice - Price) * Quantity;
    }
}
