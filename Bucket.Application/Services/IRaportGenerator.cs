using Bucket.Application.Reports;

namespace Bucket.Application.Services;

public interface IRaportGenerator
{
    PurchaseCsvReport GeneratePurchaseCsvReport(PurchaseCsvReportInput input);
}
