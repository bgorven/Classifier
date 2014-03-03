using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaBoost
{
    internal struct TrainingSample<Sample> where Sample : ISample
    {
        public TrainingSample(Sample s, int index, float weight, float actual)
        {
            this.sample = s;
            this.weight = weight;
            this.actual = actual;
            this.index = index;
            this._confidenceP = new KahanSum();
            this._confidenceN = new KahanSum();
        }

        public Sample sample;
        public float weight;
        public float actual;
        public int index;

        private KahanSum _confidenceP, _confidenceN;
        public float confidence { get { return _confidenceP.s + _confidenceN.s - _confidenceP.c - _confidenceN.c; } }

        internal void addConfidence(float c)
        {
            if (c > 0) _confidenceP.add(c);
            else _confidenceN.add(c);
        }
    }
}
