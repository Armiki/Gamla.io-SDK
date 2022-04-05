using System;
using UnityEngine;
using UnityEngine.UI;
using Gamla.Scripts.Data;
using Gamla.Scripts.UI.Main;
using JetBrains.Annotations;
using UnityEngine.UI.Extensions;

namespace GamlaSDK.Scripts.UI.Game
{
    public class TournamentUserPair : MonoBehaviour
    {
        [SerializeField] private UserProfileWidget _user1;
        [SerializeField] private Text _user1Points;
        [SerializeField] private RecolorItem _recolorUser1Name;
        [SerializeField] private GameObject _user1PrizeLbl;
        [SerializeField] private UserProfileWidget _user2;
        [SerializeField] private Text _user2Points;
        [SerializeField] private RecolorItem _recolorUser2Name;
        [SerializeField] private GameObject _user2PrizeLbl;
        [SerializeField] private GameObject _leftLine;
        [SerializeField] private GameObject _rightLine;
        [SerializeField] private UILineRenderer _lineRenderer;

        public Transform getLeftLine => _leftLine.transform;

        private Transform _to;
        private RectTransform _rootCanvas;

        public void Init(ServerPlayerMatch user1, long user1Points, ServerPlayerMatch user2, long user2Points, long winId)
        {
            _user1.Init(user1);
            _user2.Init(user2);
            _user1Points.text = user1Points.ToString();
            _user2Points.text = user2Points.ToString();
            long currentUserId = LocalState.currentUser.uid;
            _recolorUser1Name.recolorFilter = user1.id == currentUserId ? "textColorPrize" : "textColorPrimary";
            _recolorUser2Name.recolorFilter = user2.id == currentUserId ? "textColorPrize" : "textColorPrimary";
            _user1PrizeLbl.SetActive(user1.id == winId);
            _user2PrizeLbl.SetActive(user2.id == winId);
            
//            _leftLine.SetActive(false);
//            _rightLine.SetActive(false);
        }

        public void DrawLines([CanBeNull] Transform from, [CanBeNull] Transform to, RectTransform rootCanvas)
        {
            _to = to;
            _rootCanvas = rootCanvas;
        }

        private void LateUpdate()
        {
            _lineRenderer.Points[0] = Vector2.zero;
            if (_to != null)
            {
                float delta = MainCanvas.Canvas.transform.localScale.x;
                float scale = _rootCanvas.localScale.x;
                var targetPos = (_to.position - _rightLine.transform.position) / delta / scale;
                _lineRenderer.Points[1] =  Vector3.right * 25;
                _lineRenderer.Points[2] =  targetPos - Vector3.right * 25;
                _lineRenderer.Points[3] =  targetPos;
                _lineRenderer.gameObject.SetActive(true);
                _lineRenderer.SetAllDirty();
            }
            else 
                _lineRenderer.gameObject.SetActive(false);
        }

        public void Clear()
        {
            _user1.Clear();
            _user2.Clear();
            _user1Points.text = "";
            _user2Points.text = "";
            _user1PrizeLbl.SetActive(false);
            _user2PrizeLbl.SetActive(false);
            
//            _leftLine.SetActive(false);
//            _rightLine.SetActive(false);
            _lineRenderer.gameObject.SetActive(false);
        }
    }
}