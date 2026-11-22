using FoodRecognitionApp.Services.Abstraction.Reports.Models;
using FoodRecognitionApp.Shared.Common;

namespace FoodRecognitionApp.Services.Abstraction.Reports
{
    public interface IPdfReportGenerator
    {
        Result<byte[]> GenerateSystemOverviewReport(SystemOverviewData data);
        Result<byte[]> GenerateFoodRecognitionReport(FoodRecognitionReportData data);
        Result<byte[]> GenerateUserDemographicsReport(UserDemographicsData data);
        Result<byte[]> GenerateUserRecognitionHistoryReport(UserRecognitionHistoryData data);
        Result<byte[]> GenerateNutritionalInsightsReport(NutritionalInsightsData data);
        Result<byte[]> GenerateAiPerformanceByCategoryReport(AiPerformanceByCategoryData data);
        Result<byte[]> GenerateUsageTrendsReport(UsageTrendsData data);
    }
}
