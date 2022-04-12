using System.Collections.Generic;
using Gamla.Data;
using Gamla.Logic;
using UnityEngine;


namespace Gamla.UI
{
    public class TrophiesWindow : GUIView
    {
        public const int SPACE = 60;
        
        [SerializeField] private RectTransform _content;
        [SerializeField] private TrophieWidgetExtra _pref;

        public void Start()
        {
            ServerCommand.GetTrophies(SetData);
        }

        public void SetData(ServerTrophies source)
        {
            _content.ClearChilds();
            float size = 0;
            var trophies = new List<ServerTrophiesModel>(source.trophies);
            trophies.AddRange(source.trophies_to_get);

            trophies.Sort((tr1, tr2) => tr2.SortValue.CompareTo(tr1.SortValue));
            
            foreach (var trophy in trophies)
            {
                var data = Instantiate(_pref, _content);
                data.Init(trophy);
                size += 300; //data.rect.rect.y;
            }

            _content.sizeDelta =
                new Vector2(0, size + (SPACE * (source.trophies.Count + source.trophies_to_get.Count - 1)));
        }
    }
}