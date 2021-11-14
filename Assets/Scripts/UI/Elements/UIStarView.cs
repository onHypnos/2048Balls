using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Animator))]
public class UIStarView : BaseUIElementView
{
    [SerializeField] private Image _backGround;
    [SerializeField] private Image _front;

    private Animator _animator;
    private bool _isActive = false;


    private void Awake()
    {
        _animator = GetComponent<Animator>();

        if (_backGround == null || _front == null)
        {
            Debug.LogWarning($"Star {gameObject.name} is not configured! Huita!");
        }
    }


    public void Deactivate()
    {
        if (!_isActive)
        {
            return;
        }

        _front.enabled = false;
        _backGround.enabled = true;
        _isActive = false;
    }
    public void Activate()
    {
        if (_isActive)
        {
            _animator.SetBool("Activate", false);
            return;
        }

        _animator.SetBool("Activate", true);

        _backGround.enabled = false;
        _front.enabled = true;
        _isActive = true;
    }
}