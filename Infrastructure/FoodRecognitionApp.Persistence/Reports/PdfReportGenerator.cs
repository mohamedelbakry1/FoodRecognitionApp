using FoodRecognitionApp.Persistence.Reports.Documents;
using FoodRecognitionApp.Services.Abstraction.Reports;
using FoodRecognitionApp.Services.Abstraction.Reports.Models;
using FoodRecognitionApp.Shared.Common;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;

namespace FoodRecognitionApp.Persistence.Reports
{
    public class PdfReportGenerator : IPdfReportGenerator
    {
        private readonly ILogger<PdfReportGenerator> _logger;

        public PdfReportGenerator(ILogger<PdfReportGenerator> logger)
        {
            _logger = logger;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public Result<byte[]> GenerateSystemOverviewReport(SystemOverviewData data)
        {
            try
            {
                var document = new SystemOverviewDocument(data);
                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate System Overview Report PDF");
                return Error.Failure("Report.GenerationFailed", "Failed to generate System Overview Report PDF.");
            }
        }

        public Result<byte[]> GenerateFoodRecognitionReport(FoodRecognitionReportData data)
        {
            try
            {
                var document = new FoodRecognitionDocument(data);
                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate Food Recognition Report PDF");
                return Error.Failure("Report.GenerationFailed", "Failed to generate Food Recognition Report PDF.");
            }
        }

        public Result<byte[]> GenerateUserDemographicsReport(UserDemographicsData data)
        {
            try
            {
                var document = new UserDemographicsDocument(data);
                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate User Demographics Report PDF");
                return Error.Failure("Report.GenerationFailed", "Failed to generate User Demographics Report PDF.");
            }
        }

        public Result<byte[]> GenerateUserRecognitionHistoryReport(UserRecognitionHistoryData data)
        {
            try
            {
                var document = new UserRecognitionHistoryDocument(data);
                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate User Recognition History Report PDF");
                return Error.Failure("Report.GenerationFailed", "Failed to generate User Recognition History Report PDF.");
            }
        }

        public Result<byte[]> GenerateNutritionalInsightsReport(NutritionalInsightsData data)
        {
            try
            {
                var document = new NutritionalInsightsDocument(data);
                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate Nutritional Insights Report PDF");
                return Error.Failure("Report.GenerationFailed", "Failed to generate Nutritional Insights Report PDF.");
            }
        }

        public Result<byte[]> GenerateAiPerformanceByCategoryReport(AiPerformanceByCategoryData data)
        {
            try
            {
                var document = new AiPerformanceByCategoryDocument(data);
                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate AI Performance By Category Report PDF");
                return Error.Failure("Report.GenerationFailed", "Failed to generate AI Performance By Category Report PDF.");
            }
        }

        public Result<byte[]> GenerateUsageTrendsReport(UsageTrendsData data)
        {
            try
            {
                var document = new UsageTrendsDocument(data);
                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate Usage Trends Report PDF");
                return Error.Failure("Report.GenerationFailed", "Failed to generate Usage Trends Report PDF.");
            }
        }
    }
}
