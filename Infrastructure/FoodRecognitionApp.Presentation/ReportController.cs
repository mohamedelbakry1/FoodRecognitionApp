using FoodRecognitionApp.Services.Abstraction.Reports;
using FoodRecognitionApp.Shared.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FoodRecognitionApp.Presentation
{
    public class ReportController(IReportService _reportService) : ApiBaseController
    {
        [HttpGet("system-overview")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK, "application/pdf")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSystemOverviewReport(CancellationToken ct)
        {
            var result = await _reportService.GenerateSystemOverviewReportAsync(ct);
            if (result.IsFailure)
                return ToActionResult((Result)result);

            return File(result.Value, "application/pdf", "SystemOverviewReport.pdf");
        }

        [HttpGet("food-recognition")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK, "application/pdf")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFoodRecognitionReport(CancellationToken ct)
        {
            var result = await _reportService.GenerateFoodRecognitionReportAsync(ct);
            if (result.IsFailure)
                return ToActionResult((Result)result);

            return File(result.Value, "application/pdf", "FoodRecognitionReport.pdf");
        }

        [HttpGet("user-demographics")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK, "application/pdf")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserDemographicsReport(CancellationToken ct)
        {
            var result = await _reportService.GenerateUserDemographicsReportAsync(ct);
            if (result.IsFailure)
                return ToActionResult((Result)result);

            return File(result.Value, "application/pdf", "UserDemographicsReport.pdf");
        }

        [Authorize]
        [HttpGet("user-history")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK, "application/pdf")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserRecognitionHistoryReport(CancellationToken ct)
        {
            var userId = GetUserIdFromToken();
            var result = await _reportService.GenerateUserRecognitionHistoryReportAsync(userId, ct);
            if (result.IsFailure)
                return ToActionResult((Result)result);

            return File(result.Value, "application/pdf", "UserRecognitionHistoryReport.pdf");
        }

        [HttpGet("nutritional-insights")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK, "application/pdf")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetNutritionalInsightsReport(CancellationToken ct)
        {
            var result = await _reportService.GenerateNutritionalInsightsReportAsync(ct);
            if (result.IsFailure)
                return ToActionResult((Result)result);

            return File(result.Value, "application/pdf", "NutritionalInsightsReport.pdf");
        }

        [HttpGet("ai-performance")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK, "application/pdf")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAiPerformanceReport(CancellationToken ct)
        {
            var result = await _reportService.GenerateAiPerformanceByCategoryReportAsync(ct);
            if (result.IsFailure)
                return ToActionResult((Result)result);

            return File(result.Value, "application/pdf", "AiPerformanceByCategoryReport.pdf");
        }

        [HttpGet("usage-trends")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK, "application/pdf")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUsageTrendsReport(CancellationToken ct)
        {
            var result = await _reportService.GenerateUsageTrendsReportAsync(ct);
            if (result.IsFailure)
                return ToActionResult((Result)result);

            return File(result.Value, "application/pdf", "UsageTrendsReport.pdf");
        }
    }
}
