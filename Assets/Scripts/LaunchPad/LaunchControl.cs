using UnityEngine;
using System.Collections;
using TMPro;

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
    public TextMeshProUGUI heightText;
    public GameObject inflightInfo;
    public GameObject missionSuccess;
    public GameObject missionFail;
    public GameObject earth;

    private Camera _cam;
    private bool _hasReachedorbitalAltitude = false;
    private GameObject rocket;

    void Awake()
    {
        _cam = mainCamera;
        _cam.clearFlags = CameraClearFlags.SolidColor;
        _cam.backgroundColor = skyColor;
        
    }

    void Start(){
        rocket = GameObject.FindWithTag("Rocket");
    }

    void Update()
    {
        // Compute current height from this object’s Y position
        float currentHeight = transform.position.y - 73;

        // Update the UI every frame
        heightText.text = $"{currentHeight:F0} m";
    }

    public void StartLaunch()
    {
        // turn off smoke
        objectsToActivate[0].SetActive(false);

        // todo: set orbitalAltitude based on mission outcome

        // reset state if you want to allow re-launching
        _hasReachedorbitalAltitude = false;
        StartCoroutine(RotateCamera(initialTilt)); 
        StartCoroutine(MoveCameraLocal(new Vector3(69f, -73f, 0f), 20f));
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
                
                StartCoroutine(EndSequence());
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

    private IEnumerator RotateLaunchView(Vector3 eulerOffset, float duration = 5f)
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

    private IEnumerator RotateCamera(Vector3 eulerOffset, float duration = 5f)
    {
        Quaternion start = mainCamera.transform.rotation;
        Quaternion end   = start * Quaternion.Euler(eulerOffset);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            mainCamera.transform.rotation = Quaternion.Slerp(start, end, elapsed / duration);
            yield return null;
        }
        mainCamera.transform.rotation = end;
    }

    private IEnumerator RotateObject(GameObject thing, Vector3 eulerOffset, float duration = 5f)
    {
        Quaternion start = thing.transform.rotation;
        Quaternion end   = start * Quaternion.Euler(eulerOffset);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            thing.transform.rotation = Quaternion.Slerp(start, end, elapsed / duration);
            yield return null;
        }
        thing.transform.rotation = end;
    }

    private IEnumerator RotateToEuler(Transform  thing, Vector3 targetEuler, float duration = 5f)
    {
        // if you want to respect parent orientation, use localRotation;
        // otherwise swap Local↔World as needed
        Quaternion start = thing.transform.localRotation;
        Quaternion end   = Quaternion.Euler(targetEuler);
        float elapsed    = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            thing.transform.localRotation = Quaternion.Slerp(start, end, t);
            yield return null;
        }
        thing.transform.localRotation = end;
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

    IEnumerator MoveObject(Transform thing, Vector3 targetPosition, float moveSpeed)
    {
        while (Vector3.Distance(thing.transform.position, targetPosition) > 0.01f)
        {
            thing.transform.position = Vector3.MoveTowards(
                thing.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        thing.transform.position = targetPosition;
    }

    IEnumerator MoveCameraLocal(Vector3 targetLocalPos, float moveSpeed)
    {
        // move its localPosition instead of world position
        while (Vector3.Distance(mainCamera.transform.localPosition, targetLocalPos) > 0.01f)
        {
            mainCamera.transform.localPosition = Vector3.MoveTowards(
                mainCamera.transform.localPosition,
                targetLocalPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        mainCamera.transform.localPosition = targetLocalPos;
    }

    IEnumerator MoveObjectLocal(GameObject thing, Vector3 targetLocalPos, float moveSpeed)
    {
        // move its localPosition instead of world position
        while (Vector3.Distance(thing.transform.localPosition, targetLocalPos) > 0.01f)
        {
            thing.transform.localPosition = Vector3.MoveTowards(
                thing.transform.localPosition,
                targetLocalPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        thing.transform.localPosition = targetLocalPos;
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
                Invoke(nameof(StartLaunch), 1f);
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

    // launch end
    public IEnumerator EndSequence(){
        // activate earth
        earth.SetActive(true);
        // move up earth
        // start at -623 -1613 -1035
        StartCoroutine(MoveObject(earth.transform, new Vector3(-1106f, -501f, -1035), 10f));
        // turn rocket

        // turn camera
        StartCoroutine(MoveCameraLocal(new Vector3(-154f, 12f, -120f), 10f));
        StartCoroutine(RotateToEuler(mainCamera.transform, new Vector3(-10f, 32f, 0f), 10f));

        StartCoroutine(RotateToEuler(rocket.transform, new Vector3(303.617889f,223.776978f,308.639282f), 10f));
        StartCoroutine(MoveObjectLocal(rocket, new Vector3(182.129501f,-23.6542969f,32.101059f), 10f));
        

        // kill burn 
        objectsToActivate[1].SetActive(false);
        // hide altitude
        inflightInfo.SetActive(false);
        // show mission status
        missionSuccess.SetActive(true);

        yield return null;
    }
}
        // StartCoroutine(RotateToEuler(rocket.transform, new Vector3(0f, 36f, -52f), 10f));
        // StartCoroutine(MoveObjectLocal(rocket, new Vector3(70f, 0f, 0f), 10f));
        // StartCoroutine(MoveCameraLocal(new Vector3(-255f, 116f, -203f), 10f));
        // StartCoroutine(RotateToEuler(mainCamera.transform, new Vector3(0f, 23f, 0f), 10f));