using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class LabelManager : MonoBehaviour
{
    public static LabelManager Instance { get; private set; }
    
    public UnityEvent<LabelType, int> OnClickLabel = new UnityEvent<LabelType, int>();

    private Camera mainCamera;
    private float lastClickTime;
    private const float clickCooldown = 0.5f; // 防抖时间间隔(秒)
    private bool isMouseDown; // 鼠标按下状态标志

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        InputSystem.onEvent += OnInputEvent;
    }

    private void OnDisable()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (!eventPtr.IsA<StateEvent>())
            return;
            
        // 处理鼠标/触摸按下事件
        bool isPressed = false;
        Vector2 position = Vector2.zero;
        
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            isPressed = true;
            position = Mouse.current.position.ReadValue();
        }
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            isPressed = true;
            position = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        
        if (isPressed)
        {
            if (!isMouseDown)
            {
                isMouseDown = true;
                
                // 防抖检查
                if (Time.time - lastClickTime < clickCooldown)
                    return;
                    
                lastClickTime = Time.time;
                
                Ray ray = mainCamera.ScreenPointToRay(position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Label label = hit.collider.GetComponent<Label>();
                    if (label != null)
                    {
                        HandleLabelClick(label.LabelType, label.LabelId);
                    }
                }
            }
        }
        else if (isMouseDown)
        {
            // 鼠标/触摸抬起时重置状态
            isMouseDown = false;
        }
    }

    public void HandleLabelClick(LabelType type, int id)
    {
        OnClickLabel.Invoke(type, id);
        Debug.Log($"Label {type} {id} clicked");
    }
}