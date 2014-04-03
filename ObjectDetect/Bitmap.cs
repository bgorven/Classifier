namespace ObjectDetect
{
    class Bitmap<T>
    {
        private readonly int _stride;
        private readonly T[] _data;
        public readonly int Width;
        public readonly int Height;


        public Bitmap(T[] data, int width, int height) : this(data, width, height, width) { }

        public Bitmap(T[] data, int width, int height, int stride)
        {
            _data = data;
            Width = width;
            Height = height;
            _stride = stride;
        }

        public T this[int x, int y] { get { return _data[y * _stride + x]; } }
    }
}
