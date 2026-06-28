using FoodRecognitionApp.Services.Reports.Documents.Components;
using FoodRecognitionApp.Services.Reports.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FoodRecognitionApp.Services.Reports.Documents
{
    public class AiPerformanceByCategoryDocument : IDocument
    {
        private readonly AiPerformanceByCategoryData _data;
        public AiPerformanceByCategoryDocument(AiPerformanceByCategoryData data) => _data = data;

        public DocumentMetadata GetMetadata() => new() { Title = "AI Performance by Category Report" };

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.Header().Element(c => ReportStyles.ComposeHeader(c, "AI Performance by Category",
                    "Model accuracy analysis across food categories", _data.GeneratedAt));
                page.Footer().Element(ReportStyles.ComposeFooter);

                page.Content().PaddingTop(10).Column(column =>
                {
                    column.Item().Element(c => ReportStyles.ComposeSummaryCards(c,
                        ("Categories", _data.TotalCategories.ToString(), ReportStyles.PrimaryHex),
                        ("Total Recognitions", _data.TotalRecognitions.ToString(), ReportStyles.SuccessHex),
                        ("Overall Avg Confidence", $"{_data.OverallAvgConfidence:F1}%", ReportStyles.AccentHex)
                    ));

                    if (_data.CategoryPerformances.Any())
                    {
                        var ordered = _data.CategoryPerformances.OrderByDescending(c => c.AvgConfidence).ToList();
                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Average Confidence by Category"));
                        column.Item().Image(ReportStyles.CreateBarChart(
                            ordered.Select(c => c.CategoryName).ToArray(),
                            ordered.Select(c => c.AvgConfidence).ToArray(),
                            "Avg Confidence %"));

                        column.Item().PageBreak();

                        var orderedByCount = _data.CategoryPerformances.OrderByDescending(c => c.TotalRecognitions).ToList();
                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Recognitions by Category"));
                        column.Item().Image(ReportStyles.CreateBarChart(
                            orderedByCount.Select(c => c.CategoryName).ToArray(),
                            orderedByCount.Select(c => (double)c.TotalRecognitions).ToArray(),
                            "Total Recognitions"));
                    }

                    if (_data.CategoryPerformances.Any())
                    {
                        column.Item().PageBreak();
                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Category Performance Details"));
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(2);
                                cols.ConstantColumn(65);
                                cols.ConstantColumn(50);
                                cols.ConstantColumn(55);
                                cols.ConstantColumn(50);
                                cols.ConstantColumn(50);
                            });
                            table.Header(header =>
                            {
                                header.Cell().Element(ReportStyles.TableHeaderStyle).Text("Category");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Recognitions");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Foods");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Avg %");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Min %");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Max %");
                            });
                            int idx = 0;
                            foreach (var cat in _data.CategoryPerformances)
                            {
                                var alt = idx % 2 == 1;
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).Text(cat.CategoryName);
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text(cat.TotalRecognitions.ToString());
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text(cat.UniqueFoodsRecognized.ToString());
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text($"{cat.AvgConfidence:F1}");
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text($"{cat.MinConfidence:F1}");
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text($"{cat.MaxConfidence:F1}");
                                idx++;
                            }
                        });
                    }
                });
            });
        }
    }
}
