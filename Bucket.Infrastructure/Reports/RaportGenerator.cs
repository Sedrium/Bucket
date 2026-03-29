using System.Globalization;
using System.Text;
using Bucket.Application.Services;
using Bucket.Application.Reports;

namespace Bucket.Infrastructure.Reports;

public class RaportGenerator : IRaportGenerator
{
    private static readonly CultureInfo PlCulture = CultureInfo.GetCultureInfo("pl-PL");

    public PurchaseCsvReport GeneratePurchaseCsvReport(PurchaseCsvReportInput input)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"CustomerName:;{EscapeCsvField(input.CustomerDisplayName)}");
        sb.AppendLine("ProductId;Count;ProductName;Price");

        foreach (var line in input.Lines)
        {
            var priceStr = line.Price.ToString("0.##", PlCulture);
            sb.AppendLine($"{line.ProductId};{line.Count};{EscapeCsvField(line.ProductName)};{priceStr}");
        }

        var fileName = $"purchase-{input.PurchaseId}-report.csv";
        return new PurchaseCsvReport(fileName, sb.ToString());
    }

    private static string EscapeCsvField(string value)
    {
        if (value.Contains(';') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            return '"' + value.Replace("\"", "\"\"") + '"';
        }

        return value;
    }
}
