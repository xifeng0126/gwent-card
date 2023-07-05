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

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayed = false;
    }

    // Update is called once per frame
    void Update()
    {
        // ������Ƶ����ֻ����һ��
        if (!videoPlayed)
        {
            videoPlayer.Play();
            videoPlayed = true;
            //music.GetComponent<AudioManager>().stopPlay();
        }

        // ���¿ո��ʱ������������Ƶ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            videoPlayer.Stop();
            videoPlayer.gameObject.SetActive(false);
            //music.GetComponent<AudioManager>().startPaly();
        }
    }
    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoCompleted;
    }
    private void OnVideoCompleted(VideoPlayer vp)
    {
        // ��Ƶ������ɺ�ִ�����ٲ���
        Destroy(vp.gameObject);
    }
}
