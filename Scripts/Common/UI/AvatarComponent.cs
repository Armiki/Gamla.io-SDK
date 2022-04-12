using System;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class AvatarComponent : MonoBehaviour
    {
        [SerializeField] public RawImage avatar;
        private Texture2D _temp;
        private string _tempUrl;

        public void Load(string url)
        {
            if (url == String.Empty) return;

            _tempUrl = url;

            LoadAva();
        }

        private void OnEnable()
        {
            LoadAva();
        }

        private void LoadAva()
        {
            if (_tempUrl != String.Empty)
            {
                RemoteResourceManager.GetContent(_tempUrl, texture2D =>
                {
                    if (texture2D != null)
                    {
                        _temp = texture2D;
                        if (avatar != null && gameObject != null && this != null)
                            avatar.texture = _temp;
                    }
                }, gameObject);
            }
        }
    }
}
