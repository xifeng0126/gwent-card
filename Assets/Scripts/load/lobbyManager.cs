using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lobbyManager : MonoBehaviourPunCallbacks
{
    RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2 };

    bool hasDeck = false;

    // Start is called before the first frame update
    void Start()
    {
        CardPlayer player = DBmanager.selectPlayerById(Convert.ToInt32(PhotonNetwork.AuthValues.UserId));
        Deck deck1= DBmanager.selectDeckByDeckId(player.northerndeck);
        Deck deck2 = DBmanager.selectDeckByDeckId(player.nilfgaardiandeck);
        Deck deck3 = DBmanager.selectDeckByDeckId(player.monsterdeck);
        Deck deck4 = DBmanager.selectDeckByDeckId(player.scoiataeldeck);
        if(deck1 != null || deck2 != null || deck3 != null || deck4 != null)
        {
            hasDeck = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnSetCardClick()
    {
        PhotonNetwork.LoadLevel("SetCardTable");
    }

    public void onStartClick()
    {
        Debug.Log("后续修改");
        if(!hasDeck)
        {
            //提示请先设置卡组
            Debug.Log("请先设置卡组");
            return;
        }
        //PhotonNetwork.JoinOrCreateRoom("test", roomOptions, PhotonNetwork.CurrentLobby);
        PhotonNetwork.JoinRandomRoom();
        //PhotonNetwork.JoinRoom("test");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
        //Debug.Log("创建房间");
    }
    //public override void OnCreatedRoom()
    //{
    //    Debug.Log("创建房间");
    //}
    public override void OnJoinedRoom()
    {
        //Debug.Log("加入成功");
        PhotonNetwork.LoadLevel("FightTable");
    }
    //public override void OnJoinRoomFailed(short returnCode, string message)
    //{
    //    Debug.Log("加入失败");
    //}

    //private void CreateRoom()
    //{
    //    RoomOptions roomOptions = new RoomOptions();
    //    roomOptions.MaxPlayers = 2;
    //    PhotonNetwork.CreateRoom("test", roomOptions, null);
    //    Debug.Log("创建房间");
    //    Debug.Log(PhotonNetwork.CurrentLobby);
    //}

    //public override void OnJoinRandomFailed(short returnCode, string message)
    //{
    //    CreateRoom();
    //}

    //public override void OnJoinedRoom()
    //{
    //    // joined a room successfully
    //    Debug.Log("加入成功");
    //    //PhotonNetwork.LoadLevel("FightTable");
    //}
    //public override void OnJoinRoomFailed(short returnCode, string message)
    //{
    //    Debug.Log("加入失败");
    //    Debug.Log(message);
    //    CreateRoom();
    //}

    //public override void OnCreateRoomFailed(short returnCode, string message)
    //{
    //    Debug.Log(message);
    //}
}
