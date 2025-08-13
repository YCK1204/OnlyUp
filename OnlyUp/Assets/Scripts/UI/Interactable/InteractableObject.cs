using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public InteractableObjectData Data;
    void Start()
    {
        if (Data == null)
            Debug.LogError("InteractableObjectData is not assigned.");
        Init();
    }
    protected virtual void Init() { }
    private void OnMouseEnter()
    {
        Manager.UI.ShowObjectSummary(this);
    }
    private void OnMouseExit()
    {
        Manager.UI.HideObjectSummary();
    }
    public abstract void Use();
}
