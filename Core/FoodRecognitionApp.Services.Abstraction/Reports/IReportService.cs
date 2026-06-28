using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction.Reports
{
    public interface IReportService
    {
        Task<byte[]> GenerateSystemOverviewReportAsync();
        Task<byte[]> GenerateFoodRecognitionReportAsync();
        Task<byte[]> GenerateUserDemographicsReportAsync();
        Task<byte[]> GenerateUserRecognitionHistoryReportAsync(int userId);
        Task<byte[]> GenerateNutritionalInsightsReportAsync();
        Task<byte[]> GenerateAiPerformanceByCategoryReportAsync();
        Task<byte[]> GenerateUsageTrendsReportAsync();
    }
}
