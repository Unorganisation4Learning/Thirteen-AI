
namespace Game.Core.Utilities
{
    using UnityEngine;

    public static class TransformUtils
    {
        public static void SetDefault(this Transform trans)
        {
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
        }

        public static void SetParentThenSetDefault(this Transform trans, Transform parent)
        {
            trans.SetParent(parent);
            trans.SetDefault();
        }
    }
}
