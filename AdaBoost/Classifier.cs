using System.Collections.Generic;
using System.Linq;

namespace AdaBoost
{
    /// <summary>
    /// A boosted classifier that can operate on the specified type of sample.
    /// </summary>
    /// <typeparam name="TSample">an implementation of the ISample interface such that learners of type <c>ILearner&lt;Sample&gt;</c>
    /// will be able to classify objects of this type.</typeparam>
    public class Classifier<TSample> where TSample : ISample
    {
        /// <summary>
        /// Creates a new classifier with no weak Learners.
        /// </summary>
        public Classifier()
        {
            _layers = new List<Layer<TSample>>();
        }

        /// <summary>
        /// Creates a new classifier with layers copied from another classifier.
        /// </summary>
        /// <param name="prototype">The classifier to copy.</param>
        public Classifier(Classifier<TSample> prototype)
        {
            _layers = new List<Layer<TSample>>(prototype._layers);
        }

        private readonly List<Layer<TSample>> _layers;

        /// <summary>
        /// Adds a specific weak learner to the classifier. Typically called at the end of each training iteration.
        /// </summary>
        /// <param name="l">The layer to be added. This will be a weak learner with a specific set of parameters.</param>
        internal void AddLayer(Layer<TSample> l) {
            _layers.Add(l);
        }

        /// <summary>
        /// Classifies an object. This function may terminate early if the confidence reaches a determined level.
        /// </summary>
        /// <param name="s">The object to classify.</param>
        /// <returns>The sign of the return value is the prediction, and it's absolute value is the confidence.</returns>
        public float Classify(TSample s) {
            return Classify(s, true);
        }

        /// <summary>
        /// Classifies an object, optionally terminating at a predefined confidence level.
        /// </summary>
        /// <param name="s">The object to classify.</param>
        /// <param name="canEarlyTerminate">If this value is true, the object will only be tested against enough weak
        /// learners to reach a predetermined confidence level.</param>
        /// <returns>The sign of the return value is the prediction, and it's absolute value is the confidence.</returns>
        public float Classify(TSample s, bool canEarlyTerminate)
        {
            float confidence = 0;
            foreach (Layer<TSample> l in _layers) {
                confidence += l.Classify(s);
                if (canEarlyTerminate) break;
            }

            return confidence;
        }

        /// <summary>
        /// provides a list of the layers that this classifier is composed of.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _layers.Aggregate("", (current, l) => current + (l + ";\n"));
        }
    }
}
