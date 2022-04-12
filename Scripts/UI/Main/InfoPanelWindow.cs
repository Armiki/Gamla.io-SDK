using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class InfoPanelWindow : GUIView
    {
        public event Action onResetTutorialClick;
        public event Action onContactUsClick;
        public event Action onFAQClick;
        public event Action onLegalNoteClick;
        
        [SerializeField] private Button _resetTutor;
        [SerializeField] private Button _contactUs;
        [SerializeField] private Button _faq;
        [SerializeField] private Button _legalNote;
        
        public void Start()
        {
            _resetTutor.onClick.RemoveAllListeners();
            _resetTutor.onClick.AddListener(() => onResetTutorialClick?.Invoke());
            
            _contactUs.onClick.RemoveAllListeners();
            _contactUs.onClick.AddListener(() => onContactUsClick?.Invoke());
            
            _faq.onClick.RemoveAllListeners();
            _faq.onClick.AddListener(() => onFAQClick?.Invoke());
            
            _legalNote.onClick.RemoveAllListeners();
            _legalNote.onClick.AddListener(() => onLegalNoteClick?.Invoke());
        }
    }
}
