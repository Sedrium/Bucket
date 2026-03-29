namespace Bucket.Application.Reports;

public record PurchaseCsvReport(string FileName, string Content);

public record PurchaseCsvLineItem(long ProductId, int Count, string ProductName, double Price);

public record PurchaseCsvReportInput(long PurchaseId, string CustomerDisplayName, IReadOnlyList<PurchaseCsvLineItem> Lines);
