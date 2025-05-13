using UnityEngine;
using UnityEngine.Video;

public class VidPlayer : MonoBehaviour
{
    [SerializeField] string videoFileName;
     VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        PlayVideo();
    }

    public void PlayVideo(){
        videoPlayer.Play();
    }

    public void ChangeVideoSource(string videoFileName){
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        videoPlayer.url = videoPath;
        videoPlayer.Play();
    }
}
