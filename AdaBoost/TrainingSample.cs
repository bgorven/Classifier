using Utilities.Arithmetic;

namespace AdaBoost
{
    internal struct TrainingSample<TSample> where TSample : ISample
    {
        public TrainingSample(TSample s, int index, float weight, float actual)
        {
            Sample = s;
            Weight = weight;
            Actual = actual;
            Index = index;
            _confidenceP = new KahanSum();
            _confidenceN = new KahanSum();
        }

        public TSample Sample;
        public float Weight;
        public float Actual;
        public int Index;

        private KahanSum _confidenceP, _confidenceN;
        public float Confidence { get { return _confidenceP.Value + _confidenceN.Value - _confidenceP.Compensation - _confidenceN.Compensation; } }

        internal void AddConfidence(float c)
        {
            if (c > 0) _confidenceP.Add(c);
            else _confidenceN.Add(c);
        }
    }
}
