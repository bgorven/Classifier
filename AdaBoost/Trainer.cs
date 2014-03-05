using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace AdaBoost
{
    /// <summary>
    /// This class is used to produce a trained classifier.
    /// </summary>
    /// <typeparam name="Sample">The type of sample to be classified.</typeparam>
    public class Trainer<Sample> where Sample : ISample
    {
        const float coefLimit = 2;

        /// <summary>
        /// Gets the classifier in its current state.
        /// </summary>
        /// <returns>A copy of the current state of the classifier bieng trained.</returns>
        public Classifier<Sample> getClassifier()
        {
            return classifier;
        }

        /// <summary>
        /// Creates a boost classifier trainer.
        /// </summary>
        /// <param name="learners">An array of weak learners. Each weak learner may have multiple configurations that
        /// will be iterated through as each layer is added.</param>
        /// <param name="positiveSamples">The samples with which to train the learner.</param>
        /// <param name="negativeSamples">The samples with which to train the learner.</param>
        /// <param name="loss">A loss function, used to choose the best weak learner at each stage.</param>
        public Trainer(IEnumerable<ILearner<Sample>> learners, List<Sample> positiveSamples, List<Sample> negativeSamples)
        {
            this.learners = learners.ToArray();
            float count = positiveSamples.Count + negativeSamples.Count;
            this.positiveSamples = (positiveSamples.Select((s, i) => new TrainingSample<Sample>(s, i, 1f/count, 1f))).ToArray();
            int offset = positiveSamples.Count;
            this.negativeSamples = (negativeSamples.Select((s, i) => new TrainingSample<Sample>(s, i + offset, 1f/count, -1f))).ToArray();

            this.classifier = new Classifier<Sample>();


            this.cacheIndex = Interlocked.Increment(ref nextCacheIndex);

        }

        private static int nextCacheIndex = 0;
        private int cacheIndex;
        private static readonly MemoryCache cache = MemoryCache.Default;

        TrainingSample<Sample>[] positiveSamples, negativeSamples;

        private Classifier<Sample> classifier;

        ILearner<Sample>[] learners;


        /// <summary>
        /// Executes one iteration of the training process.
        /// </summary>
        /// <returns>the current value of the error function over all samples.</returns>
        public float addLayer()
        {
            bestLoss = float.PositiveInfinity;

            //Parallel.ForEach(learners, learner => bestConfiguration(learner));
            foreach (var learner in learners)
            {
                bestConfiguration(learner);
            }

            Array.Reverse(learners);

            if (this.bestLearner.HasValue)
            {
                var outputs = this.bestLearner.Value.Value;
                var layer = this.bestLearner.Value.Key;

                classifier.addLayer(layer);

                float oldLoss = 0, newLoss = 0;

                //Add new layer's output to each learner's cumulative score, recalculate weight, find highest weight
                for (int j = 0; j < positiveSamples.Length; j++)
                {
                    oldLoss += (float)Math.Exp(-positiveSamples[j].confidence);
                    positiveSamples[j].addConfidence(outputs[positiveSamples[j].index] > layer.threshold ? layer.coefPos : layer.coefNeg);
                    positiveSamples[j].weight = (float)Math.Exp(-positiveSamples[j].confidence);
                    newLoss += positiveSamples[j].weight;
                    j++;
                }
                for (int j = 0; j < negativeSamples.Length; j++)
                {
                    oldLoss += (float)Math.Exp(negativeSamples[j].confidence);
                    negativeSamples[j].addConfidence(outputs[negativeSamples[j].index] > layer.threshold ? layer.coefPos : layer.coefNeg);
                    negativeSamples[j].weight = (float)Math.Exp(negativeSamples[j].confidence);
                    newLoss += negativeSamples[j].weight;
                    j++;
                }

#if DEBUG
                if (newLoss > oldLoss) throw new Exception("Loss went up");
                if (!approxEqual(newLoss, bestLoss))
                {
                    throw new Exception("Weight calculation wrong");
                }
#endif
            }

            return bestLoss;
        }

        private bool approxEqual(float l, float r)
        {
            return (l + l / 64) > r && (l - l / 64) < r;
        }

        private Object bestLock = new Object();
        private KeyValuePair<Layer<Sample>, float[]>? bestLearner;
        private float bestLoss = float.PositiveInfinity;

        private float getLoss(float coefTrue, float coefFalse, IEnumerable<Tuple<float,bool>> predictions, float target)
        {
            double cost = 0;

            foreach (var p in predictions)
            {
                cost += Math.Exp(-(p.Item1 + (p.Item2 ? coefTrue : coefFalse)) * target);
            }

            return (float)cost;
        }

        private float sumWeights(TrainingSample<Sample>[] samples)
        {
            float sum = 0;

            Array.Sort(samples, (l, r) => l.weight.CompareTo(r.weight));

            for (int i = 0; i < samples.Length; i++)
            {
                sum += samples[i].weight;
            }

            return sum;
        }

        private void sumWeights(float[] weights, bool[] coefSelect, ref double sum1, ref double sum2)
        {
#if DEBUG
            if (weights.Length != coefSelect.Length) throw new Exception("weights.Length != coefSelect.Length");
#endif

            for (int i = 0; i < weights.Length; i++)
            {
                sum1 += coefSelect[i] ? weights[i] : 0;
                sum2 += coefSelect[i] ? 0 : weights[i];
            }
        }

        private void bestConfiguration(ILearner<Sample> learner)
        {
            string cacheKey = "<" + cacheIndex + ">" + learner.getUniqueIDString();
            Dictionary<Configuration<ILearner<Sample>, Sample>, float[]> predictions = cache[cacheKey] as Dictionary<Configuration<ILearner<Sample>, Sample>, float[]>;

            if (predictions == null)
            {
                predictions = getPredictions(learner);

                cache[cacheKey] = predictions;
            }

            Parallel.ForEach(
                predictions,
                () => new LayerHolder(float.PositiveInfinity, null, null),
                (p, s, best) => bestLayerSetup(learner, p.Key, p.Value, best),
                setBest
            );
        }

        private Dictionary<Configuration<ILearner<Sample>, Sample>, float[]> getPredictions(ILearner<Sample> learner)
        {
            var predictions = new Dictionary<Configuration<ILearner<Sample>, Sample>, float[]>();

            foreach (var s in positiveSamples.Concat(negativeSamples))
            {
                learner.setSample(s.sample);
                foreach (Configuration<ILearner<Sample>, Sample> config in learner.getPossibleParams())
                {
                    if (!predictions.ContainsKey(config))
                    {
                        predictions.Add(config, new float[positiveSamples.Length + negativeSamples.Length]);
                    }
                    learner.setParams(config);

                    predictions[config][s.index] = (float)learner.classify();
                }
            }
            return predictions;
        }

        private void setBest(LayerHolder best)
        {
            lock (bestLock)
            {
                if (best.loss < bestLoss)
                {
                    bestLearner = new KeyValuePair<Layer<Sample>, float[]>(best.layer, best.values);
                    bestLoss = best.loss;
                }
            }
        }

        private struct LayerHolder
        {
            public float loss;
            public Layer<Sample> layer;
            public float[] values;

            public LayerHolder(float l, Layer<Sample> L, float[] v)
            {
                loss = l;
                layer = L;
                values = v;
            }
        }

        private LayerHolder bestLayerSetup(ILearner<Sample> learner, Configuration<ILearner<Sample>, Sample> configuration, float[] predictions, LayerHolder best)
        {
            LayerHolder result = optimizeCoefficients(learner, configuration, predictions);

#if DEBUG
            if (float.IsInfinity(result.loss))
            {
                ;//let me know
            }
#endif

            best = result.loss < best.loss ? result : best;

            return best;
        }

        private LayerHolder optimizeCoefficients(ILearner<Sample> learner, Configuration<ILearner<Sample>, Sample> configuration, float[] outputs)
        {
            float[] positiveWeights = positiveSamples.Select(s => s.weight).ToArray();
            float[] negativeWeights = negativeSamples.Select(s => s.weight).ToArray();

            bool[] positiveCorrect = new bool[positiveSamples.Length];
            bool[] negativeCorrect = new bool[negativeSamples.Length];

#if DEBUG
            bool perfect = true;
            learner.setParams(configuration);
            foreach (var s in positiveSamples.Concat(negativeSamples))
            {
                learner.setSample(s.sample);
                if (learner.classify() != outputs[s.index])
                {
                    throw new Exception();
                }
                perfect &= s.actual == outputs[s.index];
            }
#endif

            //Find best bias point, adding one sample at a time to the predicted target set and updating the cost.
            //Note that -cost is the cost of the inverse of the current hypothesis i.e. "all samples are in the target".
            float bestThres = float.PositiveInfinity;
            float bestCost = float.PositiveInfinity;
            float bestPos = float.PositiveInfinity;
            float bestNeg = float.NegativeInfinity;
            float threshold = float.NegativeInfinity;

            while (threshold < float.PositiveInfinity)
            {
                {
                    int i;
                    i = 0;
                    foreach (var s in positiveSamples) positiveCorrect[i++] = outputs[s.index] > threshold;
                    i = 0;
                    foreach (var s in negativeSamples) negativeCorrect[i++] = outputs[s.index] <= threshold;
                }

                double sumWeightsPosCorrect = 0;
                double sumWeightsPosIncorrect = 0;
                double sumWeightsNegCorrect = 0;
                double sumWeightsNegIncorrect = 0;

                sumWeights(positiveWeights, positiveCorrect, ref sumWeightsPosCorrect, ref sumWeightsPosIncorrect);
                sumWeights(negativeWeights, negativeCorrect, ref sumWeightsNegCorrect, ref sumWeightsNegIncorrect);

                float coefPos = (float)Math.Log(sumWeightsPosCorrect / sumWeightsNegIncorrect) / 2;
                float coefNeg = (float)Math.Log(sumWeightsNegCorrect / sumWeightsPosIncorrect) / -2;

                coefPos = float.IsNaN(coefPos) ? 0 : coefPos;
                coefNeg = float.IsNaN(coefNeg) ? 0 : coefNeg;

                float loss = (float)(sumWeightsPosCorrect * Math.Exp(-coefPos) + sumWeightsNegIncorrect * Math.Exp(coefPos));
                loss += (float)(sumWeightsNegCorrect * Math.Exp(coefNeg) + sumWeightsPosIncorrect * Math.Exp(-coefNeg));

#if DEBUG
                if (!approxEqual((float)(sumWeightsNegCorrect + sumWeightsNegIncorrect + sumWeightsPosCorrect + sumWeightsPosIncorrect), 1))
                {
                    ;
                }
                if (!perfect && float.IsInfinity(coefNeg) || float.IsInfinity(coefPos))
                {
                    throw new Exception("Unexpected coefficients");
                }

                //float calcLoss = 0;
                //calcLoss += getLoss(coefPos, coefNeg, positiveSamples.Select(s => s.confidence).Zip(positiveCorrect, (s, c) => new Tuple<float, bool>(s, c)), 1f);
                //calcLoss += getLoss(coefNeg, coefPos, negativeSamples.Select(s => s.confidence).Zip(negativeCorrect, (s, c) => new Tuple<float, bool>(s, c)), -1f);
#endif

                if (!float.IsNaN(loss) && loss < bestCost)
                {
                    bestCost = loss;
                    bestPos = coefPos;
                    bestNeg = coefNeg;
                    bestThres = threshold;
                }


                float next = float.PositiveInfinity;
                foreach (var v in outputs) if (v > threshold) next = Math.Min(v, next);
                threshold = next;
            }

            return new LayerHolder(bestCost, new Layer<Sample>(learner, configuration, bestPos, bestNeg, bestThres), outputs);
        }
    }
}
