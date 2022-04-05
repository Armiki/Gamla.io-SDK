using System.Collections.Generic;
using System.Linq;
using Gamla.Scripts.Logic;
using Gamla.Scripts.UI.Tutorial;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    private readonly List<string> _steps = new List<string>
    {
        "tutorial-1-1",
        "tutorial-1-2",
        "tutorial-1-3",
        "tutorial-2-1",
        "tutorial-2-2",
        "tutorial-2-3"
    };
    
    public static TutorialManager Instance { get; private set; }

    public bool IsCompleted => PlayerPrefs.GetInt("TutorialIsCompleted", 0) > 0;
    
    private TutorialBlocker _tutorialBlocker;
    
    private string _currentStepName;
    public string CurrentStepName
    {
        get
        {
            if (string.IsNullOrEmpty(_currentStepName))
            {
                _currentStepName = PlayerPrefs.GetString("TutorialStepName", string.Empty);
            }

            return _currentStepName;
        }

        private set
        {
            if (_currentStepName == value) return;
            if (string.IsNullOrEmpty(value))
            {
                PlayerPrefs.DeleteKey("TutorialStepName");
                return;
            }
            _currentStepName = value;
            PlayerPrefs.SetString("TutorialStepName", _currentStepName);
        }
    }

    private void Awake()
    {
        // uncomment for tutorial debug
        //ResetTutorial();
        
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);

        if (string.IsNullOrEmpty(CurrentStepName))
        {
            CurrentStepName = _steps[0];
        }
    }

    public void InitTutorialBlocker()
    {
        _tutorialBlocker = 
            Instantiate(GamlaResourceManager.GamlaResources.GetResource("Tutorial/TutorialBlocker"),
                GamlaResourceManager.tutorialContainer).GetComponent<TutorialBlocker>();
        
        _tutorialBlocker.gameObject.SetActive(false);
    }
    
    public void TryActivateStepWithElements(List<TutorialElement> elements)
    {
        foreach (var element in elements)
        {
            TryActivateElementByStep(element);
        }
    }
    
    private void TryActivateElementByStep(TutorialElement element)
    {
        if (!IsCompleted && element.TutorialStepName == CurrentStepName)
        {
            _tutorialBlocker.gameObject.SetActive(true);
            var refPoint = _tutorialBlocker.TutorialRefPoints
                .FirstOrDefault(x => x.TutorialStepName == CurrentStepName);

            if (refPoint == null)
            {
                Debug.LogError($"There isn't TutorialRefPoint for tutorial step {CurrentStepName}");
                return;
            }
            
            _tutorialBlocker.gameObject.SetActive(true);
            
            var copy = Instantiate(element.gameObject, refPoint.RefPoint);
            
            var newButton = copy.GetComponent<Button>();
            if (newButton == null)
            {
                newButton = copy.GetComponentInChildren<Button>();
            }
            
            newButton.onClick.AddListener(() =>
            {
                element.TutorialButton.onClick?.Invoke();
                FinishCurrentStep(element, refPoint.RefPoint);
            });
        }
    }

    private void FinishCurrentStep(TutorialElement element, Transform refPoint)
    {
        var index = _steps.IndexOf(CurrentStepName);
        _tutorialBlocker.gameObject.SetActive(false);
        
        foreach(Transform child in refPoint)
        {
            Destroy(child.gameObject);
        }
        
        if (index > _steps.Count)
        {
            CurrentStepName = string.Empty;
            PlayerPrefs.SetInt("TutorialIsCompleted", 1);
            
            return;
        }

        CurrentStepName = _steps[index + 1];
    }

    public void ResetTutorial()
    {
        PlayerPrefs.DeleteKey("TutorialIsCompleted");
        PlayerPrefs.DeleteKey("TutorialStepName");
        InitTutorialBlocker();
    }
}
