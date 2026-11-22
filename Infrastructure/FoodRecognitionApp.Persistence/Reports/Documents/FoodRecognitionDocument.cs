using FoodRecognitionApp.Persistence.Reports.Documents.Components;
using FoodRecognitionApp.Services.Abstraction.Reports.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;

namespace FoodRecognitionApp.Persistence.Reports.Documents
{
    public class FoodRecognitionDocument : IDocument
    {
        private readonly FoodRecognitionReportData _data;
        public FoodRecognitionDocument(FoodRecognitionReportData data) => _data = data;

        public DocumentMetadata GetMetadata() => new() { Title = "Food Recognition Analysis Report" };

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.Header().Element(c => ReportStyles.ComposeHeader(c, "Food Recognition Analysis", "AI recognition accuracy and performance metrics", _data.GeneratedAt));
                page.Footer().Element(ReportStyles.ComposeFooter);

                page.Content().PaddingTop(10).Column(column =>
                {
                    column.Item().Element(c => ReportStyles.ComposeSummaryCards(c,
                        ("Total Recognitions", _data.TotalRecognitions.ToString(), ReportStyles.PrimaryHex),
                        ("Avg Confidence", $"{_data.AvgConfidence:F1}%", ReportStyles.SuccessHex),
                        ("Min Confidence", $"{_data.MinConfidence:F1}%", ReportStyles.DangerHex),
                        ("Max Confidence", $"{_data.MaxConfidence:F1}%", ReportStyles.InfoHex)
                    ));

                    if (_data.TopFoodsByConfidence.Any())
                    {
                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Top Foods by Confidence Score"));
                        column.Item().Image(ReportStyles.CreateBarChart(
                            _data.TopFoodsByConfidence.Select(f => f.FoodName).ToArray(),
                            _data.TopFoodsByConfidence.Select(f => f.AvgConfidence).ToArray(),
                            "Avg Confidence %"));
                    }

                    column.Item().PageBreak();

                    if (_data.TopFoodsByCount.Any())
                    {
                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Most Frequently Recognized Foods"));
                        column.Item().Image(ReportStyles.CreateBarChart(
                            _data.TopFoodsByCount.Select(f => f.FoodName).ToArray(),
                            _data.TopFoodsByCount.Select(f => (double)f.TimesRecognized).ToArray(),
                            "Times Recognized"));
                    }

                    if (_data.AllFoodDetails.Any())
                    {
                        column.Item().PageBreak();
                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Detailed Food Recognition Data"));
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(1.5f);
                                cols.ConstantColumn(50);
                                cols.ConstantColumn(55);
                                cols.ConstantColumn(50);
                                cols.ConstantColumn(50);
                            });
                            table.Header(header =>
                            {
                                header.Cell().Element(ReportStyles.TableHeaderStyle).Text("Food");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).Text("Category");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Count");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Avg %");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Min %");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Max %");
                            });
                            int idx = 0;
                            foreach (var food in _data.AllFoodDetails)
                            {
                                var alt = idx % 2 == 1;
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).Text(food.FoodName);
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).Text(food.CategoryName);
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text(food.TimesRecognized.ToString());
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text($"{food.AvgConfidence:F1}");
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text($"{food.MinConfidence:F1}");
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text($"{food.MaxConfidence:F1}");
                                idx++;
                            }
                        });
                    }
                });
            });
        }
    }
}
