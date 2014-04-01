using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdaBoost;

namespace Program
{
    class Program
    {
        private static readonly Random Rand = new Random();

        static void Main(string[] args)
        {
            try
            {
                for (int noise = 0; noise < 2; noise++)
                {
                    double error = 0.05 * noise;

                    List<Sample> pointsPos, pointsNeg;

                    int flippedLabels = GenerateSamples(error, out pointsPos, out pointsNeg);

                    Trainer<Sample> t = new Trainer<Sample>(new AdaBoost.ILearner<Sample>[] { new Sample.Learner(21) }, pointsPos, pointsNeg);

                    int convergence = 0;
                    int prev = 0;
                    Console.WriteLine("Noise: " + error + " (" + flippedLabels + " label errors)");
                    for (int i = 0; i < 1000; i++)
                    {
                        float cost = t.AddLayer();

                        if (float.IsNaN(cost))
                        {
                            Console.WriteLine("Terminated after " + i + " layers.");
                            break;
                        }

                        //Console.WriteLine("Loss = " + (float)cost);
                        int testErrors = TestTrainer(t.Classifier, pointsPos, pointsNeg);
                        int veriErrors = TestTrainer(t.Classifier);

                        if (testErrors == prev) convergence++;
                        else convergence = 0;
                        if (convergence > 19)
                        {
                            Console.WriteLine("Fully trained after " + (i - 9) + " layers.");
                            break;
                        }
                        else if ((i + 1) % 10 == 0)
                        {
                            Console.WriteLine("[" + testErrors + " / " + veriErrors + " ] errors after " + (i + 1) + " layers.");
                        }
                        prev = testErrors;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadKey();
        }

        private static int GenerateSamples(double error, out List<Sample> pointsPos, out List<Sample> pointsNeg)
        {
            int flippedLabels = 0;
            pointsPos = new List<Sample>();
            pointsNeg = new List<Sample>();


            flippedLabels += AddSamples(1000, LargeMargins, error, pointsPos, pointsNeg);
            flippedLabels += AddSamples(1000, Pullers, error, pointsPos, pointsNeg);
            flippedLabels += AddSamples(2000, Penalisers, error, pointsPos, pointsNeg);

            return flippedLabels;
        }

        private static Sample Penalisers(bool pos)
        {
            Sample s = new Sample(21);
            for (int j = 0; j < 11; j++)
            {
                int loc = Rand.Next(j + 1);
                s.Features[j] = s.Features[loc];
                s.Features[loc] = (pos ^ j > 4) ? 1 : -1;
            }
            for (int j = 11; j < 21; j++)
            {
                int loc = Rand.Next(j - 10);
                s.Features[j] = s.Features[11 + loc];
                s.Features[11 + loc] = (pos ^ j > 16) ? 1 : -1;
            }
            return s;
        }

        private static Sample Pullers(bool pos)
        {
            Sample s = new Sample(21);
            for (int j = 0; j < 21; j++)
            {
                s.Features[j] = (pos ^ j > 10) ? 1 : -1;
            }
            return s;
        }

        private static Sample LargeMargins(bool pos)
        {
            var s = new Sample(21);
            for (int j = 0; j < 21; j++)
            {
                s.Features[j] = pos ? 1 : -1;
            }
            return s;
        }

        private static int AddSamples(int count, Func<bool, Sample> featureGenerator, double error, List<Sample> pointsPos, List<Sample> pointsNeg)
        {
            int flippedLabels = 0;
            for (int i = 0; i < count; i++)
            {
                bool label = Rand.NextDouble() < 0.5;
                Sample s = featureGenerator(label);
                if (Rand.NextDouble() < error)
                {
                    label = !label;
                    flippedLabels++;
                }
                if (label) pointsPos.Add(s);
                else pointsNeg.Add(s);
            }
            return flippedLabels;
        }

        static Encoding _encoding = new UnicodeEncoding();

        static int TestTrainer(Classifier<Sample> c, IEnumerable<Sample> pointsPos, IEnumerable<Sample> pointsNeg)
        {
            //Console.WriteLine(errors / 40f + "% error rate.");
            //Console.WriteLine();
            return pointsPos.Count(s => c.Classify(s) <= 0) + pointsNeg.Count(s => c.Classify(s) >= 0);
        }

        static int TestTrainer(Classifier<Sample> c)
        {
            List<Sample> pointsPos, pointsNeg;

            GenerateSamples(0, out pointsPos, out pointsNeg);

            return TestTrainer(c, pointsPos, pointsNeg);
        }
    }
}
