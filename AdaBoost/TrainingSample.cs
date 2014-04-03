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
        public float Confidence { get { return _confidenceP.S + _confidenceN.S - _confidenceP.C - _confidenceN.C; } }

        internal void AddConfidence(float c)
        {
            if (c > 0) _confidenceP.Add(c);
            else _confidenceN.Add(c);
        }
    }
}
