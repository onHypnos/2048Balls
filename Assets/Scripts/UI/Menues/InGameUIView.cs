using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUIView : BaseMenuView
{
    [Header("Panel")]
    [SerializeField] private GameObject _panel;

    [Header("Elements")]
    [SerializeField] private Button _buttonPause;
    [SerializeField] private Slider _slider;
    [SerializeField] private UIStarView[] _stars;
    [SerializeField] private TextMeshProUGUI _textScore;

    [Header("Settings")]
    [SerializeField] [Range(0.0f, 1.0f)] private float _star1Value;
    [SerializeField] [Range(0.0f, 1.0f)] private float _star2Value;
    [SerializeField] [Range(0.0f, 1.0f)] private float _star3Value;

    private UIController _uiController;


    private void Awake()
    {
        _buttonPause.onClick.AddListener(UIEvents.Current.ButtonPauseGame);
        FindMyController();

        if (_stars.Length < 3)
        {
            Debug.LogWarning("UI stars array is not full. They'll be not working! Huita!");
        }
        else
        {
            DeactivateStars();
        }

        _slider.value = 0.0f;
        _textScore.text = "0";
    }


    private void FindMyController()
    {
        _uiController = transform.parent.GetComponent<UIController>();
        _uiController.AddView(this);
    }

    public override void Hide()
    {
        if (!IsShow) return;
        _panel.gameObject.SetActive(false);
        IsShow = false;
    }

    public override void Show()
    {
        if (IsShow) return;
        _panel.gameObject.SetActive(true);
        IsShow = true;
    }

    public void SetSlider(int currentPoints, int maxPoints)
    {
        float value = (float)currentPoints / maxPoints;

        if (value > 1.0f)
        {
            value = 1.0f;
        }

        _slider.value = value;

        if (value >= _star1Value)
        {
            ActivateStar(1);
        }
        if (value >= _star2Value)
        {
            ActivateStar(2);
        }
        if (value >= _star3Value)
        {
            ActivateStar(3);
        }

        _textScore.text = $"{currentPoints}";
    }

    public void ActivateStar(int starNumber)
    {
        _stars[starNumber - 1].Activate();
    }
    public void DeactivateStars()
    {
        for (int i = 0; i < _stars.Length; i++)
        {
            _stars[i].Deactivate();
        }
    }

    private void OnDestroy()
    {
        _buttonPause.onClick.RemoveAllListeners();
    }
}