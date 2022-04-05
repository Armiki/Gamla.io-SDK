using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.Data
{
    [CreateAssetMenu(fileName = "ColorTemplate", menuName = "ScriptableObjects/GUISettings", order = 0)]
    public class GUISettings : ScriptableObject
    {
        public Sprite soft_icon;
        public Sprite hard_icon;
        public Sprite ticket_icon;

        public List<Sprite> avatars;

        public List<FlagItem> storeIcons;

        public List<FlagItem> flags;
        
        public Sprite GetFlagByCurrentCulture()
        {
            var item = flags.FirstOrDefault(x => x.name == CultureInfo.CurrentCulture.Name);
            return item?.flag;
        }
    }

    [Serializable]
    public class FlagItem
    {
        public string name;
        public Sprite flag;
    }
    
}