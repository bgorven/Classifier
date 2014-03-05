using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using AdaBoost;
using System.IO;

namespace ConsoleTrainer
{
    class Program
    {

        static void Main(string[] args)
        {
            Task veriTask, testTask;

            using (FileStream veri = File.Open("verification.txt", FileMode.Create), test = File.Open("test.txt", FileMode.Create))
            {
                try
                {
                    veriTask = veri.WriteAsync(encoding.GetPreamble(), 0, encoding.GetPreamble().Length);
                    testTask = test.WriteAsync(encoding.GetPreamble(), 0, encoding.GetPreamble().Length);

                    for (int noise = 0; noise < 2; noise++)
                    {
                        double error = 0.05 * noise;
                        int flippedLabels = 0;

                        testTask.Wait();
                        testTask = test.WriteAsync(encoding.GetBytes(error.ToString()), 0, encoding.GetBytes(error.ToString()).Length);
                        veriTask.Wait();
                        veriTask = veri.WriteAsync(encoding.GetBytes(error.ToString()), 0, encoding.GetBytes(error.ToString()).Length);
                        

                        Random rand = new Random();
                        List<Sample> pointsPos = new List<Sample>();//from s in Enumerable.Range(0, 20) select new Point(rand.Nextfloat()*2, rand.Nextfloat()));
                        List<Sample> pointsNeg = new List<Sample>();//from s in Enumerable.Range(0, 20) select new Point(rand.Nextfloat()*2+1, rand.Nextfloat()));

                        //large margins
                        for (int i = 0; i < 1000; i++)
                        {
                            bool pos = rand.NextDouble() < 0.5;
                            Sample s = new Sample(21);
                            for (int j = 0; j < 21; j++)
                            {
                                s.features[j] = pos ? 1 : -1;
                            }
                            if (rand.NextDouble() < error)
                            {
                                flippedLabels++;
                                pos = !pos;
                            }
                            if (pos) pointsPos.Add(s);
                            else pointsNeg.Add(s);
                        }

                        //pullers
                        for (int i = 0; i < 1000; i++)
                        {
                            bool pos = rand.NextDouble() < 0.5;
                            Sample s = new Sample(21);
                            for (int j = 0; j < 21; j++)
                            {
                                s.features[j] = (pos ^ j > 10) ? 1 : -1;
                            }
                            if (rand.NextDouble() < error)
                            {
                                flippedLabels++;
                                pos = !pos;
                            }
                            if (pos) pointsPos.Add(s);
                            else pointsNeg.Add(s);
                        }

                        //penalisers
                        for (int i = 0; i < 2000; i++)
                        {
                            bool pos = rand.NextDouble() < 0.5;
                            Sample s = new Sample(21);
                            for (int j = 0; j < 11; j++)
                            {
                                int loc = rand.Next(j + 1);
                                s.features[j] = s.features[loc];
                                s.features[loc] = (pos ^ j > 4) ? 1 : -1;
                            }
                            if (s.features[1] == -1)
                            {
                                ;
                            }
                            for (int j = 11; j < 21; j++)
                            {
                                int loc = rand.Next(j - 10);
                                s.features[j] = s.features[11 + loc];
                                s.features[11 + loc] = (pos ^ j > 16) ? 1 : -1;
                            }
                            if (rand.NextDouble() < error)
                            {
                                flippedLabels++;
                                pos = !pos;
                            }
                            if (pos) pointsPos.Add(s);
                            else pointsNeg.Add(s);
                        }

                        Trainer<Sample> t = new Trainer<Sample>(new Sample(21).getLearnerArray(), pointsPos, pointsNeg);

                        int convergence = 0;
                        int prev = 0;
                        Console.WriteLine("Noise: " + error + " (" + flippedLabels + " label errors)");
                        for (int i = 0; i < 1000; i++)
                        {
                            float cost = t.addLayer();

                            if (float.IsNaN(cost))
                            {
                                Console.WriteLine("Terminated after " + i + " layers.");
                                break;
                            }

                            //Console.WriteLine("Loss = " + (float)cost);
                            int testErrors = testTrainer(t.getClassifier(), pointsPos, pointsNeg, test, testTask);
                            int veriErrors = testTrainer(t.getClassifier(), veri, veriTask);

                            if (prev == 0 && testErrors > 0)
                            {
                                ;
                            }

                            if (testErrors == 0) convergence++;
                            else convergence = 0;
                            if (convergence > 19)
                            {
                                Console.WriteLine("Fully trained after " + (i-9) + " layers.");
                                break;
                            }
                            else if ((i+1) % 10 == 0)
                            {
                                Console.WriteLine("[" + testErrors + " / " + veriErrors + " ] errors after " + (i+1) + " layers.");
                            }
                            prev = testErrors;
                        }

                        testTask.Wait();
                        testTask = test.WriteAsync(encoding.GetBytes("\n"), 0, encoding.GetBytes("\n").Length);
                        veriTask.Wait();
                        veriTask = veri.WriteAsync(encoding.GetBytes("\n"), 0, encoding.GetBytes("\n").Length);
                    }

                    testTask.Wait();
                    veriTask.Wait();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            Console.ReadKey();
        }

        static Encoding encoding = new UnicodeEncoding();

        static int testTrainer(Classifier<Sample> c, List<Sample> pointsPos, List<Sample> pointsNeg, FileStream file, Task writeTask)
        {
            int errors = 0;
            foreach (var s in pointsPos)
            {
                if (c.classify(s) <= 0)
                {
                    errors++;
                }
            }
            foreach (var s in pointsNeg)
            {
                if (c.classify(s) >= 0)
                {
                    errors++;
                }
            }

            writeTask.Wait();
            writeTask = file.WriteAsync(encoding.GetBytes("," + errors), 0, encoding.GetBytes("," + errors).Length);
            //Console.WriteLine(errors / 40f + "% error rate.");
            //Console.WriteLine();
            return errors;
        }

        static int testTrainer(Classifier<Sample> c, FileStream file, Task writeTask)
        {

            Random rand = new Random();
            List<Sample> pointsPos = new List<Sample>();//from s in Enumerable.Range(0, 20) select new Point(rand.NextDouble()*2, rand.NextDouble()));
            List<Sample> pointsNeg = new List<Sample>();//from s in Enumerable.Range(0, 20) select new Point(rand.NextDouble()*2+1, rand.NextDouble()));

            //large margins
            for (int i = 0; i < 1000; i++)
            {
                bool pos = rand.NextDouble() < 0.5;
                Sample s = new Sample(21);
                for (int j = 0; j < 21; j++)
                {
                    s.features[j] = pos ? 1 : -1;
                }
                if (pos) pointsPos.Add(s);
                else pointsNeg.Add(s);
            }

            //pullers
            for (int i = 0; i < 1000; i++)
            {
                bool pos = rand.NextDouble() < 0.5;
                Sample s = new Sample(21);
                for (int j = 0; j < 21; j++)
                {
                    s.features[j] = (pos ^ j > 10) ? 1 : -1;
                }
                if (pos) pointsPos.Add(s);
                else pointsNeg.Add(s);
            }

            //penalisers
            for (int i = 0; i < 2000; i++)
            {
                bool pos = rand.NextDouble() < 0.5;
                Sample s = new Sample(21);
                for (int j = 0; j < 11; j++)
                {
                    int loc = rand.Next(j + 1);
                    s.features[j] = s.features[loc];
                    s.features[loc] = (pos ^ j > 4) ? 1 : -1;
                }
                for (int j = 11; j < 21; j++)
                {
                    int loc = rand.Next(j - 10);
                    s.features[j] = s.features[11 + loc];
                    s.features[11 + loc] = (pos ^ j > 16) ? 1 : -1;
                }
                if (pos) pointsPos.Add(s);
                else pointsNeg.Add(s);
            }

            return testTrainer(c, pointsPos, pointsNeg, file, writeTask);
        }
    }
}
