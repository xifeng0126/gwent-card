using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //将要轮流播放的音乐组
    public AudioClip[] audioGroup;

    //当前播放的是谁
    private int playingIndex;

    //是否允许播放音乐
    private bool canPlayAudio;

    //AudioSource组件
    private AudioSource audioSource;

    private bool video = false;

    //-----------------------------------------------------

    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();

        canPlayAudio = true;

        playingIndex = 0;

        if (GameObject.FindGameObjectsWithTag("AudioManager").Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }


    void Update()
    {
        if (canPlayAudio)
        {
            PlayAudio();   

            canPlayAudio = false;
        }

        if (!audioSource.isPlaying&&!video)
        {
            playingIndex++;

            if (playingIndex >= audioGroup.Length)
            {
                playingIndex = 0;
            }

            canPlayAudio = true;
        }
    }


    public void PlayAudio()
    {
        audioSource.clip = audioGroup[playingIndex];
        audioSource.Play();
    }
    
    public void stopPlay()
    {
        canPlayAudio = false;
        video = true;
        audioSource.Stop();
    }

    public void startPaly()
    {
        video = false;
        PlayAudio();
    }

}