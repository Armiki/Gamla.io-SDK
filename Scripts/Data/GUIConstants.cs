using UnityEngine;

namespace Gamla.Scripts.Data
{
    public static class GUIConstants
    {
        static ColorTemplate _colorTemplate;
        public static ColorTemplate colorTemplate
        {
            get
            {
                if (_colorTemplate == null) {
                    _colorTemplate = Resources.Load<ColorTemplate>("Settings/ColorTemplate");
                }

                return _colorTemplate;
            }
        }
        
        static GUISettings _guiSettings;
        public static GUISettings guiSettings
        {
            get
            {
                if (_guiSettings == null) {
                    _guiSettings = Resources.Load<GUISettings>("Settings/GUISettings");
                }

                return _guiSettings;
            }
        }
    }
}