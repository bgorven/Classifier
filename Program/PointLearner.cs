using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaBoost;

namespace ConsoleTrainer
{
    class PointLearner : ILearner<Point>
    {
        float ILearner<Point>.classify()
        {
            return (float)(Math.Cos(parameters.rotation) * sample.x - Math.Sin(parameters.rotation) * sample.y);
            
        }

        private Point sample;
        void ILearner<Point>.setSample(Point s)
        {
            sample = s;
        }

        string ILearner<Point>.getUniqueIDString()
        {
            return "PointLearner";
        }

        struct PointLearnerConfig : Configuration<ILearner<Point>, Point>
        {
            internal float rotation;

            public PointLearnerConfig(float theta)
            {
                rotation = theta;
            }

            public override string ToString()
            {
                return "rotation = " + rotation;
            }
        }

        PointLearnerConfig parameters;
        void ILearner<Point>.setParams(Configuration<ILearner<Point>, Point> parameters)
        {
            this.parameters = (PointLearnerConfig) parameters;
        }

        IEnumerable<Configuration<ILearner<Point>, Point>> ILearner<Point>.getPossibleParams()
        {
            float delta = 1/128f;
                for (float th = 0; th < Math.PI; th += delta)
                    yield return new PointLearnerConfig(th);
            yield break;
        }

        public int getFeature() { return 0; }
    }
}
