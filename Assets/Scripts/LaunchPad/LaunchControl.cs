using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class LaunchControl : MonoBehaviour
{

    public float orbitalAltitude;
    public float ascentSpeed;
    public Vector3 initialTilt;
    public Vector3 tiltDownOffset;
    public Gradient backgroundGradient;
    public GameObject groundSet;
    public Color skyColor;
    public GameObject launchCanvas;
    public GameObject launchViewer;
    public Camera mainCamera;
    public GameObject[] objectsToActivate;
    public float delayBetween = 0.5f;

    private Camera _cam;
    private bool _hasReachedorbitalAltitude = false;

    void Awake()
    {
        _cam = mainCamera;
        _cam.clearFlags = CameraClearFlags.SolidColor;
        _cam.backgroundColor = skyColor;
    }

    public void StartLaunch()
    {
        objectsToActivate[0].SetActive(false);
        // reset state if you want to allow re-launching
        _hasReachedorbitalAltitude = false;
        StartCoroutine(RotateRoutine(initialTilt));  // initial tilt
        StartCoroutine(LaunchSequence());
    }

    private IEnumerator LaunchSequence()
    {
        while (true)
        {
            float y = transform.position.y;

            // 1) Ascend until threshold
            if (!_hasReachedorbitalAltitude)
                transform.Translate(Vector3.up * ascentSpeed * Time.deltaTime, Space.Self);

            // 2) On first crossing of orbitalAltitude
            if (!_hasReachedorbitalAltitude && y >= orbitalAltitude)
            {
                _hasReachedorbitalAltitude = true;
                groundSet.SetActive(false);
                _cam.clearFlags = CameraClearFlags.Skybox;

                // tilt down, then start spinning
                // StartCoroutine(RotateRoutine(tiltDownOffset));
            }

            // 3) While below threshold, update sky gradient
            if (!_hasReachedorbitalAltitude)
            {
                float t = Mathf.Clamp01(y / orbitalAltitude);
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

    // LAUNCH setup
    public void SetupLaunch()
    {
        launchCanvas.SetActive(false);
        StartCoroutine(MoveCamera(new Vector3(554f, 73f, 560f), 50));
        StartCoroutine(ActivateSequence());
    }

    IEnumerator MoveCamera(Vector3 targetPosition, float moveSpeed)
    {
        while (Vector3.Distance(mainCamera.transform.position, targetPosition) > 0.01f)
        {
            mainCamera.transform.position = Vector3.MoveTowards(
                mainCamera.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        mainCamera.transform.position = targetPosition;
    }

    private IEnumerator ActivateSequence()
    {
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            if(i == 1){
                objectsToActivate[1].SetActive(true);
                StartCoroutine(DoShake());
                objectsToActivate[2].SetActive(true);
            }else if(i == 2){
                Invoke(nameof(StartLaunch), 2f);
            }
            else{
                objectsToActivate[i].SetActive(true);
            }
            yield return new WaitForSeconds(delayBetween);
        }
    }


    public float shakeDuration = 2f;
    public float shakeMagnitude = 0.1f;
    private IEnumerator DoShake()
    {
        // Cache the original local position so we can reset
        Vector3 originalPos = mainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // Pick a random point inside a unit circle for X and Y
            Vector2 offset = Random.insideUnitCircle * shakeMagnitude;
            
            // Apply to localPosition (so it respects any parent transform)
            mainCamera.transform.localPosition = originalPos + new Vector3(offset.x, offset.y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Restore exact original position
        mainCamera.transform.localPosition = originalPos;
    }
}
