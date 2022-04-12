using System;
using UnityEngine;

namespace Gamla.UI
{
    public class TabBar : MonoBehaviour
    {
        public event Action onGamesClick;
        public event Action onPlayClick;
        public event Action onStoreClick;
        public event Action onTopClick;
        public event Action onProfileClick;

        [SerializeField] private TabBarItem _games;
        [SerializeField] private TabBarItem _play;
        [SerializeField] private TabBarItem _store;
        [SerializeField] private TabBarItem _top;
        [SerializeField] private TabBarItem _profile;

        public void Start()
        {
            _games.onSelect += SelectGames;
            _play.onSelect += SelectPlay;
            _store.onSelect += SelectStore;
            _top.onSelect += SelectTop;
            _profile.onSelect += SelectProfile;
        }

        public void SelectGames()
        {
            _games.pressed = true;
            _play.pressed = false;
            _store.pressed = false;
            _top.pressed = false;
            _profile.pressed = false;
            onGamesClick?.Invoke();
        }
        
        public void SelectPlay()
        {
            _games.pressed = false;
            _play.pressed = true;
            _store.pressed = false;
            _top.pressed = false;
            _profile.pressed = false;
            onPlayClick?.Invoke();
        }
        
        public void SelectStore()
        {
            _games.pressed = false;
            _play.pressed = false;
            _store.pressed = true;
            _top.pressed = false;
            _profile.pressed = false;
            onStoreClick?.Invoke();
        }
        
        public void SelectTop()
        {
            _games.pressed = false;
            _play.pressed = false;
            _store.pressed = false;
            _top.pressed = true;
            _profile.pressed = false;
            onTopClick?.Invoke();
        }
        
        public void SelectProfile()
        {
            _games.pressed = false;
            _play.pressed = false;
            _store.pressed = false;
            _top.pressed = false;
            _profile.pressed = true;
            onProfileClick?.Invoke();
        }

        private bool isChange = false;
        
        public void RefreshSafeZone(int indentSize)
        {
            var root = GetComponent<RectTransform>();
            //root.localPosition = new Vector2(0, indentSize/2.0f);
            if (!isChange)
            {
                root.pivot = new Vector2(0.5f,0);
                root.sizeDelta = new Vector2(root.sizeDelta.x, (root.sizeDelta.y - indentSize / 2));
                root.position = new Vector3(root.position.x, 0);
                isChange = true;
            }

            //root.sizeDelta = new Vector2(0, indentSize);
        }
    }
}
