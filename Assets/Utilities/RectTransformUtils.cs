namespace Game.Core.Utilities
{
	using Rect = UnityEngine.Rect;
	using Vector2 = UnityEngine.Vector2;
	using RectTransform = UnityEngine.RectTransform;

	public static class RectTransformUtils
	{
		public static Rect GetRect(RectTransform child, RectTransform parent)
		{
			Rect res = Rect.zero;
			if (child && parent)
			{
				Rect parentRect = parent.rect;
				if (parentRect.IsZero())
				{
					return GetRect(child, parent.parent as RectTransform);
				}
				else
				{
					Vector2 anchorMin = child.anchorMin;
					Vector2 anchorMax = child.anchorMax;

					res.width = parentRect.width * (anchorMax.x - anchorMin.x);
					res.height = parentRect.height * (anchorMax.y - anchorMin.y);
					res.x = -res.width * 0.5f;
					res.y = -res.height * 0.5f;
				}
			}
			else if (parent == null)
			{
				res = child.rect;
			}
			return res;
		}
	}
}
