using System;
using System.Collections;
using DG.Tweening;
using Gamla.Scripts.Common.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Main
{

    [Serializable]
    public class SugnUpPages
    {
        public int order;
        public Image progress;
        public GameObject page;
        public Animation anim;
        public AnimationClip clip;
    }

    public class SignUpWindow : GUIView, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public event Action onSignInClick;
        public event Action onSignUpClick;
        public event Action onGuestClick;

        [SerializeField] private SugnUpPages[] _pages;
        [SerializeField] private Button _signUp;
        [SerializeField] private Button _signUpGuest;
        [SerializeField] private Button _signIn;
        
        private int currentOrder = 0;
        private float currentProgress = 0;
        private Coroutine _coroutine;
        private Vector2 _lastMousePosition;


        public void Start()
        {
            currentOrder = 0;
            _pages[0].clip.legacy = true;
            _pages[0].anim.clip = _pages[0].clip;
            _pages[0].anim.Play();
            _coroutine = StartCoroutine(NextPage());
            _signUp.onClick.RemoveAllListeners();
            _signUp.onClick.AddListener(() => onSignUpClick?.Invoke());
            _signUpGuest.onClick.RemoveAllListeners();
            _signUpGuest.onClick.AddListener(() => onGuestClick?.Invoke());
            _signIn.onClick.RemoveAllListeners();
            _signIn.onClick.AddListener(() => onSignInClick?.Invoke());
        }

        public void OnDisable()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
        }

        IEnumerator NextPage()
        {
            while (currentProgress <= 1)
            {
                yield return null;
                currentProgress += 0.005f;
                _pages[currentOrder].progress.fillAmount = currentProgress;
                if (currentProgress > 1)
                {
                    if(currentOrder < _pages.Length - 1)
                    {
                        MoveLeft();
                        currentOrder++;
                        MoveCenter();
                        currentProgress = 0;

                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        void MoveLeft()
        {
            _pages[currentOrder].page.transform.DOLocalMoveX(-1084, 0.5f);
        }

        void MoveCenter()
        {
            _pages[currentOrder].page.transform.DOLocalMoveX(0, 0.5f);
            if (_pages[currentOrder].anim != null)
            {
                _pages[currentOrder].clip.legacy = true;
                _pages[currentOrder].anim.clip = _pages[currentOrder].clip;
                _pages[currentOrder].anim.Play();
            }
        }

        void MoveRight()
        {
            _pages[currentOrder].page.transform.DOLocalMoveX(1084, 0.5f);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _lastMousePosition = eventData.position;
            StopCoroutine(_coroutine);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var delta = eventData.position.x - _lastMousePosition.x;
            //Debug.LogError(delta);
            if ( delta > 100)
            {
                HardMove(-1);
            }
            if ( delta < -100)
            {
                HardMove(1);
            }
            _coroutine = StartCoroutine(NextPage());
        }

        void HardMove(int direction)
        {
            if ((currentOrder + direction) >= 0 && (currentOrder + direction) <= (_pages.Length - 1))
            {
                if (direction > 0)
                {
                    MoveLeft();
                }

                if (direction < 0)
                {
                    MoveRight();
                }
                _pages[currentOrder].progress.fillAmount = 1;
                currentOrder = currentOrder + direction;
                MoveCenter();
                currentProgress = 0;

            }
        }

        public void OnDrag(PointerEventData eventData)
        {
          
        }
    }
}
