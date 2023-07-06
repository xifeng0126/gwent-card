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
//        // ������Ƶ����ֻ����һ��
//        if (!videoPlayed && firstPlay)
//        {
//            videoPlayer.Play();
//            videoPlayed = true;
//            firstPlay = false;
//            //music.GetComponent<AudioManager>().stopPlay();
//        }

//        // ���¿ո��ʱ������������Ƶ
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
//        // ��Ƶ������ɺ�ִ�����ٲ���
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
        // ���¿ո��ʱ������������Ƶ
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
