using System;

namespace ObjectDetect
{
    class Bitmap<T>
    {
        private readonly T[][] _data;
        public readonly int Width;
        public readonly int Height;


        public Bitmap(T[][] data, int width, int height)
        {
            _data = data;
            Width = width;
            Height = height;
        }

        public Bitmap(T[] data, int width, int height, int stride)
        {
            if (data.Length != stride*height) throw new ArgumentException("data length should equal stride*height");
            Width = width;
            Height = height;
            _data = new T[height][];

            for (var y =0; y < _data.Length; y++)
            {
                _data[y] = new T[width];
                for (var x = 0; x < _data[y].Length; x++)
                {
                    _data[y][x] = data[y*stride + x];
                }
            }
        }

        public T this[int x, int y] { get { return _data[y][x]; } }
    }
}
