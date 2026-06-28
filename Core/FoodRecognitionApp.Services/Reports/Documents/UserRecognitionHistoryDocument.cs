using FoodRecognitionApp.Services.Reports.Documents.Components;
using FoodRecognitionApp.Services.Reports.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FoodRecognitionApp.Services.Reports.Documents
{
    public class UserRecognitionHistoryDocument : IDocument
    {
        private readonly UserRecognitionHistoryData _data;
        public UserRecognitionHistoryDocument(UserRecognitionHistoryData data) => _data = data;

        public DocumentMetadata GetMetadata() => new() { Title = "User Recognition History Report" };

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.Header().Element(c => ReportStyles.ComposeHeader(c, "User Recognition History",
                    $"{_data.UserName} ({_data.Email})", _data.GeneratedAt));
                page.Footer().Element(ReportStyles.ComposeFooter);

                page.Content().PaddingTop(10).Column(column =>
                {
                    column.Item().Element(c => ReportStyles.ComposeSummaryCards(c,
                        ("Total Images", _data.TotalImages.ToString(), ReportStyles.PrimaryHex),
                        ("Total Recognitions", _data.TotalRecognitions.ToString(), ReportStyles.SuccessHex),
                        ("Avg Confidence", $"{_data.AvgConfidence:F1}%", ReportStyles.AccentHex)
                    ));

                    if (_data.MonthlyTrend.Any())
                    {
                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Monthly Recognition Trend"));
                        column.Item().Image(ReportStyles.CreateBarChart(
                            _data.MonthlyTrend.Select(m => m.Month).ToArray(),
                            _data.MonthlyTrend.Select(m => (double)m.Count).ToArray(),
                            "Recognitions", useMultiColor: false));
                    }

                    if (_data.Recognitions.Any())
                    {
                        column.Item().PageBreak();
                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Recognition History"));
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(40);
                                cols.ConstantColumn(110);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(1.5f);
                                cols.ConstantColumn(70);
                            });
                            table.Header(header =>
                            {
                                header.Cell().Element(ReportStyles.TableHeaderStyle).Text("#");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).Text("Date");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).Text("Food");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).Text("Category");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Confidence");
                            });
                            int idx = 0;
                            foreach (var r in _data.Recognitions)
                            {
                                var alt = idx % 2 == 1;
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).Text((idx + 1).ToString());
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).Text(r.UploadTime.ToString("yyyy-MM-dd HH:mm"));
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).Text(r.FoodName);
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).Text(r.CategoryName);
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text($"{r.Confidence:F1}%");
                                idx++;
                            }
                        });
                    }
                });
            });
        }
    }
}
