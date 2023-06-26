using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class FightManager : MonoBehaviour, IOnEventCallback
{
    bool waitFlag = true;
    bool startFlag = true;
    public Dictionary<string, Deck> decks = new Dictionary<string, Deck>();

    bool my_getReady = false;   //当双方都选择好卡组时，开始游戏
    bool other_getReady = false;
    public const byte GetReadyEventCode = 1;

    public int gameflag;   //控制哪个玩家出牌，用playerId赋值 
    public const byte GameFalgEventCode = 2;

    public const byte HandCardsEventCode = 3;  
    public const byte PutCardsEventCode = 4;  //对方出牌

    int Myplayer_id;
    int Otherplayer_id;



    CardPlayer player;
    public GameObject miniCard;
    public GameObject cardBack_monster;
    public GameObject cardBack_nilfgaardian;
    public GameObject cardBack_northern;
    public GameObject cardBack_scoiatael;

    public GameObject MessagePanel;
    public GameObject selectDeckPanel;
    public GameObject GamePanel;
    public GameObject MyGameImages;
    public GameObject OtherGameImages;
    public GameObject roundPanel;

    public List<Card> MygameCards;
    public List<Card> OthergameCards;
    public Deck MygameDeck;
    public Deck OtherGameDeck;

    public int MyAllAttackNum;
    public int MyCloseAttackNum;
    public int MyRemoteAttackNum;
    public int MyCityAttackNum;

    public int OtherAllAttackNum;
    public int OtherCloseAttackNum;
    public int OtherRemoteAttackNum;
    public int OtherCityAttackNum;

    public int MyHandCardNum;
    public int OtherHandCardNum;
    public List<Card> MyHandCardList;
    public List<Card> OtherHandCardList;

    public int PartGame;  //当为2时开始一小局



    // Start is called before the first frame update
    void Start()
    {
        Myplayer_id = Convert.ToInt32(PhotonNetwork.AuthValues.UserId);
        player = DBmanager.selectPlayerById(Myplayer_id);

        MygameCards = new List<Card>();
        OthergameCards = new List<Card>();
        MyHandCardList = new List<Card>();
        OtherHandCardList = new List<Card>();

        MessagePanel.SetActive(false);
        selectDeckPanel.SetActive(false);
        GamePanel.SetActive(false);
        roundPanel.SetActive(false);

        innicialDeck();
        setAllNumZero();
        PartGame = 0;
    }

    // Update is called once per frame
    void Update()
    {
        int playerNum = PhotonNetwork.PlayerList.Length;
        waitPlayer(playerNum);

        Timer.Update();
    }
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void innicialDeck()
    {
        decks.Add("northern", DBmanager.selectDeckByDeckId(player.northerndeck));
        decks.Add("nilfgaardian", DBmanager.selectDeckByDeckId(player.nilfgaardiandeck));
        decks.Add("monster", DBmanager.selectDeckByDeckId(player.monsterdeck));
        decks.Add("scoiatael", DBmanager.selectDeckByDeckId(player.scoiataeldeck));
    }
    public void waitPlayer(int playerNum)
    {
        if (playerNum == 1 && waitFlag == true)
        {
            waitFlag = false;
            wait();   //后续修改
            //Debug.Log("后续修改");
            //startGame();
        }
        if (playerNum == 2 && startFlag == true)
        {
            startFlag = false;
            startGame();
        }
    }

    public void wait()  //等待对手
    {
        MessagePanel.SetActive(true);
    }

    public void startGame() //开始游戏
    {
        //yield return new WaitForSeconds(delay);

        MessagePanel.SetActive(false);
        selectDeckPanel.SetActive(true);

        foreach (var deck in decks)
        {
            if (deck.Value != null)
            {
                UnityEngine.UI.Button btn = selectDeckPanel.transform.Find(deck.Key).AddComponent<UnityEngine.UI.Button>();
                btn.onClick.AddListener(() =>
                {
                    selectDeck(deck.Value);
                });
            }
            else
            {
                ImageManager.setDullColor(selectDeckPanel.transform.Find(deck.Key + "/mask/Image").GetComponent<UnityEngine.UI.Image>());
            }
        }
    }

    public void selectDeck(Deck deck)
    {
        //Debug.Log(1);
        MygameDeck = deck;
        MygameCards.Clear();
        MygameCards.Add(deck.Leader);
        foreach (var card in deck.Special)
        {
            MygameCards.Add(card);
        }
        foreach (var card in deck.Base)
        {
            MygameCards.Add(card);
        }
        selectDeckPanel.SetActive(false);
        //Debug.Log(gameCards.Count);
        
        my_getReady = true;
        MessagePanel.SetActive(true);
        MessagePanel.transform.Find("Message").GetComponent<TextMeshProUGUI>().text = "等待对手选择卡组";
        SendGetReady(deck);
    }

    public void SendGetReady(Deck deck)  //发送消息
    {
        int data = deck.deck_id;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GetReadyEventCode, data, raiseEventOptions, SendOptions.SendReliable);   //自己准备好后发送自己的卡组id
        //allGetReady();
    }
    public void OnEvent(EventData photonEvent)  //接收消息
    {
        byte eventCode = photonEvent.Code;
        Debug.Log(eventCode);
        if (eventCode == GetReadyEventCode)     //双方选好卡组信息
        {
            allGetReady();
            OtherGameDeck = DBmanager.selectDeckByDeckId((int)photonEvent.CustomData);
            //OtherGameDeck = MygameDeck; //后续修改
            int id=OtherGameDeck.player_id;
            if (id == Myplayer_id)   //自己发送的消息
            {
                return;
            }
            else  //对手发送的消息，即收到不是自己的id后即可认为对手已经准备好
            {
                other_getReady = true;
                Otherplayer_id = id;

                OthergameCards.Add(OtherGameDeck.Leader);
                foreach (var card in OtherGameDeck.Special)
                {
                    OthergameCards.Add(card);
                }
                foreach (var card in OtherGameDeck.Base)
                {
                    OthergameCards.Add(card);
                }
            }
            allGetReady();
        }
        if (eventCode == GameFalgEventCode)   //设置先手信息后开始游戏
        {
            gameflag = (int)photonEvent.CustomData;
            PartGame++;
            if (PartGame == 2)    //后续修改
            {
                startPartGame();
            }
        }
        if(eventCode == HandCardsEventCode) //对方手牌信息
        {
            int[] data = (int[])photonEvent.CustomData;
            if (data[10] == Myplayer_id)
            {
                return;
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    OtherHandCardList.Add(DBmanager.selectCardById(data[i]));
                }
                onStartPartGame();
            }
        }
    }

    public void allGetReady()
    {
        if (my_getReady == false || other_getReady == false)
        {
            return;
        }
        //Debug.Log("后续修改");

        Debug.Log("allGetReady");
        MessagePanel.SetActive(false);

        GamePanel.SetActive(true);
        instanceCard(MygameDeck.Leader, MyGameImages.transform.Find("cards/Leader"));
        instanceBack(MygameDeck.deck_camp, MyGameImages.transform.Find("cards/back"));
        instanceCard(OtherGameDeck.Leader, OtherGameImages.transform.Find("cards/Leader")); 
        instanceBack(OtherGameDeck.deck_camp,OtherGameImages.transform.Find("cards/back"));
        //instanceCard(MygameDeck.Leader, OtherGameImages.transform.Find("cards/Leader"));  
        //instanceBack(MygameDeck.deck_camp, OtherGameImages.transform.Find("cards/back"));

        SetFlag();
    }

    public void SetFlag()
    {
        int flag = UnityEngine.Random.Range(0, 2);
        if (flag == 0)
        {
            gameflag = Myplayer_id;
        }
        else if (flag == 1)
        {
            gameflag = Otherplayer_id;
        }

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameFalgEventCode, gameflag, raiseEventOptions, SendOptions.SendReliable);

    }


    public void instanceCard(Card card,Transform position)
    {
        GameObject instance = Instantiate(miniCard, position.transform.position, position.transform.rotation, position.transform);
        MiniCardControl MiniCardControl = instance.GetComponent<MiniCardControl>();
        MiniCardControl.card = card;
        MiniCardControl.display();
    }
    public void instanceBack(string camp, Transform position)
    {
        if (camp == "northernDeck")
        {
            Instantiate(cardBack_northern, position.transform.position, position.transform.rotation, position.transform);
        }
        if (camp == "nilfgaardianDeck")
        {
            Instantiate(cardBack_nilfgaardian, position.transform.position, position.transform.rotation, position.transform);
        }
        if (camp == "monsterDeck")
        {
            Instantiate(cardBack_monster, position.transform.position, position.transform.rotation, position.transform);
        }
        if (camp == "scoiataelDeck")
        {
            Instantiate(cardBack_scoiatael, position.transform.position, position.transform.rotation, position.transform);
        }
    }

    public void setMyAllAttackNum(int num)
    {
        MyGameImages.transform.Find("AllNum/AllAttackNum").GetComponent<TextMeshProUGUI>().text = num.ToString();
        MyAllAttackNum = num;
    }
    public void setMyCloseAttackNum(int num)
    {
        MyGameImages.transform.Find("AllNum/closeNum").GetComponent<TextMeshProUGUI>().text = num.ToString();
        MyCloseAttackNum = num;
    }
    public void setMyRemoteAttackNum(int num)
    {
        MyGameImages.transform.Find("AllNum/remoteNum").GetComponent<TextMeshProUGUI>().text = num.ToString();
        MyRemoteAttackNum = num;
    }
    public void setMyCityAttackNum(int num)
    {
        MyGameImages.transform.Find("AllNum/cityNum").GetComponent<TextMeshProUGUI>().text = num.ToString();
        MyCityAttackNum = num;
    }
    public void setOtherAllAttackNum(int num)
    {
        OtherGameImages.transform.Find("AllNum/AllAttackNum").GetComponent<TextMeshProUGUI>().text = num.ToString();
        OtherAllAttackNum = num;
    }
    public void setOtherCloseAttackNum(int num)
    {
        OtherGameImages.transform.Find("AllNum/closeNum").GetComponent<TextMeshProUGUI>().text = num.ToString();
        OtherCloseAttackNum = num;
    }
    public void setOtherRemoteAttackNum(int num)
    {
        OtherGameImages.transform.Find("AllNum/remoteNum").GetComponent<TextMeshProUGUI>().text = num.ToString();
        OtherRemoteAttackNum = num;
    }
    public void setOtherCityAttackNum(int num)
    {
        OtherGameImages.transform.Find("AllNum/cityNum").GetComponent<TextMeshProUGUI>().text = num.ToString();
        OtherCityAttackNum = num;
    }
    public void setMyHandCardNum(int num)
    {
        MyGameImages.transform.Find("AllNum/handCardNum").GetComponent<TextMeshProUGUI>().text = num.ToString();
        MyHandCardNum = num;
    }
    public void setOtherHandCardNum(int num)
    {
        OtherGameImages.transform.Find("AllNum/handCardNum").GetComponent<TextMeshProUGUI>().text = num.ToString();
        OtherHandCardNum = num;
    }
    public void setAllNumZero()
    {
        setMyAllAttackNum(0);
        setMyCloseAttackNum(0);
        setMyRemoteAttackNum(0);
        setMyCityAttackNum(0);
        setOtherAllAttackNum(0);
        setOtherCloseAttackNum(0);
        setOtherRemoteAttackNum(0);
        setOtherCityAttackNum(0);
        setMyHandCardNum(0);
        setOtherHandCardNum(0);
    }



    public void startPartGame()
    {
        MygameCards.Remove(MygameDeck.Leader);
        OthergameCards.Remove(OtherGameDeck.Leader);

        //随机从MygameCards中取10个元素，写入MyHandCardList
        int[] handcards = new int[11];   //[10]为自己的id
        for (int i = 0; i < 10; i++)
        {
            int index = UnityEngine.Random.Range(0, MygameCards.Count);
            MyHandCardList.Add(MygameCards[index]);
            handcards[i] = MygameCards[index].id;
            MygameCards.RemoveAt(index);
        }
        handcards[10] = Myplayer_id;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(HandCardsEventCode, handcards, raiseEventOptions, SendOptions.SendReliable);
    }
    
    public void onStartPartGame()
    {
        clearHandCard();
        foreach (Card card in MyHandCardList)
        {
            instanceMiniCard(card, GamePanel.transform.Find("MyHandCards").transform);
        }
        showRound();
    }

    public void instanceMiniCard(Card card, Transform position)
    {
        GameObject instance = Instantiate(miniCard, position.transform.position, position.transform.rotation, position.transform);
        MiniCardControl MiniCardControl = instance.GetComponent<MiniCardControl>();
        MiniCardControl.card = card;
        MiniCardControl.setAttac();
        MiniCardControl.display();

        LayoutRebuilder.ForceRebuildLayoutImmediate(GamePanel.transform.Find("MyHandCards").GetComponent<RectTransform>());
        DragHandler dragHandler = instance.AddComponent<DragHandler>();
        dragHandler.SetGamePanel(MyGameImages,OtherGameImages,this);
    }
    
    public void clearHandCard()
    {
        foreach (Transform child in GamePanel.transform.Find("MyHandCards").transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void showRound()
    {
        if (Myplayer_id == gameflag)
        {
            roundPanel.SetActive(true);
            roundPanel.transform.Find("round").GetComponent<TextMeshProUGUI>().text = "你的回合";
            Timer.StartTimer(1f,() =>
            {
                roundPanel.SetActive(false);
            });
        }
        else
        {
            roundPanel.SetActive(true);
            roundPanel.transform.Find("round").GetComponent<TextMeshProUGUI>().text = "对方回合";
            Timer.StartTimer(1f, () =>
            {
                roundPanel.SetActive(false);
            });
        }
        CalculateAttackNum();
    }

    //计算显示所有数值
    public void CalculateAttackNum()    
    {
        int MyhandCards = GamePanel.transform.Find("MyHandCards").childCount;
        setMyHandCardNum(MyhandCards);
        //Debug.Log(MyhandCards);  


        Transform[] closeAttackList = MyGameImages.transform.Find("closeAttackList").GetComponentsInChildren<Transform>();
        int closeAttackNum = 0;
        foreach (Transform child in closeAttackList)
        {
            if (child.GetComponent<MiniCardControl>() != null)
            {
                closeAttackNum += child.GetComponent<MiniCardControl>().attackNum;
            }
        }
        setMyCloseAttackNum(closeAttackNum);
        //处理remoteAttackList
        Transform[] remoteAttackList = MyGameImages.transform.Find("remoteAttackList").GetComponentsInChildren<Transform>();
        int remoteAttackNum = 0;
        foreach (Transform child in remoteAttackList)
        {
            if (child.GetComponent<MiniCardControl>() != null)
            {
                remoteAttackNum += child.GetComponent<MiniCardControl>().attackNum;
            }
        }
        setMyRemoteAttackNum(remoteAttackNum);
        //处理cityAttackList
        Transform[] cityAttackList = MyGameImages.transform.Find("cityAttackList").GetComponentsInChildren<Transform>();
        int cityAttackNum = 0;
        foreach (Transform child in cityAttackList)
        {
            if (child.GetComponent<MiniCardControl>() != null)
            {
                cityAttackNum += child.GetComponent<MiniCardControl>().attackNum;
            }
        }
        setMyCityAttackNum(cityAttackNum);
        //处理allAttackList
        setMyAllAttackNum(closeAttackNum + remoteAttackNum + cityAttackNum);


        Transform[] otherCloseAttackList = OtherGameImages.transform.Find("otherCloseAttackList").GetComponentsInChildren<Transform>();
        int otherCloseAttackNum = 0;
        foreach (Transform child in otherCloseAttackList)
        {
            if (child.GetComponent<MiniCardControl>() != null)
            {
                otherCloseAttackNum += child.GetComponent<MiniCardControl>().attackNum;
            }
        }
        setOtherCloseAttackNum(otherCloseAttackNum);
        //处理remoteAttackList
        Transform[] otherRemoteAttackList = OtherGameImages.transform.Find("otherRemoteAttackList").GetComponentsInChildren<Transform>();
        int otherRemoteAttackNum = 0;
        foreach (Transform child in otherRemoteAttackList)
        {
            if (child.GetComponent<MiniCardControl>() != null)
            {
                otherRemoteAttackNum += child.GetComponent<MiniCardControl>().attackNum;
            }
        }
        setOtherRemoteAttackNum(otherRemoteAttackNum);
        //处理cityAttackList
        Transform[] otherCityAttackList = OtherGameImages.transform.Find("otherCityAttackList").GetComponentsInChildren<Transform>();
        int otherCityAttackNum = 0;
        foreach (Transform child in otherCityAttackList)
        {
            if (child.GetComponent<MiniCardControl>() != null)
            {
                otherCityAttackNum += child.GetComponent<MiniCardControl>().attackNum;
            }
        }
        setOtherCityAttackNum(otherCityAttackNum);
        //处理allAttackList
        setOtherAllAttackNum(otherCloseAttackNum + otherRemoteAttackNum + otherCityAttackNum);
    }
}
