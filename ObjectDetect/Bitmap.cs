namespace ObjectDetect
{
    class Bitmap<T>
    {
        private readonly int stride;
        private readonly T[] data;
        public readonly int Width;
        public readonly int Height;


        public Bitmap(T[] data, int width, int height) : this(data, width, height, width) { }

        public Bitmap(T[] data, int width, int height, int stride)
        {
            this.data = data;
            this.Width = width;
            this.Height = height;
            this.stride = stride;
        }

        public T this[int x, int y] { get { return data[y * stride + x]; } }
    }
}
