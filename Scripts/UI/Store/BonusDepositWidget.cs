using System;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class BonusDepositWidget : MonoBehaviour
    {
        public event Action onBonusClick;
        
        [SerializeField] private GameObject _stars;
        [SerializeField] private RecolorItem _textRecolor;
        [SerializeField] private RecolorItem _logoRecolor;
        [SerializeField] private Text _timer;
        [SerializeField] private GameObject _arrow;
        [SerializeField] private Button _button;

        private bool _isActive;
        private DateTime? _endTime = null;
        private long _timerCount;
        public void Start()
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() =>
            {
                onBonusClick?.Invoke();
            });
        }

        public void Update()
        {
            if (!_isActive &&  _timerCount > 0)
            {
                //_timer.text = String.Format("{0:h HH}", (_endTime - DateTime.UtcNow).Value);
                //_timer.text = (_endTime - DateTime.UtcNow).Value.
                var hours = (_endTime - DateTime.UtcNow).Value.Hours;
                var minutes = (_endTime - DateTime.UtcNow).Value.Minutes;
                var sek = (_endTime - DateTime.UtcNow).Value.Seconds;
                _timer.text =  $"{hours}:{minutes}:{sek}";
            }
        }

        public void Init(bool isActive, DateTime? endTime = null, long  timer = 0)
        {
            if (timer == -1)
            {
                _timer.text = LocalizationManager.Text("gamla.main.notavailable");
            }
            
            TimeSpan time = TimeSpan.FromSeconds(timer);
            DateTime dateTime = DateTime.Now.Add(time);
            //string displayTime = dateTime.ToString("hh:mm:tt");
            
            _endTime = dateTime;
            _isActive = isActive;
            _stars.SetActive(isActive);
            _arrow.SetActive(isActive);
            _timer.gameObject.SetActive(!isActive);
            _textRecolor.recolorFilter = isActive ? "textColorPrimary" : "noActiveColorPrize";
            _logoRecolor.recolorFilter = isActive ? "specialColor" : "noActiveColorPrize";
            _textRecolor.Recolor();
            _logoRecolor.Recolor();
            _timerCount = timer;
        }
    }
}