using System;
using System.Collections.Generic;


/// <summary>
/// SalesTaxManager: Reduced to just managing the flow by delegating responsibilities.
/// InputParser: Handles parsing of input.
/// ExemptItemChecker: Handles determining if an item is tax-exempt.
/// TaxCalculator: Encapsulates the logic for tax calculation.
//  ReceiptPrinter: Dedicated class to print receipts.
/// </summary>


public interface IInputParser
{
    List<Item> ParseItems(string[] input);
}

public interface ITaxCalculator
{
    void CalculateTaxes(List<Item> items);
}

public interface IReceiptPrinter
{
    void PrintReceipt(List<Item> items, int billCount);
}

public class SalesTaxManager
{
    private readonly IInputParser _inputParser;
    private readonly ITaxCalculator _taxCalculator;
    private readonly IReceiptPrinter _receiptPrinter;

    public SalesTaxManager(IInputParser inputParser, ITaxCalculator taxCalculator, IReceiptPrinter receiptPrinter)
    {
        _inputParser = inputParser;
        _taxCalculator = taxCalculator;
        _receiptPrinter = receiptPrinter;
    }

    public void ProcessBill(List<string> inputItems, int billCount)
    {
        List<Item> items = _inputParser.ParseItems(inputItems.ToArray());
        _taxCalculator.CalculateTaxes(items);
        _receiptPrinter.PrintReceipt(items, billCount);
    }

    public static void Main(string[] args)
    {
        var manager = new SalesTaxManager(new InputParser(), new TaxCalculator(), new ReceiptPrinter());

        int billCount = 1;
        while (true)
        {
            List<string> inputItems = new List<string>();
            Console.WriteLine($"Enter item details for Input {billCount} (e.g., '1 book at 12.49'). Type 'done' when finished with this bill:");

            while (true)
            {
                string input = Console.ReadLine();
                if (input.Trim().ToLower() == "done") break;
                inputItems.Add(input);
            }

            // Process the bill and print receipt
            manager.ProcessBill(inputItems, billCount);

            Console.WriteLine("Do you want to enter another bill? (yes/no)");
            string continueInput = Console.ReadLine();
            if (continueInput.Trim().ToLower() != "yes") break;

            billCount++;
        }
    }
}

public class InputParser : IInputParser
{
    public List<Item> ParseItems(string[] input)
    {
        List<Item> items = new List<Item>();

        foreach (var line in input)
        {
            string[] parts = line.Split(new string[] { " at " }, StringSplitOptions.None);
            if (parts.Length != 2) continue; // Skip lines that do not split correctly

            string[] quantityAndName = parts[0].Trim().Split(' ');
            if (quantityAndName.Length < 2) continue; // Skip lines without quantity or name

            if (!int.TryParse(quantityAndName[0], out int quantity)) continue; // Skip if quantity is invalid

            string name = string.Join(" ", quantityAndName, 1, quantityAndName.Length - 1).Trim();
            if (!double.TryParse(parts[1].Trim(), out double price)) continue; // Skip if price is invalid

            bool isImported = name.Contains("imported");
            bool isExempt = ExemptItemChecker.IsExemptItem(name);

            items.Add(new Item(name, isImported, isExempt, price, quantity));
        }

        return items;
    }
}

public static class ExemptItemChecker
{
    private static readonly List<string> ExemptItems = new List<string> { "book", "chocolate", "pill" };

    public static bool IsExemptItem(string name)
    {
        foreach (var exempt in ExemptItems)
        {
            if (name.Contains(exempt))
            {
                return true;
            }
        }
        return false;
    }
}

public class TaxCalculator : ITaxCalculator
{
    public void CalculateTaxes(List<Item> items)
    {
        foreach (var item in items)
        {
            item.CalculateTaxedPrice();
        }
    }
}

public class ReceiptPrinter : IReceiptPrinter
{
    public void PrintReceipt(List<Item> items, int billCount)
    {
        double totalTaxes = 0;
        double totalCost = 0;

        Console.WriteLine($"\nOutput {billCount}:");

        foreach (var item in items)
        {
            double itemTotalPrice = item.GetTotalPrice();
            totalTaxes += item.GetTotalTax();
            totalCost += itemTotalPrice;

            Console.WriteLine($"{item.Quantity} {item.Name}: {itemTotalPrice:F2}");
        }

        Console.WriteLine($"Sales Taxes: {totalTaxes:F2}");
        Console.WriteLine($"Total: {totalCost:F2}\n");
    }
}

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

    public double CalculateTaxedPrice()
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
