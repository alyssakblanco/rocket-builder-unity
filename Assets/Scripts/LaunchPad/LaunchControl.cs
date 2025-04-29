using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class LaunchControl : MonoBehaviour
{
    [Header("Flight Settings")]
    [Tooltip("Height at which skybox is enabled and ascent stops")]
    public float disableHeight;
    [Tooltip("Ascent speed (units/sec)")]
    public float ascentSpeed;

    [Header("Rotation Settings")]
    [Tooltip("Initial upward tilt (degrees)")]
    public Vector3 initialTilt;
    [Tooltip("Tilt-down offset when reaching disableHeight (degrees)")]
    public Vector3 tiltDownOffset;
    [Tooltip("Spin speed around Y axis after disableHeight (°/sec)")]
    public float spinSpeed;

    [Header("Camera & Environment")]
    public Camera launchThing;
    public Gradient backgroundGradient;
    public GameObject groundSet;
    public Color skyColor;

    public GameObject launchCanvas;

    private Camera _cam;
    private bool _hasReachedDisableHeight = false;

    void Awake()
    {
        _cam = launchThing;
        _cam.clearFlags = CameraClearFlags.SolidColor;
        _cam.backgroundColor = skyColor;
    }

    /// <summary>
    /// Call this from your UI button (OnClick → LaunchControl.StartLaunch)
    /// </summary>
    public void StartLaunch()
    {
        launchCanvas.SetActive(false);
        // reset state if you want to allow re-launching
        _hasReachedDisableHeight = false;
        StartCoroutine(RotateRoutine(initialTilt));  // initial tilt
        StartCoroutine(LaunchSequence());
    }

    /// <summary>
    /// Drives ascent, gradient update, threshold-triggered tilt & spin.
    /// </summary>
    private IEnumerator LaunchSequence()
    {
        while (true)
        {
            float y = transform.position.y;

            // 1) Ascend until threshold
            if (!_hasReachedDisableHeight)
                transform.Translate(Vector3.up * ascentSpeed * Time.deltaTime, Space.Self);

            // 2) On first crossing of disableHeight
            if (!_hasReachedDisableHeight && y >= disableHeight)
            {
                _hasReachedDisableHeight = true;
                groundSet.SetActive(false);
                _cam.clearFlags = CameraClearFlags.Skybox;

                // tilt down, then start spinning
                StartCoroutine(RotateRoutine(tiltDownOffset));
                StartCoroutine(SpinYAxis());
            }

            // 3) While below threshold, update sky gradient
            if (!_hasReachedDisableHeight)
            {
                float t = Mathf.Clamp01(y / disableHeight);
                _cam.backgroundColor = backgroundGradient.Evaluate(t);
            }

            yield return null;
        }
    }

    // Smoothly apply an Euler offset over time
    private IEnumerator RotateRoutine(Vector3 eulerOffset, float duration = 5f)
    {
        Quaternion start = transform.rotation;
        Quaternion end   = start * Quaternion.Euler(eulerOffset);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(start, end, elapsed / duration);
            yield return null;
        }

        transform.rotation = end;
    }

    // Continuous Y-axis spin
    private IEnumerator SpinYAxis()
    {
        while (true)
        {
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.Self);
            yield return null;
        }
    }
}
