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
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayed = false;
        audioManager = GameObject.FindGameObjectsWithTag("AudioManager")[0].GetComponent<AudioManager>();
        
        videoPlayed = PlayerPrefs.GetInt("VideoPlayed", 0) == 1;

        if (!videoPlayed)
        {
            PlayVideo();
            PlayerPrefs.SetInt("VideoPlayed", 1);
            if (audioManager != null)
                audioManager.stopPlay();
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
            //music.GetComponent<AudioManager>().startPaly();
            if (audioManager != null)
                audioManager.startPaly();
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
