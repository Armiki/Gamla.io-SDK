using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GamlaSDK
{
    public static class Tools
    {
        public static string ToJsonString<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            var str = new StringBuilder();
            str.Append("{");
            foreach (var pair in dict)
            {
                str.Append(String.Format(" \"{0}\"=\"{1}\", ", pair.Key, pair.Value));
            }

            str.Append("}");
            return str.ToString();
        }
        
        public static Transform ClearChilds(this Transform transform)
        {
            foreach (Transform child in transform) {
                GameObject.Destroy(child.gameObject);
            }
            return transform;
        }
        
        public static RectTransform GetRect(this Transform transform)
        {
            return transform.GetComponent<RectTransform>();
        }
    }
}