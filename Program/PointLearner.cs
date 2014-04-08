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

        public Point Sample
        {
            set { _sample = value; }
        }

        public string UniqueId
        {
            get { return "PointLearner"; }
        }

        public string Params { get { return _rotation.ToString(CultureInfo.InvariantCulture); } }

        public ILearner<Point> WithParams(string parameters)
        {
            var ret = this;
            float.TryParse(parameters, out ret._rotation);
            return ret;
        }

        public IEnumerable<string> GetPossibleParams()
        {
            const float delta = 1/128f;
            for (float theta = 0; theta < Math.PI; theta += delta) yield return theta.ToString(CultureInfo.InvariantCulture);
        }
    }
}
