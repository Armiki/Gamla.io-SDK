using System;
using Gamla.Data;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class CreateTournamentWindow : GUIView
    {
        public event Action<ServerCreateTournamentModel, Action<ServerCreateTournamentResult>> onCreateTournament;

        [SerializeField] private ValidateInputWidget _nameInput;
        [SerializeField] private Button _createTournamentBtn;
        [SerializeField] private Slider _playerNumSlider;
        [SerializeField] private RectTransform _handleAddictNum;
        [SerializeField] private CurrencyFilter _currencyFilter;

        [SerializeField] private GameObject _softNumSliderGO;
        [SerializeField] private Slider _softNumSlider;
        [SerializeField] private Text _softNum;
        [SerializeField] private RectTransform _handleAddictSoft;

        [SerializeField] private GameObject _hardNumSliderGO;
        [SerializeField] private Slider _hardNumSlider;
        [SerializeField] private Text _hardNum;
        [SerializeField] private RectTransform _handleAddictHard;
        
        [SerializeField] private GameObject _createGO;
        [SerializeField] private GameObject _existGO;
        [SerializeField] private Text _code;
        [SerializeField] private Text _persons;
        [SerializeField] private Text _entryFee;
        [SerializeField] private CurrencyBadge _entryBadge;
        [SerializeField] private Button _copyCode;
        [SerializeField] private Button _share;
        [SerializeField] private Text _endTime;


        private float _handleAddictNumMax;
        private float _handleAddictSoftMax;
        private float _handleAddictHardMax;
        
        public void Start()
        {
            _handleAddictNumMax = _handleAddictNum.sizeDelta.x;
            _handleAddictSoftMax = _handleAddictSoft.sizeDelta.x;
            _handleAddictHardMax = _handleAddictHard.sizeDelta.x;
            
            float dif1 = (_playerNumSlider.value - _playerNumSlider.minValue)  / (_playerNumSlider.maxValue - _playerNumSlider.minValue );
            _handleAddictNum.sizeDelta = new Vector2(dif1 * _handleAddictNumMax, _handleAddictNum.sizeDelta.y);
            
            float dif2 = (_softNumSlider.value - _softNumSlider.minValue)  / (_softNumSlider.maxValue - _softNumSlider.minValue );
            _handleAddictSoft.sizeDelta = new Vector2(dif2 * _handleAddictSoftMax, _handleAddictSoft.sizeDelta.y);
            
            float dif3 = (_hardNumSlider.value - _hardNumSlider.minValue)  / (_hardNumSlider.maxValue - _hardNumSlider.minValue );
            _handleAddictHard.sizeDelta = new Vector2(dif3 * _handleAddictHardMax, _handleAddictHard.sizeDelta.y);
            
            _createTournamentBtn.onClick.RemoveAllListeners();
            _createTournamentBtn.onClick.AddListener(() =>
            {
                if (_nameInput.Validate())
                {
                    _createTournamentBtn.gameObject.SetActive(false);
                    onCreateTournament?.Invoke(TakeServerCreateTournamentModel(), result =>
                    {
                        _createTournamentBtn.gameObject.SetActive(true);
                        UpdateCreateView(result.tournament);
                    });
                }
            });
            
            _playerNumSlider.onValueChanged.AddListener((num) =>
            {
                float dif = (num - _playerNumSlider.minValue)  / (_playerNumSlider.maxValue - _playerNumSlider.minValue );
                _handleAddictNum.sizeDelta = new Vector2(dif * _handleAddictNumMax, _handleAddictNum.sizeDelta.y);
            });
            
            _softNumSlider.onValueChanged.AddListener((num) =>
            {
                if (num > _softNumSlider.minValue && num < _softNumSlider.maxValue)
                {
                    _softNum.gameObject.SetActive(true);
                    _softNum.text = num.ToString();
                }
                else
                {
                    _softNum.gameObject.SetActive(false);
                }
                
                float dif = (num - _softNumSlider.minValue)  / (_softNumSlider.maxValue - _softNumSlider.minValue );
                _handleAddictSoft.sizeDelta = new Vector2(dif * _handleAddictSoftMax, _handleAddictSoft.sizeDelta.y);
            });
            
            _hardNumSlider.onValueChanged.AddListener((num) =>
            {
                if (num > _hardNumSlider.minValue && num < _hardNumSlider.maxValue)
                {
                    _hardNum.gameObject.SetActive(true);
                    _hardNum.text = num.ToString();
                }
                else
                {
                    _hardNum.gameObject.SetActive(false);
                }
                
                float dif = (num - _hardNumSlider.minValue)  / (_hardNumSlider.maxValue - _hardNumSlider.minValue );
                _handleAddictHard.sizeDelta = new Vector2(dif * _handleAddictHardMax, _handleAddictHard.sizeDelta.y);
            });

            _currencyFilter.onCurrencyClick += (type) =>
            {
                _softNumSliderGO.SetActive(type == CurrencyType.Z);
                _hardNumSliderGO.SetActive(type == CurrencyType.USD);
            };
            
            _copyCode.onClick.RemoveAllListeners();
            _copyCode.onClick.AddListener(() =>
            {
                GUIUtility.systemCopyBuffer = _code.text;
                UIMapController.OpenNotification(new ServerNotification()
                {
                    id = -1,
                    notification_id = -1,
                    short_text = LocalizationManager.Text("gamla.codecopied.notification")
                });
            });
            
            _share.onClick.RemoveAllListeners();
            _share.onClick.AddListener(() =>
            {
                GUIUtility.systemCopyBuffer = _code.text;
                Close();
            });
            
            var myTournament = LocalState.tournaments.Find(t => t.isMy && t.game_id == ClientManager.gameId && t.status != "finished" && t.status != "cancelled");
            UpdateCreateView(myTournament);
        }

        private void UpdateCreateView(ServerTournamentModel myTournament)
        {
            if (myTournament != null)
            {
                _code.text = myTournament.id + "p" + myTournament.private_code;
                _persons.text = myTournament.players_count + "";
                _entryFee.text = myTournament.entry_cost + "";
                _entryBadge.Init(myTournament.currency == "Z" ? CurrencyType.Z : CurrencyType.USD);
                
                var endAt = DateTime.Parse(myTournament.end_at);
                _endTime.text = endAt.ToShortDateString() + " " + endAt.ToShortTimeString();
                
                _existGO.SetActive(true);
                _createGO.SetActive(false);
            }
            else
            {
                _existGO.SetActive(false);
                _createGO.SetActive(true);
            }
        }

        ServerCreateTournamentModel TakeServerCreateTournamentModel()
        {
            var num = 0;
            switch (_playerNumSlider.value)
            {
                case 1:
                    num = 8;
                    break;
                case 2:
                    num = 16;
                    break;
                case 3:
                    num = 32;
                    break;
            }
            var model = new ServerCreateTournamentModel
            {
                name = _nameInput.text,
                players_count = num,
                currency = _currencyFilter.currentCurrency.ToString(),
                entry_cost = (int)(_currencyFilter.currentCurrency == CurrencyType.Z ? _softNumSlider.value : _hardNumSlider.value)
            };
            return model;
        }
    }
}