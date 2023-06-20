using Bytescout.Spreadsheet.Charts;
using ComponentAce.Compression.Libs.zlib;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Drawing.Text;
using System.IO;
using System.Reflection.PortableExecutable;
using AxisPosition = OxyPlot.Axes.AxisPosition;

public class EquationCalculator
{
    public double CalculateH(double v, double pr, double M)
    {
        double numerator = pr + (v - 1) * (1 - pr) / (M - 1);
        //double denominator = M;
        //return numerator / denominator;

        return numerator;
    }

    public double CalculateN(double v, double pr, double M)
    {
        double term1 = (1 - CalculateH(v, pr, M)) / CalculateH(v, pr, M);
        double term2 = (1 - CalculateH(M - v, pr, M)) / CalculateH(M - v, pr, M);

        return term1 * term2;
    }

    public double CalculateR(double dh, double v, double pr, double M)
    {
        double logN = Math.Log(CalculateN(v, pr, M));
        return Math.Sqrt(dh) * logN / 2;
    }

    public Queue<OxyColor> colors = new Queue<OxyColor>();

    public EquationCalculator()
    {
        colors.Enqueue(OxyColors.Blue);

        colors.Enqueue(OxyColors.Orange);

        colors.Enqueue(OxyColors.Green);

        colors.Enqueue(OxyColors.Red);

        colors.Enqueue(OxyColors.Black);
    }
}

public class Program
{
    public static void Main()
    {

        int M = 5;

        EquationCalculator p = new EquationCalculator();

        List<double> Pr = new List<double>();

        for (double pr = 0.14; pr <= 0.99; pr += 0.01)
        {
            Pr.Add(pr);
        }

        List<double> R = new List<double>();

        Stack<int> dh = new Stack<int>();

        dh.Push(64); // exhastive
        dh.Push(20); // 4vr
        dh.Push(30); // 3vr
        dh.Push(3); // 2vr
        dh.Push(2); // 1vr
        var plotModel = new PlotModel();

        // Create X and Y axes
        var xAxis = new LinearAxis { Position = AxisPosition.Bottom, Title = "Pr when M equals " + M, Minimum = 0, Maximum = 1.1, MajorStep = 0.2 };
        var yAxis = new LinearAxis { Position = AxisPosition.Left, Title = "Evaluation function", Minimum = -40, Maximum = 0, MajorStep = 10 };

        // Add axes to the plot model
        plotModel.Axes.Add(xAxis);
        plotModel.Axes.Add(yAxis);
        for (int v = 1; v <= 5; v++)
        {
            var d = dh.Pop();

            for (int i = 0; i < Pr.Count; i++)
            {
                R.Add(p.CalculateR(d, v, Pr[i], M));
            }

            string s = v + "vrCode";
            OxyColor color = p.colors.Dequeue();

            Plot(Pr,R, plotModel, s, color);
            R.Clear();
        }
    }

    static void Plot(List<double> Pr, List<double> R, PlotModel plotModel, string s, OxyColor color)
    {
        // Create a line series and add data points
        var series = new LineSeries();

        series.Color = color;

        for (int i = 0; i < Pr.Count; i++)
        {
            series.Points.Add(new DataPoint(Pr[i], R[i]));
        }

        var lineAnnotation = new TextAnnotation
        {
            Text = s,
            TextPosition = new DataPoint(series.Points.Min(p => p.X), series.Points.Min(p => p.Y)),
            TextColor = color
            //TextHorizontalAlignment = HorizontalAlignment.Left,
            //TextVerticalAlignment = VerticalAlignment.Bottom
        };

        plotModel.Annotations.Add(lineAnnotation);

        // Add the series to the plot model
        plotModel.Series.Add(series);

        // Create a PDF exporter with modified document size
        var pdfExporter = new PdfExporter { Width = 800, Height = 500 };
        // Export the plot model to a PDF file
        if (File.Exists("output.pdf"))
        {
            using (var str = File.Open("output.pdf", FileMode.Open, FileAccess.Write))
            {

                pdfExporter.Export(plotModel, str);
            }
        }
        else
        {
            using (var stream = System.IO.File.Create("output.pdf"))
            {
                pdfExporter.Export(plotModel, stream);
            }
        }
    }
}
