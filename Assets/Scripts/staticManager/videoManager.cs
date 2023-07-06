//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Video;

//public class videoManager : MonoBehaviour
//{
//    public GameObject music;
//    private VideoPlayer videoPlayer;
//    private bool videoPlayed;
//    private bool firstPlay;
//    // Start is called before the first frame update

//    void Awake()
//    {
//        videoPlayer = GetComponent<VideoPlayer>();
//        videoPlayed = false;
//        firstPlay = true;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        // 播放视频，并只播放一次
//        if (!videoPlayed && firstPlay)
//        {
//            videoPlayer.Play();
//            videoPlayed = true;
//            firstPlay = false;
//            //music.GetComponent<AudioManager>().stopPlay();
//        }

//        // 按下空格键时，跳过开场视频
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            videoPlayer.Stop();
//            videoPlayer.gameObject.SetActive(false);
//            //music.GetComponent<AudioManager>().startPaly();
//        }
//    }
//    private void Start()
//    {
//        videoPlayer.loopPointReached += OnVideoCompleted;
//    }
//    private void OnVideoCompleted(VideoPlayer vp)
//    {
//        // 视频播放完成后执行销毁操作
//        vp.gameObject.SetActive(false);
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class videoManager : MonoBehaviour
{
    public GameObject music;
    private VideoPlayer videoPlayer;
    private bool videoPlayed;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayed = PlayerPrefs.GetInt("VideoPlayed", 0) == 1;

        if (!videoPlayed)
        {
            PlayVideo();
            PlayerPrefs.SetInt("VideoPlayed", 1);
        }
        else
        {
            videoPlayer.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 按下空格键时，跳过开场视频
        if (Input.GetKeyDown(KeyCode.Space))
        {
            videoPlayer.Stop();
            videoPlayer.gameObject.SetActive(false);
        }
    }

    private void PlayVideo()
    {
        videoPlayer.Play();
    }

    private void OnDestroy()
    {
        PlayerPrefs.Save();
    }
}
