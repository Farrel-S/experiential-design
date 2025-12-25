using UnityEngine;
using Vuforia;

public class SwipeToRotate : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 0.2f;

    [Header("Zoom")]
    public float zoomSpeed = 0.01f;
    public float minScale = 0.5f;
    public float maxScale = 2.0f;

    private Vector2 lastInputPosition;
    private bool isDragging = false;

    private ObserverBehaviour imageTarget;
    private bool isTracked = false;

    void Awake()
    {
        imageTarget = GetComponentInParent<ObserverBehaviour>();
    }

    void OnEnable()
    {
        if (imageTarget != null)
            imageTarget.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    void OnDisable()
    {
        if (imageTarget != null)
            imageTarget.OnTargetStatusChanged -= OnTargetStatusChanged;
    }

    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        isTracked = status.Status == Status.TRACKED ||
                    status.Status == Status.EXTENDED_TRACKED;
    }

    void Update()
    {
        if (!isTracked)
            return;

#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    // ------------------------
    // Mouse (PC / Editor)
    // ------------------------
    void HandleMouseInput()
    {
        // Rotate
        if (Input.GetMouseButtonDown(0))
        {
            lastInputPosition = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastInputPosition;
            transform.Rotate(Vector3.up, delta.x * rotationSpeed, Space.Self);
            lastInputPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // Zoom (mouse wheel simulates pinch)
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            Zoom(scroll * zoomSpeed * 10f);
        }
    }

    // ------------------------
    // Touch (Mobile)
    // ------------------------
    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastInputPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.position - lastInputPosition;
                transform.Rotate(Vector3.up, delta.x * rotationSpeed, Space.Self);
                lastInputPosition = touch.position;
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            float prevDist = (t0.position - t0.deltaPosition -
                              (t1.position - t1.deltaPosition)).magnitude;
            float currDist = (t0.position - t1.position).magnitude;

            float delta = currDist - prevDist;
            Zoom(delta * zoomSpeed);
        }
    }

    void Zoom(float amount)
    {
        Vector3 scale = transform.localScale;
        scale += Vector3.one * amount;
        scale = Vector3.Max(Vector3.one * minScale, Vector3.Min(Vector3.one * maxScale, scale));
        transform.localScale = scale;
    }
}
