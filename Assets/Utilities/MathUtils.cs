namespace Game.Core.Utilities
{
    public static class MathUtils
    {
		public static bool IsEqualFloatingPoint(float a, float b, float epsilon)
		{
			float diff = System.Math.Abs(a - b);
			return diff <= epsilon;
		}

		public static bool IsFloatNumberZero(float value)
		{
			return IsEqualFloatingPoint(value, 0.0f, 0.00000001f);
		}

		public static bool IsZero(this UnityEngine.Rect rect)
		{
			if (rect == UnityEngine.Rect.zero)
			{
				return true;
			}

			if (
				IsEqualFloatingPoint(rect.x, 0.0f, 0.0001f) &&
				IsEqualFloatingPoint(rect.y, 0.0f, 0.0001f) &&
				IsEqualFloatingPoint(rect.width, 0.0f, 0.0001f) &&
				IsEqualFloatingPoint(rect.height, 0.0f, 0.0001f))
			{
				return true;
			}

			return false;
		}

		public static void Shuffle<T>(this System.Collections.Generic.IList<T> list)
		{
			System.Random r = new System.Random();
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = r.Next(0, n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}
}
