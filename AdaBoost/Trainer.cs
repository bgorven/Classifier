using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace AdaBoost
{
    /// <summary>
    /// This class is used to produce a trained classifier.
    /// </summary>
    /// <typeparam name="TSample">The type of sample to be classified.</typeparam>
    public class Trainer<TSample> where TSample : ISample
    {
        /// <summary>
        /// Gets the classifier in its current state.
        /// </summary>
        /// <returns>A copy of the current state of the classifier being trained.</returns>
        public Classifier<TSample> Classifier { get; private set; }

        /// <summary>
        /// Creates a boost classifier trainer.
        /// </summary>
        /// <param name="learners">An array of weak learners. Each weak learner may have multiple configurations that
        /// will be iterated through as each layer is added.</param>
        /// <param name="positiveSamples">The samples with which to train the learner.</param>
        /// <param name="negativeSamples">The samples with which to train the learner.</param>
        public Trainer(IEnumerable<ILearner<TSample>> learners, List<TSample> positiveSamples, List<TSample> negativeSamples)
        {
            this._learners = learners.ToArray();
            float count = positiveSamples.Count + negativeSamples.Count;
            this._positiveSamples = (positiveSamples.Select((s, i) => new TrainingSample<TSample>(s, i, 1f, 1f))).ToArray();
            int offset = positiveSamples.Count;
            this._negativeSamples = (negativeSamples.Select((s, i) => new TrainingSample<TSample>(s, i + offset, 1f, -1f))).ToArray();

            this.Classifier = new Classifier<TSample>();

        }

        private readonly Dictionary<string, Dictionary<string, float[]>> _cache =
            new Dictionary<string, Dictionary<string, float[]>>();

        readonly TrainingSample<TSample>[] _positiveSamples;
        readonly TrainingSample<TSample>[] _negativeSamples;

        readonly ILearner<TSample>[] _learners;


        /// <summary>
        /// Executes one iteration of the training process.
        /// </summary>
        /// <returns>the current value of the error function over all samples.</returns>
        public float AddLayer()
        {
            _bestLoss = float.PositiveInfinity;
            double loss = 0;

            //Parallel.ForEach(learners, learner => bestConfiguration(learner));
            foreach (var learner in _learners)
            {
                BestConfiguration(learner);
            }

            //Reversing the learner list each iteration means our mru cache will behave more like an lru cache
            Array.Reverse(_learners);

            //Summing sorted values is more numerically stable than summing unsorted values. This operation only needs to be
            //performed once per iteration, and improves the accuracy of all subsequent summations during weak leraner selection.
            Array.Sort(_positiveSamples, (l, r) => l.Weight.CompareTo(r.Weight));
            Array.Sort(_negativeSamples, (l, r) => l.Weight.CompareTo(r.Weight));

            if (this._bestLearner.HasValue && _bestLoss < _prevLoss)
            {
                var outputs = this._bestLearner.Value.Value;
                var layer = this._bestLearner.Value.Key;

                Classifier.AddLayer(layer);

                //Add new layer's output to each learner's cumulative score, recalculate weight, find highest weight
                for (int j = 0; j < _positiveSamples.Length; j++)
                {
                    _positiveSamples[j].AddConfidence(outputs[_positiveSamples[j].Index] > layer.Threshold ? layer.CoefPos : layer.CoefNeg);
                    loss += (_positiveSamples[j].Weight = (float)Math.Exp(-_positiveSamples[j].Confidence));
                }
                for (int j = 0; j < _negativeSamples.Length; j++)
                {
                    _negativeSamples[j].AddConfidence(outputs[_negativeSamples[j].Index] > layer.Threshold ? layer.CoefPos : layer.CoefNeg);
                    loss += (_negativeSamples[j].Weight = (float)Math.Exp(_negativeSamples[j].Confidence));
                }

#if DEBUG
                if (loss > _prevLoss) throw new Exception("Loss went up");
                if (!ApproxEqual((float)loss, _bestLoss))
                {
                    throw new Exception("Weight calculation wrong");
                }
#endif
                _prevLoss = (float)loss;
            }
            return _prevLoss;
        }

        private readonly Object _bestLock = new Object();
        private KeyValuePair<Layer<TSample>, float[]>? _bestLearner;
        private float _bestLoss = float.PositiveInfinity;
        private float _prevLoss = float.PositiveInfinity;

        private static bool ApproxEqual(float v1, float v2)
        {
            return v1*1.05 > v2 && v2 > v1*0.95;
        }

        private static void SumWeights(float[] weights, bool[] coefSelect, ref double sum1, ref double sum2)
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

        private void BestConfiguration(ILearner<TSample> learner)
        {
            string cacheKey = learner.GetUniqueIdString();
            Dictionary<string, float[]> predictions = _cache[cacheKey];

            if (predictions == null)
            {
                predictions = GetPredictions(learner);

                _cache[cacheKey] = predictions;
            }

            Parallel.ForEach(
                predictions,
                () => new LayerHolder(float.PositiveInfinity, null, null),
                (p, s, best) => BestLayerSetup(learner.WithParams(p.Key), p.Value, best),
                SetBest
            );
        }

        private Dictionary<string, float[]> GetPredictions(ILearner<TSample> learner)
        {
            var predictions = new Dictionary<string, float[]>();

            foreach (var s in _positiveSamples.Concat(_negativeSamples))
            {
                learner.SetSample(s.Sample);
                foreach (var config in learner.GetPossibleParams())
                {
                    if (!predictions.ContainsKey(config))
                    {
                        predictions.Add(config, new float[_positiveSamples.Length + _negativeSamples.Length]);
                    }
                    var temp = learner.WithParams(config);

                    predictions[config][s.Index] = temp.Classify();
                }
            }
            return predictions;
        }

        private void SetBest(LayerHolder best)
        {
            lock (_bestLock)
            {
                if (best.Loss < _bestLoss)
                {
                    _bestLearner = new KeyValuePair<Layer<TSample>, float[]>(best.Layer, best.Values);
                    _bestLoss = best.Loss;
                }
            }
        }

        private struct LayerHolder
        {
            public readonly float Loss;
            public readonly Layer<TSample> Layer;
            public readonly float[] Values;

            public LayerHolder(float loss, Layer<TSample> layer, float[] values)
            {
                Loss = loss;
                Layer = layer;
                Values = values;
            }
        }

        private LayerHolder BestLayerSetup(ILearner<TSample> learner, float[] predictions, LayerHolder best)
        {
            LayerHolder result = OptimizeCoefficients(learner, predictions);

#if DEBUG
            if (float.IsInfinity(result.Loss))
            {
                ;//let me know
            }
#endif

            best = result.Loss < best.Loss ? result : best;

            return best;
        }

        private LayerHolder OptimizeCoefficients(ILearner<TSample> learner, float[] outputs)
        {
            float[] positiveWeights = _positiveSamples.Select(s => s.Weight).ToArray();
            float[] negativeWeights = _negativeSamples.Select(s => s.Weight).ToArray();

            bool[] positiveCorrect = new bool[_positiveSamples.Length];
            bool[] negativeCorrect = new bool[_negativeSamples.Length];

#if DEBUG
            bool perfect = true;
            foreach (var s in _positiveSamples.Concat(_negativeSamples))
            {
                learner.SetSample(s.Sample);
                if (learner.Classify() != outputs[s.Index])
                {
                    throw new Exception();
                }
                perfect &= s.Actual == outputs[s.Index];
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
                    int i = 0;
                    foreach (var s in _positiveSamples) positiveCorrect[i++] = outputs[s.Index] > threshold;
                    i = 0;
                    foreach (var s in _negativeSamples) negativeCorrect[i++] = outputs[s.Index] <= threshold;
                }

                double sumWeightsPosCorrect = 0;
                double sumWeightsPosIncorrect = 0;
                double sumWeightsNegCorrect = 0;
                double sumWeightsNegIncorrect = 0;

                SumWeights(positiveWeights, positiveCorrect, ref sumWeightsPosCorrect, ref sumWeightsPosIncorrect);
                SumWeights(negativeWeights, negativeCorrect, ref sumWeightsNegCorrect, ref sumWeightsNegIncorrect);

                float coefPos = (float)Math.Log(sumWeightsPosCorrect / sumWeightsNegIncorrect) / 2;
                float coefNeg = (float)Math.Log(sumWeightsNegCorrect / sumWeightsPosIncorrect) / -2;

                coefPos = float.IsNaN(coefPos) ? 0 : coefPos;
                coefNeg = float.IsNaN(coefNeg) ? 0 : coefNeg;

                float loss = (float)(sumWeightsPosCorrect * Math.Exp(-coefPos) + sumWeightsNegIncorrect * Math.Exp(coefPos));
                loss += (float)(sumWeightsNegCorrect * Math.Exp(coefNeg) + sumWeightsPosIncorrect * Math.Exp(-coefNeg));

#if DEBUG
                if (!ApproxEqual((float)(sumWeightsNegCorrect + sumWeightsNegIncorrect + sumWeightsPosCorrect + sumWeightsPosIncorrect), 1))
                {
                    ;
                }
                if (!perfect && (float.IsInfinity(coefNeg) || float.IsInfinity(coefPos)))
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


                var next = outputs.Aggregate(float.PositiveInfinity, (min, i) => (i > threshold && i < min) ? i : min);
                threshold = next;
            }

            return new LayerHolder(bestCost, new Layer<TSample>(learner, bestPos, bestNeg, bestThres), outputs);
        }
    }
}
