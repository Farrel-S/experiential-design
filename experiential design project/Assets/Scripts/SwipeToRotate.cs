using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeToRotate : MonoBehaviour
{

    public GameObject objectGroup;
    public float rotationSpeed;
    public InputAction tapAction;
    private InputAction pointerPositionAction;
    private InputAction pointerPressAction;
    // Camera used for screen-to-world raycasts (defaults to Camera.main if null)
    public Camera raycastCamera;
    // Optional layer mask to restrict what can be selected for rotation
    public LayerMask rotatableMask = ~0; // default: everything
    // Optional tag filter: only objects (or their parents) with this tag are rotatable
    public string rotatableTag = "Rotatable";
    public bool requireTag = true;
    // If true, when nothing is selected, rotate the default `objectGroup`; if false, do nothing
    public bool fallbackToDefault = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        // Keep the tapAction if other systems want to hook into a press event,
        // but we handle pointer/mouse input in Update for hold-and-drag rotation.
        tapAction = new InputAction(name: "Press", binding: "<Pointer>/press");
        tapAction.performed += OnPress;

        // Create pointer actions to support continuous tracking via the
        // new Input System. Binding to <Pointer> covers both mouse and touch.
        pointerPositionAction = new InputAction(name: "PointerPosition", binding: "<Pointer>/position");
        pointerPressAction = new InputAction(name: "PointerPress", binding: "<Pointer>/press");

        // Hook press started/canceled to keep the same semantic as mouse button
        pointerPressAction.performed += ctx => OnPointerPressPerformed(ctx);
        pointerPressAction.canceled += ctx => OnPointerPressCanceled(ctx);
    }

    private void OnEnable()
    {
        // Enable actions so they will read values when present.
        tapAction?.Enable();
        pointerPositionAction?.Enable();
        pointerPressAction?.Enable();
    }

    private void OnDisable()
    {
        tapAction?.Disable();
        pointerPositionAction?.Disable();
        pointerPressAction?.Disable();
    }

    private void OnPointerPressPerformed(InputAction.CallbackContext ctx)
    {
        // When a press starts, initialize dragging state and capture the
        // current pointer position so subsequent deltas are computed.
        isPressing = true;
        if (pointerPositionAction != null)
        {
            lastPointerPos = pointerPositionAction.ReadValue<Vector2>();
        }

        // On press start, select the object under the pointer (if any)
        TrySelectTarget(lastPointerPos);
    }

    private void OnPointerPressCanceled(InputAction.CallbackContext ctx)
    {
        // Press released
        isPressing = false;
    }

    void Start()
    {
        if (objectGroup != null)
        {
            objectGroup.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the assigned objectGroup around its local X axis while the
        // primary pointer (mouse left button or touch) is held and moved.
        if (objectGroup == null) return;

        // We'll support both the new Input System (if available) and the old Input API.
        bool pressed = false;
        Vector2 pointerPos = Vector2.zero;

        // Prefer InputAction-based pointer (covers mouse and touch through the
        // new Input System). If those actions aren't available, fall back to
        // direct device reading or the legacy Input API.
        if (pointerPressAction != null && pointerPositionAction != null && pointerPressAction.enabled)
        {
            // Read press as a float (0/1) and position directly from the actions.
            try
            {
                float pressVal = pointerPressAction.ReadValue<float>();
                pressed = pressVal > 0.5f;
                pointerPos = pointerPositionAction.ReadValue<Vector2>();
            }
            catch
            {
                // If ReadValue fails for any reason, fall back below.
                pressed = false;
                pointerPos = Vector2.zero;
            }
        }
        else if (Mouse.current != null)
        {
            // New Input System, but using device APIs directly
            pressed = Mouse.current.leftButton.isPressed;
            pointerPos = Mouse.current.position.ReadValue();
        }
        else
        {
            // Fallback to the old Input system
            pressed = Input.GetMouseButton(0);
            pointerPos = (Vector2)Input.mousePosition;
        }

        HandlePointer(pressed, pointerPos);
    }

    public void OnSwipe(Vector2 swipeDelta)
    {
        float rotationY = swipeDelta.x * rotationSpeed;
        Transform target = currentTarget;
        if (target == null && fallbackToDefault && objectGroup != null)
        {
            target = objectGroup.transform;
        }
        if (target == null) return; // no selection, and no fallback
        target.Rotate(0, -rotationY, 0, Space.World);
    }

    // Internal state for drag rotation
    private bool isPressing = false;
    private Vector2 lastPointerPos = Vector2.zero;
    private Transform currentTarget = null;

    private void HandlePointer(bool pressed, Vector2 pointerPos)
    {
        if (pressed)
        {
            if (!isPressing)
            {
                // start of press/drag
                isPressing = true;
                lastPointerPos = pointerPos;
                TrySelectTarget(pointerPos);
                return;
            }

            // compute delta since last frame
            Vector2 delta = pointerPos - lastPointerPos;
            lastPointerPos = pointerPos;

            // Horizontal movement will rotate around local Y axis (yaw)
            float rotY = delta.x * -rotationSpeed;
            Transform target = currentTarget;
            if (target == null && fallbackToDefault && objectGroup != null)
            {
                target = objectGroup.transform;
            }
            if (target != null)
            {
                target.Rotate(0f, rotY, 0f, Space.Self);
            }
        }
        else
        {
            // released
            isPressing = false;
            currentTarget = null;
        }
    }

    private void OnPress(InputAction.CallbackContext context)
    {
        Debug.Log("Screen Pressed");
    }

    private void TrySelectTarget(Vector2 screenPos)
    {
        // Use configured camera or fall back
        Camera cam = raycastCamera != null ? raycastCamera : Camera.main;
        if (cam == null) return;

        // Raycast into the scene from the screen position
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, rotatableMask))
        {
            Transform candidate = hitInfo.transform;
            if (requireTag && !HasTagInParents(candidate, rotatableTag))
            {
                currentTarget = null;
                return;
            }
            // Prefer the nearest parent that carries the tag if requireTag is true
            currentTarget = requireTag ? FindTaggedAncestor(candidate, rotatableTag) ?? candidate : candidate;
        }
    }

    private bool HasTagInParents(Transform t, string tag)
    {
        Transform cur = t;
        while (cur != null)
        {
            if (cur.CompareTag(tag)) return true;
            cur = cur.parent;
        }
        return false;
    }

    private Transform FindTaggedAncestor(Transform t, string tag)
    {
        Transform cur = t;
        while (cur != null)
        {
            if (cur.CompareTag(tag)) return cur;
            cur = cur.parent;
        }
        return null;
    }
}
