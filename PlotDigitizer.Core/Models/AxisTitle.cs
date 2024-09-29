namespace PlotDigitizer.Core
{
    public struct AxisTitle(string xLabel, string yLabel) 
    {
        public string XLabel { get; set; } = xLabel;
        public string YLabel { get; set; } = yLabel;
    }
}