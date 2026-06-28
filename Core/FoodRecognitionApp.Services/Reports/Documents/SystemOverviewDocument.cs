using FoodRecognitionApp.Services.Reports.Documents.Components;
using FoodRecognitionApp.Services.Reports.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FoodRecognitionApp.Services.Reports.Documents
{
    public class SystemOverviewDocument : IDocument
    {
        private readonly SystemOverviewData _data;
        public SystemOverviewDocument(SystemOverviewData data) => _data = data;

        public DocumentMetadata GetMetadata() => new() { Title = "System Overview Report" };

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.Header().Element(c => ReportStyles.ComposeHeader(c, "System Overview Report", "Complete system analytics and statistics", _data.GeneratedAt));
                page.Footer().Element(ReportStyles.ComposeFooter);

                page.Content().PaddingTop(10).Column(column =>
                {
                    column.Item().Element(c => ReportStyles.ComposeSummaryCards(c,
                        ("Total Users", _data.TotalUsers.ToString(), ReportStyles.PrimaryHex),
                        ("Total Images", _data.TotalImages.ToString(), ReportStyles.AccentHex),
                        ("Total Recognitions", _data.TotalRecognitions.ToString(), ReportStyles.SuccessHex)
                    ));
                    column.Item().Element(c => ReportStyles.ComposeSummaryCards(c,
                        ("Total Foods", _data.TotalFoods.ToString(), ReportStyles.InfoHex),
                        ("Total Categories", _data.TotalCategories.ToString(), ReportStyles.WarningHex),
                        ("Avg Confidence", $"{_data.AvgConfidence:F1}%", ReportStyles.DangerHex)
                    ));

                    if (_data.FoodsByCategory.Any())
                    {
                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Food Distribution by Category"));
                        column.Item().Element(c => ReportStyles.ComposePieWithLegend(c,
                            _data.FoodsByCategory.Select(x => (x.CategoryName, (double)x.FoodCount)).ToList()));
                    }

                    column.Item().PageBreak();

                    if (_data.TopRecognizedFoods.Any())
                    {
                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Top Recognized Foods"));
                        column.Item().Image(ReportStyles.CreateBarChart(
                            _data.TopRecognizedFoods.Select(f => f.FoodName).ToArray(),
                            _data.TopRecognizedFoods.Select(f => (double)f.TimesRecognized).ToArray(),
                            "Times Recognized"));
                    }

                    column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Category Details"));
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(50);
                            cols.RelativeColumn();
                            cols.ConstantColumn(100);
                        });
                        table.Header(header =>
                        {
                            header.Cell().Element(ReportStyles.TableHeaderStyle).Text("#");
                            header.Cell().Element(ReportStyles.TableHeaderStyle).Text("Category");
                            header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Foods Count");
                        });
                        int idx = 0;
                        foreach (var cat in _data.FoodsByCategory.OrderByDescending(c => c.FoodCount))
                        {
                            var alt = idx % 2 == 1;
                            table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).Text((idx + 1).ToString());
                            table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).Text(cat.CategoryName);
                            table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text(cat.FoodCount.ToString());
                            idx++;
                        }
                    });
                });
            });
        }
    }
}
