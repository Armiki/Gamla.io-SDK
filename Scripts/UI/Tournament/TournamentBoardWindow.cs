using System;
using System.Collections.Generic;
using System.Linq;
using Gamla.Data;
using Gamla.GUI;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Gamla.UI
{
    public class TournamentBoardWindow : GUIView//, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _headRect;
        [SerializeField] private RectTransform _contentRect;
        [SerializeField] private RectTransform _contentForZoom;
        
        [SerializeField] private Button _play;
        [SerializeField] private Button _playAgain;

        [SerializeField] private Text _title;
        [SerializeField] private HeadTournamentRound _headPref;
        [SerializeField] private RectTransform _roundColumnPref;
        [SerializeField] private TournamentUserPair _userPairPref;
        [SerializeField] private TournamentWinner _winner;
        [SerializeField] private PanAndZoom _panAndZoom;
        [SerializeField] private BoundsContent _boundsContent;
        
        private List<RectTransform> _roundColumns = new List<RectTransform>();

        public float _zoomFrom = 2;
        public float _zoomTo = 0.5f;

        private ServerTournamentModel _tournament;
        private Vector2 _lastMousePosition;
        [SerializeField] private Vector2 _boundX;
        [SerializeField] private Vector2 _contentBoundY;
        
        bool _isEditorMultiTouch;
        bool _isMultiTouch;
        
        public float zoomDelta = 0.1f;
        public float zoomDeltaTouch = 0.08f;
        
        float _distance;
        PointerEventData _eventA;
        PointerEventData _eventB;
        int _currentMainFinger = -1;
        int _currentSecondFinger = -1;
        bool _isGesture;
        bool isMultiClick => (_currentMainFinger != -1 && _currentSecondFinger != -1) ||
                             Input.GetKey(KeyCode.LeftControl);

        private float _headUpPos;

        public void Start()
        {
            _boundX = new Vector2(-2100, 0);
            _contentBoundY = new Vector2(-500, -200);
            _headUpPos = _headRect.anchoredPosition.y;
            
            _panAndZoom.onSwipe += v =>
            {
                v = v * 1 / _contentForZoom.localScale.x;
                _contentRect.anchoredPosition += v;
                
                var anchoredPosition = _headRect.anchoredPosition;
                var headPos = new Vector2(anchoredPosition.x + v.x, anchoredPosition.y);
                anchoredPosition = headPos;
                _headRect.anchoredPosition = anchoredPosition;
            };
            
            _panAndZoom.onPinch += (f1, f2) =>
            {
                var delta = (f2 - f1) / 800;
                var scale = _contentForZoom.localScale;
                _contentForZoom.localScale = new Vector3(Math.Max(Mathf.Min(1f, scale.x + delta), 0.3f), Math.Max(Mathf.Min(1f, scale.y + delta), 0.3f), 1);
            };
        }

        public void SetData(ServerTournamentModel tournament)
        {
            _headRect.ClearChilds();
            _contentRect.ClearChilds();
            _roundColumns.Clear();

            _tournament = tournament;
            _title.text = tournament.name;

            tournament.awards.Sort(((a1, a2) => a2.place.CompareTo(a1.place)));
            var currentAwards = new List<ServerTournamentAward>();
            for (int i = tournament.players_count / 2; i >= 1; i /= 2)
            {
                var award = tournament.awards.Find(a => a.place == i);
                if (award == null) {
                    Debug.LogError($"Can't find award place {i} in tournament");
                    continue;
                }
                
                var head = Instantiate(_headPref, _headRect);
                head.SetData(award);
                var column = Instantiate(_roundColumnPref, _contentRect);
                column.sizeDelta = new Vector2(994, tournament.players_count * 240);
                _roundColumns.Add(column);
                currentAwards.Add(award);
            }
            
            var headWinner = Instantiate(_headPref, _headRect);
            headWinner.SetDataWinner(tournament.awards[tournament.awards.Count - 1]);
            var columnWinner = Instantiate(_roundColumnPref, _contentRect);
            columnWinner.sizeDelta = new Vector2(994, tournament.players_count * 240);
            var winnerView = Instantiate(_winner, columnWinner);
            winnerView.SetClear();

            _boundX = new Vector2(-994 * currentAwards.Count, 0);
            _contentBoundY = new Vector2(-200 * tournament.players_count + 1, -200);

            foreach (var match in tournament.matches)
            {
                if(match.stage == 0)
                    return;

                var lastStage = match.stage - 1;
                if (_roundColumns.Count <= lastStage) {
                    Debug.LogError($"Cant get stage {lastStage} in match {match.game_id}");
                    continue;
                }
                
                var column = _roundColumns[lastStage];
                var pair = Instantiate(_userPairPref, column);
                int i = match.stage - 1;
                pair.name = "pair_" + i + "_" + _roundColumns[i].childCount;
                if (match.players != null)
                {
                    var player0 = match.players.Count > 0 ? match.players[0] : new ServerPlayerMatch();
                    var player1 = match.players.Count > 1 ? match.players[1] : new ServerPlayerMatch();
                    pair.Init(player0, player0.score, player1, player1.score, match.winner_id);
                }
                else
                    pair.Clear();
            }

            int playersInRound = tournament.players_count / 2;
            for (int i = 0; i < _roundColumns.Count; i++)
            {
                for(int j = 0; j < playersInRound; j++)
                {
                    if (_roundColumns[i].childCount < playersInRound)
                    {
                        var pair = Instantiate(_userPairPref, _roundColumns[i]);
                        pair.name = "pair_" + i + "_" + _roundColumns[i].childCount;
                        pair.Clear();
                    }
                }
                playersInRound /= 2;
            }

            for (int i = 0; i < _roundColumns.Count - 1; i++)
            {
                _roundColumns[i].GetComponent<VerticalLayoutGroup>().spacing = 70 + i * 450;
                int k = 0;
                bool b = false;
                for (int j = 0; j < _roundColumns[i].childCount; j++)
                {
                    try
                    {
                        Transform pairLeft = _roundColumns[i].GetChild(j);
                        
                        Transform pairRight = _roundColumns[Math.Min(i + 1, _roundColumns.Count - 1)].GetChild(Math.Min(k, _roundColumns[i+1].childCount - 1));

                        pairLeft.GetComponent<TournamentUserPair>().DrawLines(null,
                            pairRight.GetComponent<TournamentUserPair>().getLeftLine, _contentForZoom);
                        if (b) k++;
                        b = !b;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
            
            _contentRect.sizeDelta = new Vector2(1044 * (currentAwards.Count + 1), (tournament.players_count+2) * 240);

            UpdatePlayButton();
            EventManager.OnTournamentsUpdated.Del(OnUpdateTournaments);
            EventManager.OnTournamentsUpdated.Subscribe(OnUpdateTournaments);
        }

        private void OnUpdateTournaments()
        {
            UpdatePlayButton();
        }

        void UpdatePlayButton()
        {
            if (_tournament == null) {
                _play.gameObject.SetActive(false);
                return;
            }
            var updatedTournament = LocalState.tournaments.Find(t => t.id == _tournament.id);
            if (updatedTournament == null) {
                _play.gameObject.SetActive(false);
                return;
            }

            bool isCanPlay = _tournament.status != "cancelled" &&
                             _tournament.status != "finished" &&
                             updatedTournament.matches.Any(m => m.players.Any(p =>
                                 p.user_id == LocalState.currentUser.uid && string.IsNullOrEmpty(p.score)));
            _play.gameObject.SetActive(isCanPlay);
        }

        public void PlayMatch()
        {
            if (_tournament != null)
            {
                var match = _tournament.matches.Find(m =>
                    //m.status == "waiting" && 
                    m.players.Any(p => p.user_id == LocalState.currentUser.uid && string.IsNullOrEmpty(p.score))
                );
                if (match != null)
                {
                    LocalState.currentTournament = _tournament;
                    LocalState.currentMatch = new ServerMatchStart {match = match};
                    ServerCommand.TryPlayTournament(match.id, _tournament);
                    _play.gameObject.SetActive(false);
                }
            }
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _lastMousePosition = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ChangePosition(eventData);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (!isMultiClick && !_isGesture)
            {
                ChangePosition(eventData);
            }
            else
            {
                MultiDragImpl(_eventA, _eventB);
            }
        }

        public void ChangePosition(PointerEventData eventData)
        {
            var deltaX = eventData.position.x - _lastMousePosition.x;
            var deltaY = eventData.position.y - _lastMousePosition.y;

            var x = Math.Max(_headRect.anchoredPosition.x + deltaX, _boundX.x);
            x = Math.Min(x, _boundX.y);
            
            
            var y = Math.Max(_contentRect.anchoredPosition.y + deltaY, _contentBoundY.x);
            y = Math.Min(y, _contentBoundY.y);
            
            
            _headRect.anchoredPosition = new Vector2(x, _headRect.anchoredPosition.y);
            _contentRect.anchoredPosition = new Vector2(x, y);
            _lastMousePosition = eventData.position;
        }
        
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_currentMainFinger == -1) {
                _currentMainFinger = eventData.pointerId;
                _eventA = eventData;
                return;
            }
            if (_currentSecondFinger == -1) {
                _currentSecondFinger = eventData.pointerId;
                _eventB = eventData;
            }

            if (isMultiClick) {
                _isGesture = true;
                MultiDragImpl(_eventA, _eventB);
            }
        }
        void MultiDragImpl(PointerEventData eventA, PointerEventData eventB)
        {

            float angle = 0;
            angle = Angle(eventA.position, eventB.position);
            float prevTurn = Angle(eventA.position - eventA.delta,
                eventB.position - eventB.delta);
            float turnAngleDelta = Mathf.DeltaAngle(prevTurn, angle);
            var touchDelta0 = eventA.position - eventA.delta;
                var touchDelta1 = eventB.position - eventB.delta;
                float prevTouchDeltaMag = (touchDelta1 - touchDelta0).magnitude;
                float touchDeltaMag = (eventB.position - eventA.position).magnitude;

                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
                _distance -= zoomDeltaTouch * deltaMagnitudeDiff;
                ClampZoom();
        }
        
        float Angle(Vector2 pos1, Vector2 pos2)
        {
            Vector2 from = pos2 - pos1;
            Vector2 to = new Vector2(1, 0);

            float result = Vector2.Angle(from, to);
            Vector3 cross = Vector3.Cross(from, to);

            if (cross.z > 0) {
                result = 360f - result;
            }

            return result;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if ( _currentMainFinger == eventData.pointerId ) {
                _currentMainFinger = -1;
                if (_currentSecondFinger != -1) {
                    return;
                } 
                _isGesture = false;
            }
            if ( _currentSecondFinger == eventData.pointerId ) {
                _currentSecondFinger = -1;
                if (_currentMainFinger != -1) {
                    return;
                }
                _isGesture = false;
            }
        }
        
        void ClampZoom()
        {
            if (_distance < _zoomTo) _distance = _zoomTo;
            if (_distance > _zoomFrom) _distance = _zoomFrom;
            _contentForZoom.localScale = _distance * Vector3.one;
        }
        
        
        private readonly Vector3[] m_Corners = new Vector3[4];
        private Bounds GetBounds()
        {
            if (_contentRect == null)
                return new Bounds();

            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            var toLocal = transform.GetRect().worldToLocalMatrix;
            _contentRect.GetWorldCorners(m_Corners);
            for (int j = 0; j < 4; j++)
            {
                Vector3 v = toLocal.MultiplyPoint3x4(m_Corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            var bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
            return bounds;
        }

        
        public void LateUpdate()
        {
            _headRect.anchoredPosition = new Vector2(_contentRect.anchoredPosition.x, _headUpPos);
        }

        public override void OnDestroy()
        { 
            EventManager.OnTournamentsUpdated.Del(OnUpdateTournaments);
            base.OnDestroy();
        }
    }
}