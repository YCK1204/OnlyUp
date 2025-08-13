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
        if (Manager.UI == null)
        {
            Manager.UI = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _currentObject != null)
            _currentObject.Use();
    }
}
