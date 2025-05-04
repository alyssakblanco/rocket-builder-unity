// using UnityEngine;
// using System.Collections; 

// public class LaunchSetup : MonoBehaviour
// {
//     public GameObject launchViewer;
//     public GameObject canvas;
//     public Camera mainCamera;

//     public GameObject[] objectsToActivate;

//     public float delayBetween = 0.5f;

//     public void SetupLaunch()
//     {
//         canvas.SetActive(false);
//         StartCoroutine(MoveCamera(new Vector3(557f, 75f, 561f), 50));
//         StartCoroutine(ActivateSequence());
//     }

//     IEnumerator MoveCamera(Vector3 targetPosition, float moveSpeed)
//     {
//         while (Vector3.Distance(mainCamera.transform.position, targetPosition) > 0.01f)
//         {
//             mainCamera.transform.position = Vector3.MoveTowards(
//                 mainCamera.transform.position,
//                 targetPosition,
//                 moveSpeed * Time.deltaTime
//             );
//             yield return null;
//         }
//         mainCamera.transform.position = targetPosition;
//     }

//     private IEnumerator ActivateSequence()
//     {
//         for (int i = 0; i < objectsToActivate.Length; i++)
//         {
//             if(i == 1){
//                 objectsToActivate[1].SetActive(true);
//                 objectsToActivate[2].SetActive(true);
//             }else if(i == 2){
//                 LaunchControl.StartLaunch();
//             }
//             else{
//                 objectsToActivate[i].SetActive(true);
//             }
//             yield return new WaitForSeconds(delayBetween);
//         }
//     }
// }
