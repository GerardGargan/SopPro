namespace Backend.Models.Dto
{

    public class AnalyticsResponseDto
    {
        public ChartData LineData { get; set; }
        public ChartData BarData { get; set; }
        public List<PieChartData> PieData { get; set; }
        public List<SummaryCardData> SummaryCards { get; set; }
    }

    public class SummaryCardData
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public string Subtitle { get; set; }
    }

    public class ChartData
    {
        public List<string> Labels { get; set; }
        public List<ChartDataset> Datasets { get; set; }
    }

    public class ChartDataset
    {
        public List<int> Data { get; set; }
        public string Color { get; set; } = "rgba(0, 136, 254, 1)";
        public int StrokeWidth { get; set; } = 2;
    }

    public class PieChartData
    {
        public string Name { get; set; }
        public int Population { get; set; }
        public string Color { get; set; }
        public string LegendFontColor { get; set; } = "#7F7F7F";
        public int LegendFontSize { get; set; } = 12;
    }


}