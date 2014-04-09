using System.Collections.Generic;

namespace AdaBoost
{
    /// <summary>
    /// Weak learner; outputs some hypothesis about a given sample.
    /// </summary>
    /// <typeparam name="TSample">The type of sample that can be classified by this learner.</typeparam>
    public interface ILearner<in TSample> where TSample : ISample
    {

        /// <summary>
        /// Makes a prediction about the previously set sample according to previously set parameters.
        /// </summary>
        /// <returns>A real value with sign equal to the expected class of the object and absolute
        /// value equal to strength of the prediction.</returns>
        float Classify();

        /// <summary>
        /// Sets the sample to be examined. As some processing may only need to be performed once
        /// for each sample, the learner may test whether s is equal to the previously set sample,
        /// and skip that processing if it is. If costly resources (eg. large arrays or temporary
        /// files) are allocated for each sample, changing the sample will free or reuse resources
        /// allocated for the previous sample.
        /// </summary>
        /// <param name="value">The sample, which may be the same as the the previously set sample.</param>
        void SetSample(TSample value);

        /// <summary>
        /// Sets parameters for the following classification. A boost classifier may contain many
        /// references to the same learner instance, and test each sample many consecutive times,
        /// changing only the parameters each time.
        /// </summary>
        /// <param name="configuration">A dictionary of parameters to be parsed according to some
        /// predetermined scheme.</param>
        ILearner<TSample> WithConfiguration(string configuration);

        /// <summary>
        /// For improved performance, the output of this learner may be cached. This method allows the learner
        /// to be cached by string index.
        /// </summary>
        /// <value>A string that uniquely identifies this learner.</value>
        string UniqueId { get; }

        /// <summary>
        /// During training, it may be desirable to iterate over every possible configuration of the learner.
        /// </summary>
        /// <returns>an iterable object containing every valid configuration combination for the learner.</returns>
        IEnumerable<string> AllPossibleConfigurations();

        string Config { get; }
    }
}
