using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Gamla.Data;
using Gamla.Logic;

namespace Gamla.UI
{
    public class NotificationWidget : MonoBehaviour
    {
        [SerializeField] private RectTransform _anim;
        [SerializeField] private Text _text;
        [SerializeField] private Button _close;

        private ServerNotification _notification;
        private Sequence _closeSequence;

        public void Start()
        {
            _close.onClick.RemoveAllListeners();
            _close.onClick.AddListener(() =>
            {
                ImmidiatelyClose();
                OpenUIAfterClick();
            });
        }
        public void Init(ServerNotification notification)
        {
            _notification = notification;
            _text.text = notification.short_text;
            _anim.DOLocalMoveY(100, 1f).OnComplete(Close);
        }

        void Close()
        {
            _closeSequence = DOTween.Sequence();
            _closeSequence.AppendInterval(3).OnComplete(() => _anim.DOLocalMoveY(500, 1f).OnComplete(ImmidiatelyClose));
            _closeSequence.Play();
        }

        void ImmidiatelyClose()
        {
            if(_notification.id > 0)
                ServerCommand.ReadNotification(_notification.id);
            Destroy(gameObject);
        }

        void OpenUIAfterClick()
        {
            if (_notification != null)
            {
                switch (_notification.notification_id)
                {
                    case 1: GamlaResourceManager.tabBar.SelectPlay(); break;// UIMapController.Clear(); 
                    case 2: GamlaResourceManager.tabBar.SelectPlay(); break;//  UIMapController.Clear(); break;
                    case 3: GamlaResourceManager.tabBar.SelectPlay(); break;//  UIMapController.Clear(); break;
                    case 4: GamlaResourceManager.tabBar.SelectTop(); break;//  UIMapController.Clear(); break;
                    case 5: GamlaResourceManager.tabBar.SelectPlay(); break;//  UIMapController.Clear(); break;
                    case 6: GamlaResourceManager.tabBar.SelectPlay(); break;//  UIMapController.Clear(); break;
                    case 7: GamlaResourceManager.tabBar.SelectPlay(); break;//  UIMapController.Clear(); break;
                    case 8: GamlaResourceManager.tabBar.SelectPlay(); break;//  UIMapController.Clear(); break;
                    case 9: GamlaResourceManager.tabBar.SelectPlay(); break;//  UIMapController.Clear(); break;
                    case 10: GamlaResourceManager.tabBar.SelectPlay(); break;//  UIMapController.Clear(); break;
                    case 11: GamlaResourceManager.tabBar.SelectProfile(); break;//  UIMapController.Clear(); break;
                    case 12: GamlaResourceManager.tabBar.SelectStore(); break;//  UIMapController.Clear(); break;
                    case 13: GamlaResourceManager.tabBar.SelectStore(); break;//  UIMapController.Clear(); break;
                    case 14: GamlaResourceManager.tabBar.SelectStore(); break;//  UIMapController.Clear(); break;
                    case 15: GamlaResourceManager.tabBar.SelectStore(); break;//  UIMapController.Clear(); break;
                    
                    case 18: GamlaResourceManager.tabBar.SelectPlay(); break;//  UIMapController.Clear(); break;
                    case 19: GamlaResourceManager.tabBar.SelectPlay(); break;//  UIMapController.Clear(); break;
                }
            }
        }

        private void OnDestroy()
        {
            _closeSequence?.Kill();
        }
    }
}