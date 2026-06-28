using FoodRecognitionApp.Services.Reports.Documents.Components;
using FoodRecognitionApp.Services.Reports.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FoodRecognitionApp.Services.Reports.Documents
{
    public class UsageTrendsDocument : IDocument
    {
        private readonly UsageTrendsData _data;
        public UsageTrendsDocument(UsageTrendsData data) => _data = data;

        public DocumentMetadata GetMetadata() => new() { Title = "Usage Trends Report" };

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.Header().Element(c => ReportStyles.ComposeHeader(c, "Usage Trends Report",
                    "Application usage patterns and engagement analytics", _data.GeneratedAt));
                page.Footer().Element(ReportStyles.ComposeFooter);

                page.Content().PaddingTop(10).Column(column =>
                {
                    column.Item().Element(c => ReportStyles.ComposeSummaryCards(c,
                        ("Total Users", _data.TotalUsers.ToString(), ReportStyles.PrimaryHex),
                        ("Total Images", _data.TotalImages.ToString(), ReportStyles.AccentHex),
                        ("Total Recognitions", _data.TotalRecognitions.ToString(), ReportStyles.SuccessHex)
                    ));

                    if (_data.MonthlyUsage.Any())
                    {
                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Monthly Usage Trend"));
                        column.Item().Image(ReportStyles.CreateBarChart(
                            _data.MonthlyUsage.Select(m => m.Month).ToArray(),
                            _data.MonthlyUsage.Select(m => (double)m.Recognitions).ToArray(),
                            "Recognitions", useMultiColor: false));
                    }

                    column.Item().PageBreak();

                    if (_data.DayOfWeekUsage.Any())
                    {
                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Usage by Day of Week"));
                        column.Item().Image(ReportStyles.CreateBarChart(
                            _data.DayOfWeekUsage.Select(d => d.DayName).ToArray(),
                            _data.DayOfWeekUsage.Select(d => (double)d.Count).ToArray(),
                            "Usage Count"));
                    }

                    if (_data.MostActiveUsers.Any())
                    {
                        column.Item().PageBreak();
                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Most Active Users"));
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(35);
                                cols.RelativeColumn(1.5f);
                                cols.RelativeColumn(2);
                                cols.ConstantColumn(60);
                                cols.ConstantColumn(75);
                                cols.ConstantColumn(90);
                            });
                            table.Header(header =>
                            {
                                header.Cell().Element(ReportStyles.TableHeaderStyle).Text("#");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).Text("User");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).Text("Email");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Images");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Recognitions");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Last Activity");
                            });
                            int idx = 0;
                            foreach (var u in _data.MostActiveUsers)
                            {
                                var alt = idx % 2 == 1;
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).Text((idx + 1).ToString());
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).Text(u.UserName);
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).Text(u.Email).FontSize(8);
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text(u.ImagesUploaded.ToString());
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text(u.FoodsRecognized.ToString());
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text(u.LastActivity?.ToString("yyyy-MM-dd") ?? "N/A");
                                idx++;
                            }
                        });
                    }
                });
            });
        }
    }
}
