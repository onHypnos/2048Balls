using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : BaseMenuView
{
    [Header("Panel")]
    [SerializeField] private GameObject _panel;

    [Header("Elements")]
    [SerializeField] private Button _buttonStart;

    private UIController _uiController;


    private void Awake()
    {
        _buttonStart.onClick.AddListener(UIEvents.Current.ButtonStartGame);
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
        _buttonStart.onClick.RemoveAllListeners();
    }
}