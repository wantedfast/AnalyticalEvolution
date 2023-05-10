// See https://aka.ms/new-console-template for more information
using System;
using System.Runtime.ExceptionServices;
using System.IO;
using Bytescout.Spreadsheet;
using System.ComponentModel;

namespace AnalyticalEvolution
{ 
    class Program
    {
        // nvR Code (M=8, n=1,2,3,4)
        static int m = 8;
        //static int[] n = new int[] {1,2,3,4 };

        // Prci | xi  0.14,0.15,0.99
        static double[] pr = new double[1000];

        static int dh = 8;

        // stupid data structure
        static Dictionary<int, double> data = new Dictionary<int, double>();

        static Spreadsheet spreadsheet = new Spreadsheet();

        static Worksheet worksheet = spreadsheet.Workbook.Worksheets.Add("AnalyticalEvolution");

        static Queue<string> queue = new Queue<string>();


        // this function to calc the h
        public static double CalcH(double pr,int m, int v)
        {
            int temp = v - 1;

            double last = (1 - pr) / (m - 1);

            last = last * temp;

            double result = last + pr;

            return result;
        }

        public static void EquiPosteriorPro(string s, int v, double pr,int cellCount)
        {
            double h = CalcH(pr, m, v);
            
            //  this is A part
            double firstPart = (1 - h) / h;
            
            // B part
            double temp = CalcH(pr, m, m-v);
            double secondPart = (1 - temp) / temp;
             
            //
            double seta = firstPart*secondPart;

            double r = Math.Sqrt(dh) * Math.Log(seta);
            
            r = r / 2;

            worksheet.Cell(s + cellCount).Value = r;
      
            //cellCount++;
        }
        
        // useless function
        static void IntialArray()
        {
            pr[0] = 0.14;

            int i = 1;
            while (pr[i] <= 1)
            {
                pr[++i] = pr[i] + 0.14;
            }
        }

        public static void Main()
        {
            queue.Enqueue("A");
            queue.Enqueue("B");
            queue.Enqueue("C");
            queue.Enqueue("D");
            
            pr[0] = 0.14;
            int j = 0;
            int cellNumber = 1;

            while(queue.Count()!=0)
            {
                string s = queue.Dequeue();
                worksheet.Cell(s + 1).Value = cellNumber + "vrCode";
                
                int cellCount = 2;

                // in this loop the pr will be changed
                while (pr[j] <= 1)
                {
                    EquiPosteriorPro(s, cellNumber, pr[j], cellCount++);
                    pr[j + 1] = pr[j] + 0.01;
                    j++;
                }

                cellNumber++;
                Array.Clear(pr, 1, 999);
            }

            spreadsheet.SaveAs(@"C:\Users\ITA\Desktop\task2.xlsx");

            //WriteExcel(data);
        }  
    }
}

