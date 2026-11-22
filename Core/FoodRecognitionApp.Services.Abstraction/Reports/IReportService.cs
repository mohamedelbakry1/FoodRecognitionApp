using FoodRecognitionApp.Shared.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FoodRecognitionApp.Services.Abstraction.Reports
{
    public interface IReportService
    {
        Task<Result<byte[]>> GenerateSystemOverviewReportAsync(CancellationToken ct = default);
        Task<Result<byte[]>> GenerateFoodRecognitionReportAsync(CancellationToken ct = default);
        Task<Result<byte[]>> GenerateUserDemographicsReportAsync(CancellationToken ct = default);
        Task<Result<byte[]>> GenerateUserRecognitionHistoryReportAsync(Guid userId, CancellationToken ct = default);
        Task<Result<byte[]>> GenerateNutritionalInsightsReportAsync(CancellationToken ct = default);
        Task<Result<byte[]>> GenerateAiPerformanceByCategoryReportAsync(CancellationToken ct = default);
        Task<Result<byte[]>> GenerateUsageTrendsReportAsync(CancellationToken ct = default);
    }
}
