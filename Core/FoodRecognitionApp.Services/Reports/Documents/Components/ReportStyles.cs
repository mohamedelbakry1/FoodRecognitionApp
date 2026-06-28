using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FoodRecognitionApp.Services.Reports.Documents.Components
{
    public static class ReportStyles
    {
        // Brand Colors
        public static readonly string PrimaryHex = "#2563EB";
        public static readonly string PrimaryDarkHex = "#1E40AF";
        public static readonly string AccentHex = "#F97316";
        public static readonly string SuccessHex = "#22C55E";
        public static readonly string DangerHex = "#EF4444";
        public static readonly string WarningHex = "#EAB308";
        public static readonly string InfoHex = "#06B6D4";
        public static readonly string GrayHex = "#6B7280";
        public static readonly string LightGrayHex = "#F3F4F6";
        public static readonly string DarkHex = "#1F2937";
        public static readonly string WhiteHex = "#FFFFFF";

        // Chart Colors Palette (vibrant, professional)
        public static readonly string[] ChartColors = new[]
        {
            "#3B82F6", "#F97316", "#10B981", "#EF4444", "#8B5CF6",
            "#06B6D4", "#EC4899", "#F59E0B", "#14B8A6", "#6366F1",
            "#F43F5E", "#84CC16"
        };

        // Font Sizes
        public const float TitleSize = 22;
        public const float SubtitleSize = 16;
        public const float SectionTitleSize = 14;
        public const float NormalSize = 10;
        public const float SmallSize = 8;

        #region Chart Helpers

        /// <summary>
        /// Renders a donut chart image (NO text on chart) + a QuestPDF legend beside it
        /// </summary>
        public static void ComposePieWithLegend(IContainer container, List<(string Label, double Value)> data, int maxSlices = 7)
        {
            var sorted = data.Where(d => d.Value > 0).OrderByDescending(d => d.Value).ToList();
            if (!sorted.Any()) return;

            var top = sorted.Take(maxSlices).ToList();
            var otherVal = sorted.Skip(maxSlices).Sum(d => d.Value);
            if (otherVal > 0) top.Add(("Other", otherVal));

            var total = top.Sum(t => t.Value);
            var items = top.Select((item, i) => (
                item.Label,
                item.Value,
                Color: ChartColors[i % ChartColors.Length],
                Pct: total > 0 ? (item.Value / total * 100) : 0.0
            )).ToList();

            var pieBytes = GenerateCleanDonut(items.Select(i => (i.Value, i.Color)).ToList());

            container.PaddingVertical(5).Row(row =>
            {
                row.RelativeItem(3).AlignCenter().AlignMiddle().Image(pieBytes);

                row.RelativeItem(2).PaddingLeft(15).AlignMiddle().Column(legendCol =>
                {
                    foreach (var item in items)
                    {
                        legendCol.Item().PaddingVertical(4).Row(legendRow =>
                        {
                            legendRow.ConstantItem(14).Height(14)
                                .Background(item.Color)
                                .Border(0.5f).BorderColor(LightGrayHex);
                            legendRow.AutoItem().PaddingLeft(8).AlignMiddle()
                                .Text($"{item.Label}  ({item.Pct:F0}%)")
                                .FontSize(9).FontColor(DarkHex);
                        });
                    }
                });
            });
        }

        /// <summary>
        /// Generates a clean donut chart image with NO labels - just colored slices
        /// </summary>
        private static byte[] GenerateCleanDonut(List<(double Value, string Color)> data)
        {
            var plot = new ScottPlot.Plot();
            plot.FigureBackground.Color = ScottPlot.Color.FromHex(WhiteHex);
            plot.DataBackground.Color = ScottPlot.Color.FromHex(WhiteHex);

            var slices = data.Select(d => new ScottPlot.PieSlice
            {
                Value = d.Value,
                FillColor = ScottPlot.Color.FromHex(d.Color)
            }).ToList();

            var pie = plot.Add.Pie(slices);
            pie.DonutFraction = 0.5;
            pie.ExplodeFraction = 0.03;
            plot.Axes.Frameless();

            return plot.GetImageBytes(320, 280, ScottPlot.ImageFormat.Png);
        }

        /// <summary>
        /// Creates a bar chart with truncated labels and multi-color bars
        /// </summary>
        public static byte[] CreateBarChart(string[] labels, double[] values, string yAxisLabel, bool useMultiColor = true, int maxBars = 8)
        {
            // Limit number of bars
            var count = Math.Min(labels.Length, maxBars);
            var trimmedLabels = labels.Take(count).Select(l => l.Length > 13 ? l[..11] + ".." : l).ToArray();
            var trimmedValues = values.Take(count).ToArray();

            var plot = new ScottPlot.Plot();
            plot.FigureBackground.Color = ScottPlot.Color.FromHex(WhiteHex);
            plot.DataBackground.Color = ScottPlot.Color.FromHex("#FAFBFC");
            plot.Axes.Left.Label.ForeColor = ScottPlot.Color.FromHex("#374151");
            plot.Axes.Left.Label.FontSize = 12;
            plot.Axes.Bottom.TickLabelStyle.FontSize = 10;
            plot.Axes.Left.TickLabelStyle.FontSize = 10;

            var bars = plot.Add.Bars(trimmedValues);
            for (int i = 0; i < bars.Bars.Count; i++)
            {
                var colorHex = useMultiColor
                    ? ChartColors[i % ChartColors.Length]
                    : PrimaryHex;
                bars.Bars[i].FillColor = ScottPlot.Color.FromHex(colorHex);
                bars.Bars[i].LineWidth = 0;
            }

            var ticks = trimmedValues.Select((_, i) => new ScottPlot.Tick(i, trimmedLabels[i])).ToArray();
            plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
            plot.Axes.Bottom.TickLabelStyle.Rotation = 30;
            plot.Axes.Left.Label.Text = yAxisLabel;

            return plot.GetImageBytes(540, 270, ScottPlot.ImageFormat.Png);
        }

        #endregion

        #region PDF Layout Components

        public static void ComposeHeader(IContainer container, string reportTitle, string subtitle, DateTime generatedAt)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(2).BorderColor(PrimaryHex).PaddingBottom(10).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("NutriLens").FontSize(TitleSize).Bold().FontColor(PrimaryHex);
                        col.Item().Text("Food Recognition App").FontSize(SmallSize).FontColor(GrayHex);
                    });

                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().AlignRight().Text(reportTitle).FontSize(SubtitleSize).Bold().FontColor(DarkHex);
                        col.Item().AlignRight().Text(subtitle).FontSize(SmallSize).FontColor(GrayHex);
                        col.Item().AlignRight().Text($"Generated: {generatedAt:yyyy-MM-dd HH:mm}").FontSize(SmallSize).FontColor(GrayHex);
                    });
                });
            });
        }

        public static void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(x =>
            {
                x.Span("Page ").FontSize(SmallSize).FontColor(GrayHex);
                x.CurrentPageNumber().FontSize(SmallSize).FontColor(GrayHex);
                x.Span(" of ").FontSize(SmallSize).FontColor(GrayHex);
                x.TotalPages().FontSize(SmallSize).FontColor(GrayHex);
            });
        }

        public static void ComposeSummaryCard(IContainer container, string label, string value, string colorHex)
        {
            container.Border(1).BorderColor(LightGrayHex).Background(WhiteHex)
                .Padding(12).Column(col =>
                {
                    col.Item().AlignCenter().Text(value).FontSize(24).Bold().FontColor(colorHex);
                    col.Item().AlignCenter().Text(label).FontSize(SmallSize).FontColor(GrayHex);
                });
        }

        public static void ComposeSummaryCards(IContainer container, params (string Label, string Value, string Color)[] cards)
        {
            container.Row(row =>
            {
                foreach (var card in cards)
                {
                    row.RelativeItem().Padding(4).Element(c => ComposeSummaryCard(c, card.Label, card.Value, card.Color));
                }
            });
        }

        public static void ComposeSectionTitle(IContainer container, string title, string icon = "●")
        {
            container.PaddingTop(15).PaddingBottom(5).Row(row =>
            {
                row.AutoItem().Text(icon).FontSize(SectionTitleSize).FontColor(PrimaryHex);
                row.AutoItem().PaddingLeft(6).Text(title).FontSize(SectionTitleSize).Bold().FontColor(DarkHex);
            });
        }

        public static IContainer TableHeaderStyle(IContainer container)
        {
            return container
                .Background(PrimaryHex)
                .PaddingVertical(6)
                .PaddingHorizontal(8)
                .DefaultTextStyle(x => x.FontSize(NormalSize).Bold().FontColor(WhiteHex));
        }

        public static IContainer TableCellStyle(IContainer container, bool isAlternate)
        {
            var bg = isAlternate ? LightGrayHex : WhiteHex;
            return container
                .Background(bg)
                .BorderBottom(1)
                .BorderColor(LightGrayHex)
                .PaddingVertical(5)
                .PaddingHorizontal(8)
                .DefaultTextStyle(x => x.FontSize(NormalSize).FontColor(DarkHex));
        }

        #endregion
    }
}
