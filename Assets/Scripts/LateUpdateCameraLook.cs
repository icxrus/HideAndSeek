using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class LateUpdateCameraLook : MonoBehaviour
{
    [Header("References")]
    public CinemachineVirtualCamera virtualCamera;
    public InputActionReference lookAction;

    [Header("Sensitivity")]
    public float horizontalSensitivity = 1.5f;
    public float verticalSensitivity = 1.5f;

    [Header("Invert Axes")]
    public bool invertHorizontal = false;
    public bool invertVertical = true; // true because Unity's pitch goes positive-down

    [Header("Vertical Clamp")]
    public float minVerticalAngle = -70f;
    public float maxVerticalAngle = 70f;

    [Header("Input Smoothing")]
    public bool enableSmoothing = true;
    public float smoothingSpeed = 10f;

    private CinemachinePOV pov;
    private Vector2 smoothedDelta;

    void Awake()
    {
        pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
    }

    void OnEnable()
    {
        lookAction.action.Enable();
    }

    void OnDisable()
    {
        lookAction.action.Disable();
    }

    void LateUpdate()
    {
        Vector2 rawDelta = lookAction.action.ReadValue<Vector2>();

        // Optional smoothing
        if (enableSmoothing)
        {
            smoothedDelta = Vector2.Lerp(smoothedDelta, rawDelta, smoothingSpeed * Time.deltaTime);
        }
        else
        {
            smoothedDelta = rawDelta;
        }

        // Apply inversion settings
        float finalHorizontalDelta = smoothedDelta.x * (invertHorizontal ? -1f : 1f);
        float finalVerticalDelta = smoothedDelta.y * (invertVertical ? -1f : 1f);

        // Apply input with sensitivity
        pov.m_HorizontalAxis.Value += finalHorizontalDelta * horizontalSensitivity;
        pov.m_VerticalAxis.Value += finalVerticalDelta * verticalSensitivity;

        // Clamp vertical axis
        pov.m_VerticalAxis.Value = Mathf.Clamp(pov.m_VerticalAxis.Value, minVerticalAngle, maxVerticalAngle);
    }
}
