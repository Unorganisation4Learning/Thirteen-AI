namespace Game.Core.Utilities
{
	using System.Collections.Generic;

	public static class RandomUtils
	{
		public static int Random(int[] excludes, int max)
		{
			List<int> items = new List<int>();
			for (int i = 0; i < max; i++)
			{
				if (excludes != null && excludes.Contains(i))
				{
					continue;
				}

				items.Add(i);
			}


			if (items.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, items.Count);
				return items[index];
			}

			return -1;
		}

		public static T RandomEnum<T>() where T : System.Enum
		{
			System.Array list = System.Enum.GetValues(typeof(T));

			if (list.Length > 0)
			{
				int index = UnityEngine.Random.Range(0, list.Length);

				return (T)list.GetValue(index);
			}

			return default;
		}
	}
}
