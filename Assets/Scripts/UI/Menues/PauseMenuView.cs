using UnityEngine;
using UnityEngine.UI;

public class PauseMenuView : BaseMenuView
{
    [Header("Panel")]
    [SerializeField] private GameObject _panel;

    [Header("Elements")]
    [SerializeField] private Button _buttonResume;
    [SerializeField] private Button _buttonRestart;

    private UIController _uiController;


    private void Awake()
    {
        _buttonResume.onClick.AddListener(UIEvents.Current.ButtonResumeGame);
        _buttonRestart.onClick.AddListener(UIEvents.Current.ButtonRestartGame);
        FindMyController();
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

    private void OnDestroy()
    {
        _buttonResume.onClick.RemoveAllListeners();
    }
}