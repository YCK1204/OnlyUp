using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableObject : MonoBehaviour
{
    public InteractableObjectData Data;
    void Start()
    {
        if (Data == null)
        {
            Debug.LogError("InteractableObjectData is not assigned.");
        }
    }
    private void OnMouseEnter()
    {
        Manager.UI.ShowObjectSummary(this);
    }
    private void OnMouseExit()
    {
        Manager.UI.HideObjectSummary();
    }
}
