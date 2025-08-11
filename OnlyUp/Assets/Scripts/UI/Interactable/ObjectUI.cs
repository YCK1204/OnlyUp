using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectUI : MonoBehaviour
{
    Animator _animator;
    RectTransform _rectTransform;
    TextMeshProUGUI _text;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rectTransform = GetComponent<RectTransform>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _text.enableWordWrapping = false;
    }
    public void Show(InteractableObjectData data)
    {
        var usable = data.IsUsable;
        var usableText = usable ? "<color=#FF0000> [E키를 눌러 사용]</color>" : "";
        var description = $"{data.Description}{usableText}";
        var name = data.Name;

        _text.text = $"{name}: {description}";
        _rectTransform.sizeDelta = new Vector2(_text.preferredWidth + 20, _text.preferredHeight + 20);
        _animator.SetBool("Show", true);
    }
    public void Hide()
    {
        _animator.SetBool("Show", false);
    }
}
