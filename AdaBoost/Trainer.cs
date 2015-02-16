using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public Trainer(IEnumerable<ILearner<TSample>> learners, IEnumerable<TSample> positiveSamples, IEnumerable<TSample> negativeSamples)
        {
            _learners = learners.ToArray();
            _positiveSamples = (positiveSamples.Select((s, i) => new TrainingSample<TSample>(s, i, 1f, 1f))).ToArray();
            var offset = _positiveSamples.Length;
            _negativeSamples = (negativeSamples.Select((s, i) => new TrainingSample<TSample>(s, i + offset, 1f, -1f))).ToArray();

            Classifier = new Classifier<TSample>();

        }

        private readonly Dictionary<string, IDictionary<string, float[]>> _cache =
            new Dictionary<string, IDictionary<string, float[]>>();

        readonly TrainingSample<TSample>[] _positiveSamples;
        readonly TrainingSample<TSample>[] _negativeSamples;

        readonly ILearner<TSample>[] _learners;

        private CancellationToken _cancellation;
        private IProgress<Tuple<string, int>> _progress;

        /// <summary>
        /// Executes one iteration of the training process.
        /// </summary>
        /// <returns>the current value of the error function over all samples.</returns>
        public float AddLayer(CancellationToken cancellation = default(CancellationToken), IProgress<Tuple<string, int>> currentTaskAndPercentComplete = null)
        {
            _cancellation = cancellation;
            _progress = currentTaskAndPercentComplete;

            _bestLoss = float.PositiveInfinity;
            double loss = 0;

            //Parallel.ForEach(learners, learner => bestConfiguration(learner));
            foreach (var learner in _learners)
            {
                BestConfiguration(learner);
            }

            if (_cancellation.IsCancellationRequested)
            {
                return float.NaN;
            }

            //Reversing the learner list each iteration means our mru cache will behave more like an lru cache
            Array.Reverse(_learners);

            //Summing sorted values is more numerically stable than summing unsorted values. This operation only needs to be
            //performed once per iteration, and improves the accuracy of all subsequent summations during weak leraner selection.
            Array.Sort(_positiveSamples, (l, r) => l.Weight.CompareTo(r.Weight));
            Array.Sort(_negativeSamples, (l, r) => l.Weight.CompareTo(r.Weight));

            if (_bestLearner.HasValue && _bestLoss < _prevLoss)
            {
                var outputs = _bestLearner.Value.Value;
                var layer = _bestLearner.Value.Key;

                Classifier.AddLayer(layer);

                //Add new layer's output to each learner's cumulative score, recalculate weight, find highest weight
                for (var j = 0; j < _positiveSamples.Length; j++)
                {
                    _positiveSamples[j].AddConfidence(outputs[_positiveSamples[j].Index] > layer.Threshold ? layer.CoefPos : layer.CoefNeg);
                    loss += (_positiveSamples[j].Weight = (float)Math.Exp(-_positiveSamples[j].Confidence));
                }
                for (var j = 0; j < _negativeSamples.Length; j++)
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

            for (var i = 0; i < weights.Length; i++)
            {
                sum1 += coefSelect[i] ? weights[i] : 0;
                sum2 += coefSelect[i] ? 0 : weights[i];
            }
        }

        private void BestConfiguration(ILearner<TSample> learner)
        {
            var predictions = _cache.ContainsKey(learner.UniqueId) ? _cache[learner.UniqueId] : (_cache[learner.UniqueId] = GetPredictions(learner));

            var best = new LayerHolder(float.PositiveInfinity, null, null);

            var sampleCount = predictions.Count;
            var samplesProcessed = 0;

            foreach (var p in predictions)
            {
                best = BestLayerSetup(learner.WithConfiguration(p.Key), p.Value, best);
                SetBest(best);

                if (_cancellation.IsCancellationRequested)
                {
                    return;
                }
                if (_progress != null)
                {
                    samplesProcessed++;
                    _progress.Report(Tuple.Create("Finding best setup for \"" + learner.UniqueId + "\"", samplesProcessed * 100 / sampleCount));
                }
            }
            //Parallel.ForEach(
            //    predictions,
            //    () => new LayerHolder(float.PositiveInfinity, null, null),
            //    (p, s, best) => BestLayerSetup(learner.WithConfiguration(p.Key), p.Value, best),
            //    SetBest
            //);
        }

        private IDictionary<string, float[]> GetPredictions(ILearner<TSample> learner)
        {
            var predictions = new ConcurrentDictionary<string, float[]>();
            var sampleCount = _positiveSamples.Length + _negativeSamples.Length;
            var samplesProcessed = 0;

            foreach (var config in learner.AllPossibleConfigurations())
            {
                if (config.IndexOfAny(new[] { '>', '?', ':', '\r', '\n' }) > 0)
                {
                    throw new FormatException("Learner configuration cannot contain any of '>', '?', ':', or a line break.");
                }
                predictions.TryAdd(config, new float[sampleCount]);
            }

            Parallel.ForEach(_positiveSamples.Concat(_negativeSamples), s =>
            {
                var localLearner = learner;
                localLearner.SetSample(s.Sample);

                foreach (var config in learner.AllPossibleConfigurations())
                {
                    localLearner = localLearner.WithConfiguration(config);

                    predictions[config][s.Index] = learner.Classify();
                }

                if (_cancellation.IsCancellationRequested)
                {
                    return;
                }
                if (_progress != null)
                {
                    if (Monitor.TryEnter(_progress))
                    {
                        try
                        {
                            samplesProcessed++;
                            _progress.Report(Tuple.Create("Running learner \"" + learner.UniqueId + "\"", samplesProcessed * 100 / sampleCount));
                        }
                        finally
                        {
                            Monitor.Exit(_progress);
                        }
                    }
                }
            });
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
            var result = OptimizeCoefficients(learner, predictions);

#if DEBUG
            if (float.IsInfinity(result.Loss))
            {
// ReSharper disable once EmptyStatement
                ;//let me know
            }
#endif

            best = result.Loss < best.Loss ? result : best;

            return best;
        }

        private LayerHolder OptimizeCoefficients(ILearner<TSample> learner, float[] outputs)
        {
            var positiveWeights = _positiveSamples.Select(s => s.Weight).ToArray();
            var negativeWeights = _negativeSamples.Select(s => s.Weight).ToArray();

            var positiveCorrect = new bool[_positiveSamples.Length];
            var negativeCorrect = new bool[_negativeSamples.Length];

#if DEBUG
            var perfect = true;
            foreach (var s in _positiveSamples.Concat(_negativeSamples))
            {
                learner.SetSample(s.Sample);
// ReSharper disable CompareOfFloatsByEqualityOperator
                if (learner.Classify() != outputs[s.Index])
                {
                    throw new Exception();
                }
                perfect &= s.Actual == outputs[s.Index];
// ReSharper restore CompareOfFloatsByEqualityOperator
            }
#endif

            //Find best bias point, adding one sample at a time to the predicted target set and updating the cost.
            //Note that -cost is the cost of the inverse of the current hypothesis i.e. "all samples are in the target".
            var bestThres = float.PositiveInfinity;
            var bestCost = float.PositiveInfinity;
            var bestPos = float.PositiveInfinity;
            var bestNeg = float.NegativeInfinity;
            var threshold = float.NegativeInfinity;

            while (threshold < float.PositiveInfinity)
            {
                {
                    var i = 0;
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

                var coefPos = (float)Math.Log(sumWeightsPosCorrect / sumWeightsNegIncorrect) / 2;
                var coefNeg = (float)Math.Log(sumWeightsNegCorrect / sumWeightsPosIncorrect) / -2;

                coefPos = float.IsNaN(coefPos) ? 0 : coefPos;
                coefNeg = float.IsNaN(coefNeg) ? 0 : coefNeg;

                var loss = (float)(sumWeightsPosCorrect * Math.Exp(-coefPos) + sumWeightsNegIncorrect * Math.Exp(coefPos));
                loss += (float)(sumWeightsNegCorrect * Math.Exp(coefNeg) + sumWeightsPosIncorrect * Math.Exp(-coefNeg));

#if DEBUG
                if (!ApproxEqual((float)(sumWeightsNegCorrect + sumWeightsNegIncorrect + sumWeightsPosCorrect + sumWeightsPosIncorrect), 1))
                {
// ReSharper disable once EmptyStatement
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
