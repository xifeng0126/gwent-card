using I18N.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using System;

public class setCardManager : MonoBehaviour
{
    //public GameObject card;
    //public GameObject cardPosition;

    //public Sprite sprite;
    //public Texture2D texture;
    public UImanager uimanager;

    //全部卡牌
    public List<Card> northernCards;
    public List<Card> nilfgaardianCards;
    public List<Card> monsterCards;
    public List<Card> scoiataelCards;
    public List<Card> neutralCards;

    //组卡时不同阵营的所有卡牌，第i项为Card_id
    public int[] northernCardsList;
    public int[] nilfgaardianCardsList;
    public int[] monsterCardsList;
    public int[] scoiataelCardsList;


    //玩家登录后设置为玩家的卡组
    public Deck northernDeck;
    public Deck nilfgaardianDeck;
    public Deck monsterDeck;
    public Deck scoiataelDeck;

    void Start()
    {
        //uimanager = this.GetComponent<UImanager>();

        northernCards = DBmanager.selectCardsByCamp("northern");
        nilfgaardianCards = DBmanager.selectCardsByCamp("nilfgaardian");
        monsterCards = DBmanager.selectCardsByCamp("monster");
        scoiataelCards = DBmanager.selectCardsByCamp("scoiatael");
        neutralCards = DBmanager.selectCardsByCamp("neutral");

        northernCardsList = new int[141];
        nilfgaardianCardsList = new int[141];
        monsterCardsList = new int[141];
        scoiataelCardsList = new int[141];

        setDeck();

        //uimanager.initial();
        //PhotonNetwork.ConnectUsingSettings();

    }

    public void setDeck()
    {
        //后续修改，根据登录的玩家的每个卡组设置,玩家登录时默认分配4个卡组id，初始化卡组时，用玩家卡组id查找卡组，为空则新建，不为空则查找该卡组
        CardPlayer player = DBmanager.selectPlayerById(Convert.ToInt32(PhotonNetwork.AuthValues.UserId));

        if (player == null)
        {
            Debug.Log("玩家不存在");
            return;
        }
        northernDeck = DBmanager.selectDeckByDeckId(player.northerndeck);
        if (northernDeck == null)
        {
            northernDeck = new Deck(player.northerndeck, player.player_id, "northernDeck");
        }
        nilfgaardianDeck = DBmanager.selectDeckByDeckId(player.nilfgaardiandeck);
        if (nilfgaardianDeck == null)
        {
            nilfgaardianDeck = new Deck(player.nilfgaardiandeck, player.player_id, "nilfgaardianDeck");
        }
        monsterDeck = DBmanager.selectDeckByDeckId(player.monsterdeck);
        if (monsterDeck == null)
        {
            monsterDeck = new Deck(player.monsterdeck, player.player_id, "monsterDeck");
        }
        scoiataelDeck = DBmanager.selectDeckByDeckId(player.scoiataeldeck);
        if (scoiataelDeck == null)
        {
            scoiataelDeck = new Deck(player.scoiataeldeck, player.player_id, "scoiataelDeck");
        }

        innicialCardList();
        //uimanager.initial();
    }

    //初始化不同阵营牌库
    public void innicialCardList()
    {
        foreach(Card card in northernCards)
            northernCardsList[card.id] = card.number;
        foreach (Card card in nilfgaardianCards)
            nilfgaardianCardsList[card.id] = card.number;
        foreach (Card card in monsterCards)
            monsterCardsList[card.id] = card.number;
        foreach (Card card in scoiataelCards)
            scoiataelCardsList[card.id] = card.number;
        foreach (Card card in neutralCards)
        {
            northernCardsList[card.id] = card.number;
            nilfgaardianCardsList[card.id] = card.number;
            monsterCardsList[card.id] = card.number;
            scoiataelCardsList[card.id] = card.number;
        }
        if(northernDeck.Leader!=null)
            northernCardsList[northernDeck.Leader.id] -= 1;
        if (nilfgaardianDeck.Leader != null)
            nilfgaardianCardsList[nilfgaardianDeck.Leader.id] -= 1;
        if (monsterDeck.Leader != null)
            monsterCardsList[monsterDeck.Leader.id] -= 1;
        if (scoiataelDeck.Leader != null)
            scoiataelCardsList[scoiataelDeck.Leader.id] -= 1;

        foreach (Card card in northernDeck.Special)
            northernCardsList[card.id] -= 1;
        foreach (Card card in nilfgaardianDeck.Special)
            nilfgaardianCardsList[card.id] -= 1;
        foreach (Card card in monsterDeck.Special)
            monsterCardsList[card.id] -= 1;
        foreach (Card card in scoiataelDeck.Special)
            scoiataelCardsList[card.id] -= 1;

        foreach (Card card in northernDeck.Base)
            northernCardsList[card.id] -= 1;
        foreach (Card card in nilfgaardianDeck.Base)
            nilfgaardianCardsList[card.id] -= 1;
        foreach (Card card in monsterDeck.Base)
            monsterCardsList[card.id] -= 1;
        foreach (Card card in scoiataelDeck.Base)
            scoiataelCardsList[card.id] -= 1;
    }

    //根据阵营和卡牌id获取卡牌数量
    public int getNumber(string camp,int id,int camp_id)
    {
        if (camp == "northern" || camp_id == 0)
            return northernCardsList[id];
        else if (camp == "nilfgaardian" || camp_id == 1)
            return nilfgaardianCardsList[id];
        else if (camp == "monster" || camp_id == 2)
            return monsterCardsList[id];
        else if(camp=="scoiatael" || camp_id == 3)
            return scoiataelCardsList[id];
        else
            return 0;
    }

    //选择后减少牌库中数量
    public void subCardNum(string camp, int id, int camp_id, int num)
    {
        if (camp == "northern" || camp_id == 0)
            northernCardsList[id] -= num;
        else if (camp == "nilfgaardian" || camp_id == 1)
            nilfgaardianCardsList[id] -= num;
        else if (camp == "monster" || camp_id == 2)
            monsterCardsList[id] -= num;
        else if (camp == "scoiatael" || camp_id == 3)
            scoiataelCardsList[id] -= num;
    }

    public void onReturnClick()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }


    //public override void OnJoinRandomFailed(short returnCode, string message)
    //{
    //    PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    //}
    //public override void OnJoinedRoom()
    //{
    //    Debug.Log("加入成功");
    //}

    //public const byte MoveUnitsToTargetPositionEventCode = 1;
    //public void SendCard()
    //{

    //    object[] content=new object[1];
    //    int i = northernCards[0].id;
    //    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; 
    //    PhotonNetwork.RaiseEvent(MoveUnitsToTargetPositionEventCode, i, raiseEventOptions, SendOptions.SendReliable);
    //    Debug.Log("发送成功");
    //}
    //public override void OnConnectedToMaster()
    //{
    //    Debug.Log("连接成功");
    //    PhotonNetwork.JoinRandomRoom();
    //}

    //private void OnEnable()
    //{
    //    PhotonNetwork.AddCallbackTarget(this);
    //}

    //private void OnDisable()
    //{
    //    PhotonNetwork.RemoveCallbackTarget(this);
    //}
    //public void OnEvent(EventData photonEvent)
    //{
    //    Debug.Log("接收成功");
    //    byte eventCode = photonEvent.Code;
    //    Debug.Log(eventCode);
    //    if (eventCode == MoveUnitsToTargetPositionEventCode)
    //    {
    //        Card card = DBmanager.selectCardById((int)photonEvent.CustomData);
    //        Debug.Log(card.name);
    //    }
    //}

    void OnApplicationQuit()
    {
        DBmanager.exitPlayer(Convert.ToInt32(PhotonNetwork.AuthValues.UserId));
    }
}
