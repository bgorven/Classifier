using System;
using System.Collections.Generic;
using AdaBoost;
using ConsoleTrainer;

namespace Program
{
    struct PointLearner : AdaBoost.ILearner<Point>
    {
        private float rotation;

        public float classify()
        {
            return (float)(Math.Cos(rotation) * sample.x - Math.Sin(rotation) * sample.y);
        }

        private Point sample;
        public void setSample(Point s)
        {
            sample = s;
        }

        public string getUniqueIDString()
        {
            return "PointLearner";
        }

        public ILearner<Point> withParams(string parameters)
        {
            var ret = this;
            float.TryParse(parameters, out ret.rotation);
            return ret;
        }

        public IEnumerable<string> getPossibleParams()
        {
            const float delta = 1/128f;
            for (float theta = 0; theta < Math.PI; theta += delta) yield return theta.ToString();
        }
    }
}
