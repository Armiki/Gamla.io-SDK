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

        void LoadAva()
        {
            if (avatar != null && _tempUrl != null) {
                RemoteResourceManager.LoadToRawImage(avatar, _tempUrl);
            }
        }
    }
}
