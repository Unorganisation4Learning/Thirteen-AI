namespace Game.Core.Utilities
{
    public static class ArrayUtils
    {
        public static int Convert2DTo1D(int x, int y, int width)
        {
            return x + y * width;
        }

        public static bool Convert1DTo2D(int id, int width, int height, out int x, out int y)
        {
            x = 0;
            y = 0;

            if (width == 0 || height == 0 || id < 0 || id >= width * height)
            {
                return false;
            }

            int edge = width > height ? width : height;
            if (width > height)
            {
                x = id % edge;
                y = id / edge;
            }
            else
            {
                x = id / edge;
                y = id % edge;
            }

            return true;
        }

        public static bool Contains<T>(this T[] arr, T item) where T : System.IEquatable<T>
        {
            return arr.FindIndex(item) >= 0;
        }

        public static int FindIndex<T>(this T[] arr, T item) where T : System.IEquatable<T>
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Equals(item))
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool Arr2DCopyTo<T>(this T[,] origin, T[,] target)
        {
            if (origin.Length == target.Length)
            {
                int width = origin.GetLength(0);
                int height = origin.GetLength(1);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        target[x, y] = origin[x, y];
                    }
                }
                return true;
            }
            return false;
        }
    }
}
