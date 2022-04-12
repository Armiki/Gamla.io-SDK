using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamla.Data
{
    [CreateAssetMenu(fileName = "ColorTemplate", menuName = "ScriptableObjects/ColorTemplate", order = 1)]
    public class ColorTemplate : ScriptableObject
    {
        public List<ColorInfo> colors;
    }
    
    [Serializable]
    public class ColorInfo
    {
        public bool use;
        public string name;
        public Sprite gameTheme;
        public Sprite gameView;
        
        public RecolorGradient loadingColor;
        public RecolorGradient  specialColor;
        
        public Color viewPrimaryColor;
        public Color viewSecondaryColor;
        public Color viewTertiaryColor;
        public Color viewDividerPrimaryColor;
        public Color viewDividerColorTwo;
        
        public Color cellBackgroundColor;
        public Color cellAlternateBackgroundColor;
        
        public Color listPrimaryColor;
        public Color listSecondaryColor;
        
        public Color blockPrimaryColor;
        public Color blockSecondaryColor;
        
        public Color progressBarColor;
        
        public Color buttonPrimaryColor;
        public Color buttonSecondaryColor;
        public Color buttonTertiaryColor;
        
        public RecolorGradient buttonSpecialColor;
        public RecolorGradient buttonAnimSpecialColor;
        
        public Color filterOnColor;
        public Color filterOffColor;
        
        public Color topBarColor;
        public Color tabBarColor;
        
        public Color softColor;
        public Color softBackColor;
        
        public Color softColorSecondary;
        public Color softBackColorSecondary;
        
        public Color softColorSpecial;
        public RecolorGradient softBackColorSpecial;
        
        public Color hardColor;
        public Color hardBackColor;
        
        public Color hardColorSecondary;
        public Color hardBackColorSecondary;
        
        public Color hardColorSpecial;
        public Color hardBackColorSpecialMono;
        public RecolorGradient hardBackColorSpecial;
        
        public Color ticketColorSpecial;
        public RecolorGradient ticketBackColorSpecial;

        public Color backColorPrize;
        public Color backRecolorColorPrize;
        public Color colorPrize;
        public Color noActiveColorPrize;
        public RecolorGradient specialColorPrize;

        public Color textColorPrimary;
        public Color textColorSecondary;
        public Color textColorTertiary;
        public Color textColorFour;
        public Color textColorError;
        public Color textColorPrize;
        public Color textColorCash;
        public Color textColorLink;
        
        public Color textColorSoftSpecial;
        public Color textColorHardSpecial;
        public Color textColorTicketSpecial;
    }

    [Serializable]
    public class RecolorGradient
    {
        public Color color_1;
        public Color color_2;
        public float angel;
        public bool isWaveColor;
        public float fromAngel;
        public float toAngel;
    }
}