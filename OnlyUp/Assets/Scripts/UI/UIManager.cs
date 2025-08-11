using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    ObjectUI ObjectUI;

    InteractableObject _currentObject;
    private void Start()
    {
        Manager.UI = this;
        DontDestroyOnLoad(gameObject);
    }
    public void ShowObjectSummary(InteractableObject obj)
    {
        _currentObject = obj;
        ObjectUI.Show(obj.Data);
    }
    public void HideObjectSummary()
    {
        if (_currentObject != null)
        {
            ObjectUI.Hide();
            _currentObject = null;
        }
    }
}
