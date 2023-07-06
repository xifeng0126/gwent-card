using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class videoManager : MonoBehaviour
{
    public GameObject music;
    private VideoPlayer videoPlayer;
    private bool videoPlayed;

    AudioManager audioManager;
    // Start is called before the first frame update

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayed = false;
        audioManager = GameObject.FindGameObjectsWithTag("AudioManager")[0].GetComponent<AudioManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        // 播放视频，并只播放一次
        if (!videoPlayed)
        {
            videoPlayer.Play();
            videoPlayed = true;
            //music.GetComponent<AudioManager>().stopPlay();
            if(audioManager != null)
                audioManager.stopPlay();
        }

        // 按下空格键时，跳过开场视频
        if (Input.GetKeyDown(KeyCode.Space))
        {
            videoPlayer.Stop();
            videoPlayer.gameObject.SetActive(false);
            //music.GetComponent<AudioManager>().startPaly();
            if (audioManager != null)
                audioManager.startPaly();
        }
    }
    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoCompleted;
    }
    private void OnVideoCompleted(VideoPlayer vp)
    {
        // 视频播放完成后执行销毁操作
        Destroy(vp.gameObject);
    }
}
