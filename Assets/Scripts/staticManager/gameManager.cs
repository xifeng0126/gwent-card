using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnApplicationQuit()
    {
        DBmanager.exitPlayer(Convert.ToInt32(PhotonNetwork.AuthValues.UserId));
        PlayerPrefs.SetInt("VideoPlayed", 0);
        PlayerPrefs.Save();

    }
}
