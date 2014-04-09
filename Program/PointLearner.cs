using System;
using System.Collections.Generic;
using System.Globalization;
using AdaBoost;

namespace Program
{
    struct PointLearner : ILearner<Point>
    {
        private float _rotation;

        public float Classify()
        {
            return (float)(Math.Cos(_rotation) * _sample.X - Math.Sin(_rotation) * _sample.Y);
        }

        private Point _sample;

        public void SetSample(Point value)
        {
            _sample = value;
        }

        public string UniqueId
        {
            get { return "PointLearner"; }
        }

        public string Config { get { return _rotation.ToString(CultureInfo.InvariantCulture); } }

        public ILearner<Point> WithConfiguration(string configuration)
        {
            var ret = this;
            float.TryParse(configuration, out ret._rotation);
            return ret;
        }

        public IEnumerable<string> AllPossibleConfigurations()
        {
            const float delta = 1/128f;
            for (float theta = 0; theta < Math.PI; theta += delta) yield return theta.ToString(CultureInfo.InvariantCulture);
        }
    }
}
