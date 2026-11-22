using FoodRecognitionApp.Persistence.Reports.Documents.Components;
using FoodRecognitionApp.Services.Abstraction.Reports.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodRecognitionApp.Persistence.Reports.Documents
{
    public class NutritionalInsightsDocument : IDocument
    {
        private readonly NutritionalInsightsData _data;
        public NutritionalInsightsDocument(NutritionalInsightsData data) => _data = data;

        public DocumentMetadata GetMetadata() => new() { Title = "Nutritional Insights Report" };

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.Header().Element(c => ReportStyles.ComposeHeader(c, "Nutritional Insights Report",
                    "Analysis of nutritional values of recognized foods", _data.GeneratedAt));
                page.Footer().Element(ReportStyles.ComposeFooter);

                page.Content().PaddingTop(10).Column(column =>
                {
                    column.Item().Element(c => ReportStyles.ComposeSummaryCards(c,
                        ("Avg Calories", $"{_data.AvgCalories:F0} kcal", ReportStyles.AccentHex),
                        ("Avg Protein", $"{_data.AvgProtein:F1}g", ReportStyles.SuccessHex),
                        ("Avg Carbs", $"{_data.AvgCarbs:F1}g", ReportStyles.InfoHex),
                        ("Avg Fats", $"{_data.AvgFats:F1}g", ReportStyles.WarningHex)
                    ));

                    if ((double)_data.AvgCarbs + (double)_data.AvgProtein + (double)_data.AvgFats > 0)
                    {
                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Macro Distribution"));
                        column.Item().Element(c => ReportStyles.ComposePieWithLegend(c, new List<(string, double)>
                        {
                            ($"Carbs ({_data.AvgCarbs:F1}g)", Math.Max((double)_data.AvgCarbs, 0.1)),
                            ($"Protein ({_data.AvgProtein:F1}g)", Math.Max((double)_data.AvgProtein, 0.1)),
                            ($"Fats ({_data.AvgFats:F1}g)", Math.Max((double)_data.AvgFats, 0.1))
                        }));
                    }

                    column.Item().PageBreak();

                    if (_data.TopRecognizedFoods.Any())
                    {
                        var top10 = _data.TopRecognizedFoods.OrderByDescending(f => f.Calories).Take(10).ToList();
                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Top Foods by Calories"));
                        column.Item().Image(ReportStyles.CreateBarChart(
                            top10.Select(f => f.FoodName).ToArray(),
                            top10.Select(f => (double)f.Calories).ToArray(),
                            "Calories (kcal)"));

                        column.Item().PageBreak();

                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Detailed Nutritional Values"));
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(1.3f);
                                cols.ConstantColumn(45);
                                cols.ConstantColumn(55);
                                cols.ConstantColumn(50);
                                cols.ConstantColumn(50);
                                cols.ConstantColumn(45);
                            });
                            table.Header(header =>
                            {
                                header.Cell().Element(ReportStyles.TableHeaderStyle).Text("Food");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).Text("Category");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Count");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Calories");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Carbs");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Protein");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Fats");
                            });
                            int idx = 0;
                            foreach (var f in _data.TopRecognizedFoods)
                            {
                                var alt = idx % 2 == 1;
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).Text(f.FoodName);
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).Text(f.CategoryName);
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text(f.TimesRecognized.ToString());
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text($"{f.Calories:F0}");
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text($"{f.Carbs:F1}");
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text($"{f.Protein:F1}");
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text($"{f.Fats:F1}");
                                idx++;
                            }
                        });
                    }
                });
            });
        }
    }
}
