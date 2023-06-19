using Bytescout.Spreadsheet.Charts;
using ComponentAce.Compression.Libs.zlib;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.IO;
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
}

public class Program
{
    public static void Main()
    {

        EquationCalculator p = new EquationCalculator();

        List<double> Pr = new List<double>();

        for (double pr = 0.14; pr <= 0.99; pr += 0.01)
        {
            Pr.Add(pr);
        }

        List<double> R = new List<double>();

        Stack<int> dh = new Stack<int>();

        dh.Push(64);
        dh.Push(20);
        dh.Push(30);
        dh.Push(12);
        dh.Push(2);
        var plotModel = new PlotModel();

        // Create X and Y axes
        var xAxis = new LinearAxis { Position = AxisPosition.Bottom, Title = "Pr", Minimum = 0, Maximum = 1.2, MajorStep = 0.2 };
        var yAxis = new LinearAxis { Position = AxisPosition.Left, Title = "R", Minimum = -40, Maximum = 0, MajorStep = 10 };

        // Add axes to the plot model
        plotModel.Axes.Add(xAxis);
        plotModel.Axes.Add(yAxis);
        for (int v = 1; v <= 5; v++)
        {
            var d = dh.Pop();

            for (int i = 0; i < Pr.Count; i++)
            {
                R.Add(p.CalculateR(d, v, Pr[i], 8));
            }

            string s = v + "vrCode";
            var lineAnnotation = new LineAnnotation
            {
                Text = s
            };
            plotModel.Annotations.Add(lineAnnotation);  
            Plot(Pr, R, plotModel);
            R.Clear();
        }
    }

    static void Plot(List<double> Pr, List<double> R, PlotModel plotModel)
    {
    

        // Create a line series and add data points
        var series = new LineSeries();
        
        for (int i = 0; i < Pr.Count; i++)
        {
            series.Points.Add(new DataPoint(Pr[i], R[i]));
        }

        // Add the series to the plot model
        plotModel.Series.Add(series);

        // Create a PDF exporter with modified document size
        var pdfExporter = new PdfExporter { Width = 800, Height = 600 };
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
      

        Console.WriteLine("Plot saved as output.pdf");
    }
}
