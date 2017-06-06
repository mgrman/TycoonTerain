﻿using UnityEngine;

namespace Votyra.Unity.Utils
{
    public static class GameObjectUtils
    {

        public static T GetOrAddComponent<T>(this GameObject gameObject)
            where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }
        public static void DestroyAllChildren(this GameObject gameObject)
        {
            gameObject.transform.DestroyAllChildren();
        }

        public static void DestroyAllChildren(this Transform transform)
        {
            int childCount = transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                child.gameObject.Destroy();
            }
        }

        public static void Destroy(this GameObject gameObject)
        {
            GameObject.DestroyImmediate(gameObject);
        }
    }
}