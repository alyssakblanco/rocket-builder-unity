using UnityEngine;

[AddComponentMenu("Camera/Float Bob")]
public class FloatingCamera : MonoBehaviour
{
    [Header("Bob Settings")]
    [Tooltip("Overall strength of the bobbing motion.")]
    public float amplitude = 5f;
    [Tooltip("Speed of the bobbing motion.")]
    public float frequency = 0.3f;

    [Header("Noise Settings")]
    [Tooltip("Speed of the random drift.")]
    public float noiseSpeed = 2f;
    [Tooltip("Strength of the random drift.")]
    public float noiseStrength = 0.2f;

    Vector3 _startPos;
    Quaternion _startRot;

    void Start()
    {
        _startPos = transform.localPosition;
        _startRot = transform.localRotation;
    }

    void Update()
    {
        float t = Time.time;

        // 1) Smooth sine bob
        float bobX = Mathf.Sin(t * frequency) * amplitude;
        float bobY = Mathf.Sin(t * frequency * 0.7f) * amplitude * 0.6f;
        float bobZ = Mathf.Cos(t * frequency * 1.3f) * amplitude * 0.4f;

        // 2) Perlin-noise drift for randomness
        float noiseX = (Mathf.PerlinNoise(t * noiseSpeed, 0f) - 0.5f) * 2f * noiseStrength;
        float noiseY = (Mathf.PerlinNoise(0f, t * noiseSpeed) - 0.5f) * 2f * noiseStrength;
        float noiseZ = (Mathf.PerlinNoise(t * noiseSpeed, t * noiseSpeed) - 0.5f) * 2f * noiseStrength;

        // Apply position offset
        transform.localPosition = _startPos + new Vector3(bobX + noiseX, bobY + noiseY, bobZ + noiseZ);

        // Optional: tiny rotational drift to simulate drifting attitude
        float rotX = Mathf.Sin(t * frequency * 0.2f) * amplitude * 0.1f;
        float rotY = noiseY * 5f;
        float rotZ = Mathf.Cos(t * frequency * 0.15f) * amplitude * 0.1f;
        transform.localRotation = _startRot * Quaternion.Euler(rotX, rotY, rotZ);
    }
}
