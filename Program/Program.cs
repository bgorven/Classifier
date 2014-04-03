using System;
using System.Collections.Generic;
using System.Linq;
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
                for (var noise = 0; noise < 2; noise++)
                {
                    var error = 0.05 * noise;

                    List<Sample> pointsPos, pointsNeg;

                    var flippedLabels = GenerateSamples(error, out pointsPos, out pointsNeg);

                    var t = new Trainer<Sample>(new ILearner<Sample>[] { new Sample.Learner(21) }, pointsPos, pointsNeg);

                    var convergence = 0;
                    var prev = 0;
                    Console.WriteLine("Noise: " + error + " (" + flippedLabels + " label errors)");
                    for (var i = 0; i < 1000; i++)
                    {
                        var cost = t.AddLayer();

                        if (float.IsNaN(cost))
                        {
                            Console.WriteLine("Terminated after " + i + " layers.");
                            break;
                        }

                        //Console.WriteLine("Loss = " + (float)cost);
                        var testErrors = TestTrainer(t.Classifier, pointsPos, pointsNeg);
                        var veriErrors = TestTrainer(t.Classifier);

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
            var flippedLabels = 0;
            pointsPos = new List<Sample>();
            pointsNeg = new List<Sample>();


            flippedLabels += AddSamples(1000, LargeMargins, error, pointsPos, pointsNeg);
            flippedLabels += AddSamples(1000, Pullers, error, pointsPos, pointsNeg);
            flippedLabels += AddSamples(2000, Penalisers, error, pointsPos, pointsNeg);

            return flippedLabels;
        }

        private static Sample Penalisers(bool pos)
        {
            var s = new Sample(21);
            for (var j = 0; j < 11; j++)
            {
                var loc = Rand.Next(j + 1);
                s.Features[j] = s.Features[loc];
                s.Features[loc] = (pos ^ j > 4) ? 1 : -1;
            }
            for (var j = 11; j < 21; j++)
            {
                var loc = Rand.Next(j - 10);
                s.Features[j] = s.Features[11 + loc];
                s.Features[11 + loc] = (pos ^ j > 16) ? 1 : -1;
            }
            return s;
        }

        private static Sample Pullers(bool pos)
        {
            var s = new Sample(21);
            for (var j = 0; j < 21; j++)
            {
                s.Features[j] = (pos ^ j > 10) ? 1 : -1;
            }
            return s;
        }

        private static Sample LargeMargins(bool pos)
        {
            var s = new Sample(21);
            for (var j = 0; j < 21; j++)
            {
                s.Features[j] = pos ? 1 : -1;
            }
            return s;
        }

        private static int AddSamples(int count, Func<bool, Sample> featureGenerator, double error, List<Sample> pointsPos, List<Sample> pointsNeg)
        {
            var flippedLabels = 0;
            for (var i = 0; i < count; i++)
            {
                var label = Rand.NextDouble() < 0.5;
                var s = featureGenerator(label);
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
