using FoodRecognitionApp.Domain.Exceptions.UnAuthorized;
using FoodRecognitionApp.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace FoodRecognitionApp.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpGet("system-overview")]
        public async Task<IActionResult> GetSystemOverviewReport()
        {
            var pdf = await _serviceManager.ReportService.GenerateSystemOverviewReportAsync();
            return File(pdf, "application/pdf", "SystemOverviewReport.pdf");
        }

        [HttpGet("food-recognition")]
        public async Task<IActionResult> GetFoodRecognitionReport()
        {
            var pdf = await _serviceManager.ReportService.GenerateFoodRecognitionReportAsync();
            return File(pdf, "application/pdf", "FoodRecognitionReport.pdf");
        }

        [HttpGet("user-demographics")]
        public async Task<IActionResult> GetUserDemographicsReport()
        {
            var pdf = await _serviceManager.ReportService.GenerateUserDemographicsReportAsync();
            return File(pdf, "application/pdf", "UserDemographicsReport.pdf");
        }

        [Authorize]
        [HttpGet("user-history")]
        public async Task<IActionResult> GetUserRecognitionHistoryReport()
        {
            var userId = GetCurrentUserId();
            var pdf = await _serviceManager.ReportService.GenerateUserRecognitionHistoryReportAsync(userId);
            return File(pdf, "application/pdf", "UserRecognitionHistoryReport.pdf");
        }

        [HttpGet("nutritional-insights")]
        public async Task<IActionResult> GetNutritionalInsightsReport()
        {
            var pdf = await _serviceManager.ReportService.GenerateNutritionalInsightsReportAsync();
            return File(pdf, "application/pdf", "NutritionalInsightsReport.pdf");
        }

        [HttpGet("ai-performance")]
        public async Task<IActionResult> GetAiPerformanceReport()
        {
            var pdf = await _serviceManager.ReportService.GenerateAiPerformanceByCategoryReportAsync();
            return File(pdf, "application/pdf", "AiPerformanceByCategoryReport.pdf");
        }

        [HttpGet("usage-trends")]
        public async Task<IActionResult> GetUsageTrendsReport()
        {
            var pdf = await _serviceManager.ReportService.GenerateUsageTrendsReportAsync();
            return File(pdf, "application/pdf", "UsageTrendsReport.pdf");
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null) throw new UnAuthorizedException("You are not Authorized");
            return int.Parse(userIdClaim.Value);
        }
    }
}
