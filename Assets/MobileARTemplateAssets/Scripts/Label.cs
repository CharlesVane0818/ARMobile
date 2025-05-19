using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;


public enum LabelType
{
    Default,
    Info,
    Warning,
    Error
}

public class Label : MonoBehaviour
{
    [SerializeField] private LabelType labelType;
    [SerializeField] private int labelId;
    
    public LabelType LabelType => labelType;
    public int LabelId => labelId;

    public UnityEvent<LabelType, int> OnLabelClicked;

    private void Awake()
    {
        if (LabelManager.Instance != null)
        {
            OnLabelClicked.AddListener(LabelManager.Instance.HandleLabelClick);
        }
        // GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().selectEntered.AddListener(OnSelectEntered);
        // GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().selectExited.AddListener(OnSelectExited);
        // GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().hoverEntered.AddListener(OnHoverEntered);
        // GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().hoverExited.AddListener(OnHoverExited);
    }

    private void OnDestroy()
    {
        if (LabelManager.Instance != null)
        {
            OnLabelClicked.RemoveListener(LabelManager.Instance.HandleLabelClick);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnLabelClicked?.Invoke(labelType, labelId);
    }

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        OnLabelClicked?.Invoke(labelType, labelId);
    }

    // public void OnSelectExited(SelectExitEventArgs args) {}
    // public void OnHoverEntered(HoverEnterEventArgs args) {}
    // public void OnHoverExited(HoverExitEventArgs args) {}
}