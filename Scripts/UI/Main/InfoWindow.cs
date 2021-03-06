using Gamla.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class InfoWindow : GUIView
    {

        [SerializeField] private Text _title;
        [SerializeField] private Text _description;
        [SerializeField] private Text _actionTitle;
        [SerializeField] private Button _actionBtn;
        [SerializeField] private Text _cancelTitle;

        public void Init(GUIInfoType type)
        {
            if (!DataConstants.infoDict.ContainsKey(type)) return;

            Init(DataConstants.infoDict[type]);
        }

        public void Init(GUIInfoWinData data)
        {
            _title.text = data.title;
            _description.text = data.description;
            _actionTitle.text = data.actionTitle;
            _cancelTitle.text = data.closeTitle;
        }
    }
}