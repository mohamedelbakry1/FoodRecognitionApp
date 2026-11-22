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
    public class UserDemographicsDocument : IDocument
    {
        private readonly UserDemographicsData _data;
        public UserDemographicsDocument(UserDemographicsData data) => _data = data;

        public DocumentMetadata GetMetadata() => new() { Title = "User Demographics Report" };

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.Header().Element(c => ReportStyles.ComposeHeader(c, "User Demographics Report", "User profile analysis and distribution", _data.GeneratedAt));
                page.Footer().Element(ReportStyles.ComposeFooter);

                page.Content().PaddingTop(10).Column(column =>
                {
                    column.Item().Element(c => ReportStyles.ComposeSummaryCards(c,
                        ("Total Users", _data.TotalUsers.ToString(), ReportStyles.PrimaryHex),
                        ("Male", _data.MaleCount.ToString(), ReportStyles.InfoHex),
                        ("Female", _data.FemaleCount.ToString(), ReportStyles.AccentHex),
                        ("Avg Age", $"{_data.AvgAge:F0}", ReportStyles.SuccessHex)
                    ));

                    if (_data.MaleCount + _data.FemaleCount > 0)
                    {
                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Gender Distribution"));
                        column.Item().Element(c => ReportStyles.ComposePieWithLegend(c, new List<(string, double)>
                        {
                            ($"Male ({_data.MaleCount})", Math.Max(_data.MaleCount, 0.1)),
                            ($"Female ({_data.FemaleCount})", Math.Max(_data.FemaleCount, 0.1))
                        }));
                    }

                    column.Item().PageBreak();

                    column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Goal Distribution"));
                    column.Item().Element(c => ReportStyles.ComposePieWithLegend(c, new List<(string, double)>
                    {
                        ($"Maintain Weight ({_data.MaintainWeightCount})", Math.Max(_data.MaintainWeightCount, 0.1)),
                        ($"Lose Weight ({_data.LoseWeightCount})", Math.Max(_data.LoseWeightCount, 0.1)),
                        ($"Gain Weight ({_data.GainWeightCount})", Math.Max(_data.GainWeightCount, 0.1))
                    }));

                    column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "Activity Level Distribution"));
                    column.Item().Image(ReportStyles.CreateBarChart(
                        new[] { "Sedentary", "Lightly Active", "Moderate", "Highly Active" },
                        new double[] { _data.SedentaryCount, _data.LightlyActiveCount, _data.ModeratelyActiveCount, _data.HighlyActiveCount },
                        "Number of Users"));

                    if (_data.UserDetails.Any())
                    {
                        column.Item().PageBreak();
                        column.Item().Element(c => ReportStyles.ComposeSectionTitle(c, "User Details"));
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(1.5f);
                                cols.ConstantColumn(35);
                                cols.ConstantColumn(50);
                                cols.ConstantColumn(50);
                                cols.ConstantColumn(50);
                                cols.RelativeColumn(1.2f);
                                cols.RelativeColumn(1.2f);
                                cols.ConstantColumn(50);
                            });
                            table.Header(header =>
                            {
                                header.Cell().Element(ReportStyles.TableHeaderStyle).Text("User");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Age");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Weight");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Height");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Gender");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).Text("Goal");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).Text("Activity");
                                header.Cell().Element(ReportStyles.TableHeaderStyle).AlignCenter().Text("Kcal");
                            });
                            int idx = 0;
                            foreach (var u in _data.UserDetails)
                            {
                                var alt = idx % 2 == 1;
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).Text(u.UserName).FontSize(8);
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text(u.Age.ToString());
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text($"{u.Weight:F0}");
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text($"{u.Height:F0}");
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text(u.Gender);
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).Text(u.GoalType).FontSize(8);
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).Text(u.ActivityLevel).FontSize(8);
                                table.Cell().Element(c => ReportStyles.TableCellStyle(c, alt)).AlignCenter().Text($"{u.DailyCalories:F0}");
                                idx++;
                            }
                        });
                    }
                });
            });
        }
    }
}
