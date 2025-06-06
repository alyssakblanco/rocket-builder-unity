using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(Camera))]
public class LaunchControl : MonoBehaviour
{
    [Header("Flight Settings")]
    private float OrbitalAltitude = 50073f;
    private float AscentSpeed = 1f;
    public Vector3 InitialTilt = Vector3.zero;
    public Gradient BackgroundGradient;
    public Color SkyColor;

    [Header("References")]
    public Camera MainCamera;
    public GameObject GroundSet;
    public GameObject LaunchCanvas;
    public TextMeshProUGUI HeightText;
    public GameObject heightIndicator;
    public GameObject InflightInfo;
    public GameObject MissionSuccess;
    public GameObject MissionFail;
    public GameObject Earth;
    public GameObject LaunchView;
    public GameObject Smoke;

    [Header("Sequence Settings")]
    public float DelayBetweenActivations = 0.5f;
    public float ShakeDuration = 2f;
    public float ShakeMagnitude = 0.1f;

    private Camera _camera;
    private GameObject _rocket;
    private bool _reachedOrbitalAltitude;
    private bool hasReachedSpace;

    private const float HeightOffset = 73f;
    private const float KarmanLine = 10073f;
    private GameObject Thrusters;

    // inflight facts
    private float fireInterval = 20f;
    private float _nextFireTime = 0f;
    private bool missionComplete = true;

    private void Awake()
    {
        _camera = MainCamera;
        _camera.clearFlags = CameraClearFlags.SolidColor;
        _camera.backgroundColor = SkyColor;
    }

    private void Start()
    {
        _rocket = GameObject.FindWithTag("Rocket");
        foreach (Transform child in _rocket.GetComponentsInChildren<Transform>(true))
        {
            if (child.CompareTag("fire"))
            {
                Thrusters = child.gameObject;
                break;
            }
        }
    }

    private void Update()
    {
        float currentHeight = transform.position.y - HeightOffset;
        HeightText.text = $"{(currentHeight * 10f):N0} m";

        if (!missionComplete && (Time.time >= _nextFireTime))
        {
            _nextFireTime = Time.time + fireInterval;
            GetComponent<RandomFactSlider>().ShowRandomFact();
        }
    }

    public void SetupLaunch()
    {
        LaunchCanvas.SetActive(false);
        StartCoroutine(MoveToPosition(MainCamera.transform, new Vector3(554f, 73f, 560f), 50f));
        StartCoroutine(ActivateSequence());
    }

    public void StartLaunch()
    {
        _reachedOrbitalAltitude = false;
        hasReachedSpace = false;
        missionComplete = false;

        StartCoroutine(RotateTransform(MainCamera.transform, InitialTilt, 5f));
        StartCoroutine(MoveToLocalPosition(MainCamera.transform, new Vector3(69f, -73f, 0f), 5f));
        StartCoroutine(LaunchSequence());
    }

    private IEnumerator LaunchSequence()
    {
        while (true)
        {
            float altitude = transform.position.y;

            if (!_reachedOrbitalAltitude)
            {
                transform.Translate(Vector3.up * AscentSpeed * Time.deltaTime, Space.Self);
            }

            if(!hasReachedSpace){
                AscentSpeed += 0.5f;
                UpdateSkyGradient(altitude);
            } else {
                AscentSpeed += 0.5f;
            }

            if(altitude >= KarmanLine - 5000){
                if(!GameData.makesItToSpace){
                    missionComplete = true;
                }
            }
            if(altitude >= KarmanLine - 2000){
                if(!GameData.makesItToSpace){
                    StopAllCoroutines();
                    Thrusters.SetActive(false);
                    StartCoroutine(EndSequence(true));
                } 
            }
            if(altitude >= KarmanLine){
                hasReachedSpace = true;
                GroundSet.SetActive(false);
                _camera.clearFlags = CameraClearFlags.Skybox;
            }


            if (!_reachedOrbitalAltitude && altitude >= OrbitalAltitude - 5000)
            {
                missionComplete = true;
            }
            if (!_reachedOrbitalAltitude && altitude >= OrbitalAltitude)
            {
                _reachedOrbitalAltitude = true;
                StartCoroutine(HorizontalShift());
                yield break;
            }

            yield return null;
        }
    }

    private void UpdateSkyGradient(float height)
    {
        float t = Mathf.Clamp01(height / KarmanLine);
        _camera.backgroundColor = BackgroundGradient.Evaluate(t);
    }

    private IEnumerator RotateTransform(Transform target, Vector3 eulerOffset, float duration)
    {
        Quaternion start = target.rotation;
        Quaternion end = start * Quaternion.Euler(eulerOffset);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            target.rotation = Quaternion.Slerp(start, end, elapsed / duration);
            yield return null;
        }

        target.rotation = end;
    }

    private IEnumerator RotateToEuler(Transform thing, Vector3 targetEuler, float duration = 5f)
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

    private IEnumerator MoveToPosition(Transform target, Vector3 destination, float speed)
    {
        while (Vector3.Distance(target.position, destination) > 0.01f)
        {
            target.position = Vector3.MoveTowards(target.position, destination, speed * Time.deltaTime);
            yield return null;
        }

        target.position = destination;
    }

    private IEnumerator MoveToLocalPosition(Transform target, Vector3 destination, float speed)
    {
        Vector3 start = target.localPosition;
        float elapsed = 0f;

        while (elapsed < speed)
        {
            elapsed += Time.deltaTime;
            // t goes 0→1 with eased curve
            float t = Mathf.SmoothStep(0f, 1f, elapsed / speed);
            target.localPosition = Vector3.Lerp(start, destination, t);
            yield return null;
        }

        target.localPosition = destination;
    }

    private IEnumerator ActivateSequence()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == 1)
            {
                Thrusters.SetActive(true);
                StartCoroutine(ShakeCamera());
                Smoke.SetActive(true);
            }
            else if (i == 2)
            {
                Invoke(nameof(StartLaunch), 2f);
            }

            yield return new WaitForSeconds(DelayBetweenActivations);
        }
    }

    private IEnumerator ShakeCamera()
    {
        Vector3 originalPos = MainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < ShakeDuration)
        {
            elapsed += Time.deltaTime;
            Vector2 offset = Random.insideUnitCircle * ShakeMagnitude;
            MainCamera.transform.localPosition = originalPos + new Vector3(offset.x, offset.y, 0f);
            yield return null;
        }

        MainCamera.transform.localPosition = originalPos;
    }

    private IEnumerator HorizontalShift()
    {
        missionComplete = true;
        // hesitate at top
        // yield return new WaitForSeconds(2f);
        // earth 
        Earth.SetActive(true);
        StartCoroutine(MoveToPosition(LaunchView.transform, new Vector3(251f,47468f,-535f), 100f));
        // StartCoroutine(MoveToPosition(LaunchView.transform, new Vector3(251f ,-1598f ,-535f), 100f));
        // rocket
        yield return StartCoroutine(RotateToEuler(_rocket.transform, new Vector3(13f, 225f, 272f), 2f));
        // kill engines
        Thrusters.SetActive(false);
        // camera
        StartCoroutine(MoveToLocalPosition(MainCamera.transform, new Vector3(-120f, 111f, -88f), 10f));
        yield return StartCoroutine(RotateToEuler(MainCamera.transform, new Vector3(30f, 0f, 0f), 5f));
        StartCoroutine(EndSequence(false));
    }

    private IEnumerator EndSequence(bool failsBeforeSpace)
    {
        StartCoroutine(MoveToLocalPosition(heightIndicator.transform, new Vector3(351f ,190f ,0f ), 1f));
        
        if(GameData.missionStatus){
            MissionSuccess.SetActive(true);
            if(failsBeforeSpace){
                StartCoroutine(RotateToEuler(_rocket.transform, new Vector3(13f, 225f, 272f), 10f));
                StartCoroutine(MoveToLocalPosition(MainCamera.transform, new Vector3(-16f, -102f, 17f), 10f));
            }
        }else{
            MissionFail.SetActive(true);
            if(failsBeforeSpace){
                StartCoroutine(RotateToEuler(_rocket.transform, new Vector3(13f, 225f, 272f), 10f));
                StartCoroutine(MoveToLocalPosition(MainCamera.transform, new Vector3(-16f, -102f, 17f), 10f));
            }
        }

        yield break;
    }
}