using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaBoost
{
    /// <summary>
    /// Configuration parameters for a weak learner. Instances may be used as dictionary keys, so implementors
    /// of this interface should either be structs or override Object.Equals and Object.GetHashCode
    /// </summary>
    /// <typeparam name="Learner">The type of learner that this configuration is relevant to.</typeparam>
    /// <typeparam name="Sample">Same as learner type.</typeparam>
    public interface Configuration<Learner, Sample> where Learner : ILearner<Sample> where Sample : ISample { }
}
