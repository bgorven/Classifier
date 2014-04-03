using AdaBoost;

namespace Program
{
    struct Point : ISample
    {
        public float X;
        public float Y;

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
