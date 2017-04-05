namespace Generating
{
    public struct Texture2D
    {
        public int ID { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Texture2D(int id, int width, int height)
        {
            ID = id;
            Width = width;
            Height = height;
        }
    }
}