using UnityEngine;

public class CameraAscend : MonoBehaviour
{
    public float ascendSpeed = 10f;
    public float targetHeight = 500f;

    private bool ascending = true;

    void Update()
    {
        // if (ascending && transform.position.y < targetHeight)
        // {
        //     Vector3 newPos = transform.position;
        //     newPos.y += ascendSpeed * Time.deltaTime;
        //     transform.position = newPos;
        // }
    }
}
