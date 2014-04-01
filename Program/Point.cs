using AdaBoost;

namespace Program
{
    struct Point : ISample
    {
        public float x;
        public float y;

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
