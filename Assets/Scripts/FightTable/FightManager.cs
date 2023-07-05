using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class FightManager : MonoBehaviour, IOnEventCallback
{
    bool waitFlag = true;
    bool startFlag = true;
    public Dictionary<string, Deck> decks = new Dictionary<string, Deck>();

    public SkillManager skillManager;

    bool my_getReady = false;   //当双方都选择好卡组时，开始游戏
    bool other_getReady = false;
    public const byte GetReadyEventCode = 1;

    public int gameflag;   //控制哪个玩家出牌，用playerId赋值 
    public const byte GameFalgEventCode = 2;

    public const byte HandCardsEventCode = 3;  
    public const byte PutCardsEventCode = 4;  //出牌（放到场面上）
    public const byte PutSkillCardsEventCode = 5;  //使用技能卡
    public const byte RemoveCardEventCode = 6;  //移除卡牌
    public const byte DrawCardEventCode = 7;  //抽卡
    public const byte DockerEventCode = 8;  //医生
    public const byte ClearAttackEventCode = 9;  //清除攻击力
    public const byte ChangeEventCode = 10;   //切换回合
    public const byte GiveupEventCode = 11;  //放弃回合

    public int Myplayer_id;
    public int Otherplayer_id;

    public int myBlood;
    public int OtherBlood;


    CardPlayer player;
    public GameObject miniCard;
    public GameObject largeCard;
    public GameObject disCard;

    public GameObject largeCardPoint;

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
    public GameObject scoiataelPanel;
    public GameObject GameOver;

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

    public List<Card> MyDiscardList;
    public List<Card> OtherDiscardList;

    public List<GameObject> tableCards;  //场上的卡牌
    public GameObject myBack;
    public GameObject otherBack;


    public int PartGame;  //当为2时开始一小局

    public bool MyGiveup;
    public bool OtherGiveup;

    public bool gameover = false;


    // Start is called before the first frame update
    void Start()
    {
        skillManager=this.transform.GetComponent<SkillManager>();

        Myplayer_id = Convert.ToInt32(PhotonNetwork.AuthValues.UserId);
        player = DBmanager.selectPlayerById(Myplayer_id);

        MygameCards = new List<Card>();
        OthergameCards = new List<Card>();
        MyHandCardList = new List<Card>();
        OtherHandCardList = new List<Card>();
        MyDiscardList = new List<Card>();
        OtherDiscardList = new List<Card>();

        MessagePanel.SetActive(false);
        selectDeckPanel.SetActive(false);
        GamePanel.SetActive(false);
        roundPanel.SetActive(false);
        GamePanel.transform.Find("doctor").gameObject.SetActive(false);

        innicialDeck();
        setAllNumZero();
        PartGame = 0;

        MyGiveup = false;
        OtherGiveup=false;

        myBlood = 2;
        OtherBlood = 2;
    }

    // Update is called once per frame
    void Update()
    {
        int playerNum = PhotonNetwork.PlayerList.Length;
        waitPlayer(playerNum);

        //Timer.Update();
    }
    void OnApplicationQuit()
    {
        DBmanager.exitPlayer(Convert.ToInt32(PhotonNetwork.AuthValues.UserId));
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

    public void OnEvent(EventData photonEvent)  //接收消息
    {
        byte eventCode = photonEvent.Code;
        //Debug.Log("event:"+eventCode);
        //双方选好卡组信息
        if (eventCode == GetReadyEventCode)
        {
            allGetReady();
            //int[]
            Deck deck = DBmanager.selectDeckByDeckId((int)photonEvent.CustomData);
            int playerid = deck.player_id;
            if (playerid == Myplayer_id)
            {
                return;
            }
            else  //对手发送的消息，即收到不是自己的id后即可认为对手已经准备好
            {
                //Debug.Log("对手准备好了");
                OtherGameDeck = DBmanager.selectDeckByDeckId((int)photonEvent.CustomData);
                other_getReady = true;
                Otherplayer_id = playerid;

                //OthergameCards.Add(OtherGameDeck.Leader);
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
        //设置先手信息后开始游戏
        if (eventCode == GameFalgEventCode)
        {
            int[] data = (int[])photonEvent.CustomData;
            gameflag = data[0];
            if (data[1] == 0)
            {
                PartGame++;
                if (PartGame == 2)    //后续修改
                {
                    Debug.Log("开始游戏");
                    startPartGame();
                }
            }
            else
            {
                startPartGame();
            }
            
        }
        //对方手牌信息
        if (eventCode == HandCardsEventCode)
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
                    Card cardToRemove = OthergameCards.FirstOrDefault(c => c.id == data[i]);
                    OthergameCards.Remove(cardToRemove);
                }
                onStartPartGame();
            }

        }
        //对方出牌将卡牌放到场面上
        if (eventCode == PutCardsEventCode)  //[player_id,card_id,position，放置]
        {

            int[] data = (int[])photonEvent.CustomData;
            Card card = DBmanager.selectCardById((int)data[1]);

            if (card.skill1 != 5 && card.skill2 != 5 && data[3] == 1 )   //不是医生 /玩家放置的牌
            {
                if (card.skill1 == 6 || card.skill1 == 7 || card.skill1 == 8 || card.skill1 == 9)  //天气牌
                {
                    GameObject instance = instanceLargeCard(card, largeCardPoint.transform);
                    TimerManager.StartTimer(1f, () =>
                    {
                        Destroy(instance);
                    });
                    if (!OtherGiveup && !MyGiveup)
                        TimerManager.StartTimer(1.5f, changeGameFlag);
                }
                else if (card.skill1==13)  //烧灼-近战
                {
                    if (data[0] == Myplayer_id)
                    {
                        int allAttack = 0;
                        int num = 0;
                        List<GameObject> list = new List<GameObject>();
                        foreach(Transform g in OtherGameImages.transform.Find("otherCloseAttackList").GetComponentsInChildren<Transform>())
                        {
                            MiniCardControl miniCardControl = g.GetComponent<MiniCardControl>();
                            if(miniCardControl != null && miniCardControl.card is BasicCard && miniCardControl.card.skill1!=1)
                            {
                                allAttack += miniCardControl.attackNum;
                                if (miniCardControl.attackNum > num)
                                {
                                    num = miniCardControl.attackNum;
                                    list.Clear();
                                    list.Add(g.gameObject);
                                }
                                if(miniCardControl.attackNum == num)
                                {
                                    list.Add(g.gameObject);
                                }
                            }
                        }
                        if (allAttack >= 10)
                        {
                            foreach(GameObject g in list)
                            {
                                tableCards.Remove(g);
                                Destroy(g);
                            }
                        }
                    }
                    else
                    {
                        int allAttack = 0;
                        int num = 0;
                        List<GameObject> list = new List<GameObject>();
                        foreach (Transform g in MyGameImages.transform.Find("closeAttackList").GetComponentsInChildren<Transform>())
                        {
                            MiniCardControl miniCardControl = g.GetComponent<MiniCardControl>();
                            if (miniCardControl != null && miniCardControl.card is BasicCard && miniCardControl.card.skill1 != 1)
                            {
                                allAttack += miniCardControl.attackNum;
                                if (miniCardControl.attackNum > num)
                                {
                                    num = miniCardControl.attackNum;
                                    list.Clear();
                                    list.Add(g.gameObject);
                                }
                                if (miniCardControl.attackNum == num)
                                {
                                    list.Add(g.gameObject);
                                }
                            }
                        }
                        if (allAttack >= 10)
                        {
                            foreach (GameObject g in list)
                            {
                                tableCards.Remove(g);
                                Destroy(g);
                            }
                        }
                    }
                    GameObject instance = instanceLargeCard(card, largeCardPoint.transform);
                    TimerManager.StartTimer(1f, () =>
                    {
                        Destroy(instance);
                    });
                    if (!OtherGiveup && !MyGiveup)
                        TimerManager.StartTimer(1.5f, changeGameFlag);
                }
                else if((card.skill1==14 || card.skill2 == 14) && data[3]==1  )  //集合
                {
                    GameObject instance = instanceLargeCard(card, largeCardPoint.transform);
                    TimerManager.StartTimer(1f, () =>
                    {
                        Destroy(instance);
                    });
                    if(!OtherGiveup && !MyGiveup)
                        TimerManager.StartTimer(1.5f, changeGameFlag);
                }
                else
                {
                    if (!OtherGiveup && !MyGiveup)
                        TimerManager.StartTimer(1f, changeGameFlag);
                }
            }


            int position = (int)data[2];
            if (data[0] != Myplayer_id)
            {
                GameObject ga = null;
                switch (position)
                {
                    case 1:
                        ga = instanceCard(card, OtherGameImages.transform.Find("otherCityAttackList"));
                        break;
                    case 2:
                        ga = instanceCard(card, OtherGameImages.transform.Find("otherRemoteAttackList"));
                        break;
                    case 3:
                        ga = instanceCard(card, OtherGameImages.transform.Find("otherCloseAttackList"));
                        break;
                    case 4:
                        ga = instanceCard(card, MyGameImages.transform.Find("closeAttackList"));
                        break;
                    case 5:
                        ga = instanceCard(card, MyGameImages.transform.Find("remoteAttackList"));
                        break;
                    case 6:
                        ga = instanceCard(card, MyGameImages.transform.Find("cityAttackList"));
                        break;
                    case 7:
                        ga = instanceCard(card, OtherGameImages.transform.Find("otherCityTrump"));
                        break;
                    case 8:
                        ga = instanceCard(card, OtherGameImages.transform.Find("otherRemoteTrump"));
                        break;
                    case 9:
                        ga = instanceCard(card, OtherGameImages.transform.Find("otherCloseTrump"));
                        break;
                    case 10:
                        ga = instanceCard(card, GamePanel.transform.Find("weatherCards"));
                        break;
                    default:
                        break;

                }
                if (ga != null)
                    tableCards.Add(ga);

                if (data[3] == 1)
                {
                    Card cardToRemove = OtherHandCardList.FirstOrDefault(c => c.id == card.id);
                    OtherHandCardList.Remove(cardToRemove);
                }
                //Debug.Log(OtherHandCardList.Count);
            }
            else
            {
                if (data[3] == 1)
                {
                    Card cardToRemove = MyHandCardList.FirstOrDefault(c => c.id == card.id);
                    MyHandCardList.Remove(cardToRemove);
                }
                
            }
            //setHandcardDisabled();

            //处理医生
            if (card.skill1 == 5 || card.skill2 == 5)
            {
                GameObject instance = instanceLargeCard(card, largeCardPoint.transform);
                TimerManager.StartTimer(1f, () =>
                {
                    Destroy(instance);
                });
                if (data[0] == Myplayer_id)
                {
                    TimerManager.StartTimer(1.5f, () =>
                    {
                        foreach (Transform child in GamePanel.transform.Find("doctor/layout/layout"))
                        {
                            GameObject.Destroy(child.gameObject);
                        }

                        GamePanel.transform.Find("doctor").gameObject.SetActive(true);
                        foreach (Card card in MyDiscardList)
                        {
                            if (card.skill1 != 1)
                            {
                                GameObject g = instanceDisCard(card, GamePanel.transform.Find("doctor/layout/layout").transform);
                            }

                        }
                        if (GamePanel.transform.Find("doctor/layout/layout").childCount == 0)
                        {
                            TimerManager.StartTimer(1f, () =>
                            {
                                GamePanel.transform.Find("doctor").gameObject.SetActive(false);
                                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                                PhotonNetwork.RaiseEvent(ChangeEventCode, 0, raiseEventOptions, SendOptions.SendReliable);
                            });
                        }
                    });
                }
                else
                {
                    setHandcardEnabled();
                }
                    
            }
            //处理集合
            if ((card.skill1 == 14 || card.skill2 == 14) && data[3] == 1)
            {
                skillManager.Assembly(data[0], card, data[2]);
            }

            skillManager.setAttack();
            showHandCard();
            CalculateAttackNum();
        }
        //使用技能卡
        if (eventCode == PutSkillCardsEventCode)
        {
            int[] data = (int[])photonEvent.CustomData;
            Card card = DBmanager.selectCardById((int)data[1]);

            if (card.skill1 == 9)  //天晴
            {
                foreach (Transform child in GamePanel.transform.Find("weatherCards"))
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            if(card.skill1==10 || card.skill2 == 10)   //灼烧
            {
                foreach (GameObject child in FindLargestCard())
                {
                    
                    //Card cardToRemove = tableCards.FirstOrDefault(c => c.GetComponent<MiniCardControl>().card.id == child.GetComponent<MiniCardControl>().card.id);   //移除对方技能牌
                    tableCards.Remove(child);
                    GameObject.Destroy(child);
                }
            }

            GameObject instance = instanceLargeCard(card, largeCardPoint.transform);
            TimerManager.StartTimer(1f, () =>
            {
                
                Destroy(instance);
                skillManager.setAttack();

            });
            if (!OtherGiveup && !MyGiveup)
                TimerManager.StartTimer(1.5f, changeGameFlag);

            if (data[0] == Myplayer_id)
            {
                MyDiscardList.Add(card);
                Card cardToRemove = MyHandCardList.FirstOrDefault(c => c.id == card.id);
                MyHandCardList.Remove(cardToRemove);
            }
            else
            {
                OtherDiscardList.Add(card);
                Card cardToRemove = OtherHandCardList.FirstOrDefault(c => c.id == card.id);   //移除对方技能牌
                OtherHandCardList.Remove(cardToRemove);
            }
            //skillManager.setAttack();
            showHandCard();
            CalculateAttackNum();
        }
        //移除卡牌
        if (eventCode == RemoveCardEventCode)
        {
            int[] data = (int[])photonEvent.CustomData;
            int id = data[2];
            //Debug.Log(id);
            //GameObject gameObject = findCardById(data);
            GameObject gameObject = tableCards.First(a => a.GetComponent<MiniCardControl>().card.id == id);
            tableCards.Remove(gameObject);
            if (data[0] == Otherplayer_id)   //诱饵牌，换回对手手牌
            {
                if (data[1] == 1)
                {
                    OtherHandCardList.Add(gameObject.GetComponent<MiniCardControl>().card);
                }
            }
            
            Destroy(gameObject);
            //skillManager.setAttack();
            CalculateAttackNum();
        }
        //抽卡
        if (eventCode == DrawCardEventCode)
        {
            int[] data = (int[])photonEvent.CustomData;
            int id = data[0];
            Card card = DBmanager.selectCardById(data[1]);
            if (id == Myplayer_id)
            {
                return;
            }
            else
            {
                OtherHandCardList.Add(card);
                Card cardToRemove = OthergameCards.FirstOrDefault(c => c.id == card.id);
                OthergameCards.Remove(cardToRemove);
                //Debug.Log(OtherHandCardList)
                //Debug.Log(OtherHandCardList.Count);
                showHandCard();
            }
            
        }
        //医生
        if (eventCode == DockerEventCode)
        {
            int[] data = (int[])photonEvent.CustomData;
            if (data[0] != Myplayer_id)
            {
                Card card = DBmanager.selectCardById(data[1]);
                Card cardToRemove = OtherDiscardList.FirstOrDefault(c => c.id == card.id);
                OtherDiscardList.Remove(cardToRemove);
                OtherHandCardList.Add(card);
            }
            
        }
        if(eventCode== ChangeEventCode)
        {
            if (!OtherGiveup && !MyGiveup)
                TimerManager.StartTimer(1f, changeGameFlag);
        }
        //清除攻击力
        if(eventCode== ClearAttackEventCode)
        {
            TimerManager.StartTimer(1f, () =>
            {
                Debug.Log("clear");
                skillManager.setAttack();
            });
            CalculateAttackNum();
        }
        //放弃回合
        if(eventCode == GiveupEventCode)
        {
            int id = (int)photonEvent.CustomData;
            if (id == Otherplayer_id)
            {
                OtherGiveup = true;
                OtherGameImages.transform.Find("OtherGiveup").gameObject.SetActive(true);
            }
            else
            {
                MyGiveup = true;
            }
            if(!MyGiveup||!OtherGiveup)
                TimerManager.StartTimer(0.5f, changeGameFlag);
            
            allGiveUp();
        }
    }

    public GameObject instanceDisCard(Card card, Transform position)
    {
        GameObject instance = Instantiate(disCard, position.transform.position, position.transform.rotation, position.transform);
        CardControl CardControl = instance.GetComponent<CardControl>();
        CardControl.card = card;
        CardControl.display();
        UnityEngine.UI.Button btn = instance.AddComponent<UnityEngine.UI.Button>();
        btn.onClick.AddListener(() =>
        {
            //tableCards.Add(instance);
            Card cardToRemove = MyDiscardList.FirstOrDefault(c => c.id == card.id);
            MyDiscardList.Remove(cardToRemove);

            MyHandCardList.Add(cardToRemove);
            showHandCard();
            CalculateAttackNum();

            int[] data = new int[2];
            data[0] = Myplayer_id;
            data[1] = cardToRemove.id;
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(DockerEventCode, data, raiseEventOptions, SendOptions.SendReliable);
            
            GamePanel.transform.Find("doctor").gameObject.SetActive(false);

        });
        return instance;
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
        //MygameCards.Add(deck.Leader);
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
        MessagePanel.transform.Find("Button").gameObject.SetActive(false);
        SendGetReady(deck);
    }

    public void SendGetReady(Deck deck)  //发送消息
    {
        int[] data = new int[2];
        data[0] = Myplayer_id;
        data[1] = deck.deck_id;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GetReadyEventCode, deck.deck_id, raiseEventOptions, SendOptions.SendReliable);   //自己准备好后发送自己的卡组id
        //allGetReady();
    }
    


    public void allGetReady()
    {
        if (my_getReady == false || other_getReady == false)
        {
            return;
        }

        //Debug.Log(my_getReady);
        //Debug.Log(other_getReady);
        //Debug.Log("allGetReady");
        MessagePanel.SetActive(false);

        GamePanel.SetActive(true);
        instanceCard(MygameDeck.Leader, MyGameImages.transform.Find("cards/Leader"));
        myBack = instanceBack(MygameDeck.deck_camp, MyGameImages.transform.Find("cards/back"));
        //myBack.GetComponent<cardBackControl>().setCardNum(25);
        instanceCard(OtherGameDeck.Leader, OtherGameImages.transform.Find("cards/Leader")); 
        otherBack = instanceBack(OtherGameDeck.deck_camp,OtherGameImages.transform.Find("cards/back"));
        //otherBack.GetComponent<cardBackControl>().setCardNum(25);
        //instanceCard(MygameDeck.Leader, OtherGameImages.transform.Find("cards/Leader"));  
        //instanceBack(MygameDeck.deck_camp, OtherGameImages.transform.Find("cards/back"));

        SetFlag();
    }

    public void SetFlag()
    {
        int[] data = new int[2];
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        if ((MygameDeck.deck_camp == "scoiataelDeck" && OtherGameDeck.deck_camp == "scoiataelDeck") ||(OtherGameDeck.deck_camp != "scoiataelDeck" && MygameDeck.deck_camp != "scoiataelDeck"))
        {
            //Debug.Log("123456");
            //Debug.Log(MygameDeck.deck_camp);
            //Debug.Log(OtherGameDeck.deck_camp);
            int flag = UnityEngine.Random.Range(0, 2);
            if (flag == 0)
            {
                gameflag = Myplayer_id;
            }
            else if (flag == 1)
            {
                gameflag = Otherplayer_id;
            }
            data[1] = 0;
            data[0] = gameflag;
            PhotonNetwork.RaiseEvent(GameFalgEventCode, data, raiseEventOptions, SendOptions.SendReliable);
        }
        else if(MygameDeck.deck_camp== "scoiataelDeck")
        {
            //Debug.Log("scoiataelPanel");
            data[1] = 1;
            scoiataelPanel.SetActive(true);
            scoiataelPanel.transform.Find("MyFirst").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                gameflag = Myplayer_id;
                scoiataelPanel.SetActive(false);

                data[0] = gameflag;
                PhotonNetwork.RaiseEvent(GameFalgEventCode, data, raiseEventOptions, SendOptions.SendReliable);
            });
            scoiataelPanel.transform.Find("OtherFirst").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                gameflag = Otherplayer_id;
                scoiataelPanel.SetActive(false);

                data[0] = gameflag;
                PhotonNetwork.RaiseEvent(GameFalgEventCode, data, raiseEventOptions, SendOptions.SendReliable);
            }); 
        }
        else if(OtherGameDeck.deck_camp== "scoiataelDeck")
        {
            data[1] = 1;
        }
        
    }


    public GameObject instanceCard(Card card,Transform position)
    {
        //GameObject instance = Instantiate(miniCard, position.transform.position, position.transform.rotation, position.transform);
        //MiniCardControl MiniCardControl = instance.GetComponent<MiniCardControl>();
        //MiniCardControl.card = card;
        //MiniCardControl.display();

        GameObject instance = Instantiate(miniCard, position.transform.position, position.transform.rotation, position.transform);
        MiniCardControl MiniCardControl = instance.GetComponent<MiniCardControl>();
        MiniCardControl.card = card;
        MiniCardControl.setAttack();
        //MiniCardControl.display();
        return instance;
    }
    
    public GameObject instanceBack(string camp, Transform position)
    {
        GameObject ob = null;
        if (camp == "northernDeck")
        {
            ob = Instantiate(cardBack_northern, position.transform.position, position.transform.rotation, position.transform);
        }
        if (camp == "nilfgaardianDeck")
        {
            ob = Instantiate(cardBack_nilfgaardian, position.transform.position, position.transform.rotation, position.transform);
        }
        if (camp == "monsterDeck")
        {
            ob = Instantiate(cardBack_monster, position.transform.position, position.transform.rotation, position.transform);
        }
        if (camp == "scoiataelDeck")
        {
            ob = Instantiate(cardBack_scoiatael, position.transform.position, position.transform.rotation, position.transform);
        }
        return ob;
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
        //Card cardToRemoveMy = MygameCards.FirstOrDefault(c => c.id == MygameDeck.Leader.id);
        //MygameCards.Remove(cardToRemoveMy);
        //Card cardToRemoveOther = OthergameCards.FirstOrDefault(c => c.id == OtherGameDeck.Leader.id);
        //OthergameCards.Remove(cardToRemoveOther);

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
        //Debug.Log("onStartPartGame");
        showHandCard();
        showRound();
    }

    public void showHandCard()
    {
        clearHandCard();
        foreach (Card card in MyHandCardList)
        {
            instanceMiniCard(card, GamePanel.transform.Find("MyHandCards").transform);
        }
        if (gameflag == Myplayer_id)
            setHandcardEnabled();
        else
            setHandcardDisabled();

        //GameObject.Find("MyGameImages/AllNum/handCardNum").GetComponent<TextMeshProUGUI>().text = MyHandCardList.Count.ToString();
        //Debug.Log(MyHandCardList.Count);
        myBack.GetComponent<cardBackControl>().setCardNum(MygameCards.Count);
        otherBack.GetComponent<cardBackControl>().setCardNum(OthergameCards.Count);
    }

    public void instanceMiniCard(Card card, Transform position)
    {
        GameObject instance = Instantiate(miniCard, position.transform.position, position.transform.rotation, position.transform);
        MiniCardControl MiniCardControl = instance.GetComponent<MiniCardControl>();
        MiniCardControl.card = card;
        MiniCardControl.setAttack();
        //MiniCardControl.display();

        LayoutRebuilder.ForceRebuildLayoutImmediate(GamePanel.transform.Find("MyHandCards").GetComponent<RectTransform>());
        DragHandler dragHandler = instance.AddComponent<DragHandler>();
        //TimerManager timerManager = instance.AddComponent<TimerManager>();
        dragHandler.SetGamePanel(MyGameImages,OtherGameImages,this,skillManager);
    }

    public GameObject instanceLargeCard(Card card, Transform position)
    {
        GameObject instance = Instantiate(largeCard, position.transform.position, position.transform.rotation, position.transform);
        CardControl LargeCardControl = instance.GetComponent<CardControl>();
        LargeCardControl.card = card;
        LargeCardControl.display();
        return instance;
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
        //Debug.Log("showRound");
        if (Myplayer_id == gameflag)
        {
            roundPanel.SetActive(true);
            roundPanel.transform.Find("round").GetComponent<TextMeshProUGUI>().text = "你的回合";
            TimerManager.StartTimer(1f,() =>
            {
                roundPanel.SetActive(false);
            });
            setHandcardEnabled();
            GamePanel.transform.Find("giveupBtn").gameObject.SetActive(true);
        }
        else
        {
            roundPanel.SetActive(true);
            roundPanel.transform.Find("round").GetComponent<TextMeshProUGUI>().text = "对方回合";
            TimerManager.StartTimer(1f, () =>
            {
                roundPanel.SetActive(false);
            });
            setHandcardDisabled();
            GamePanel.transform.Find("giveupBtn").gameObject.SetActive(false);
        }
        //showHandCard();
        CalculateAttackNum();
    }
    public void changeGameFlag()
    {
        //Debug.Log(OtherGiveup);
        //Debug.Log("changeGameFlag");
        if (Myplayer_id == gameflag)
        {
            gameflag = Otherplayer_id;
        }
        else
        {
            gameflag = Myplayer_id;
        }
        showRound();
    }

    //计算显示所有数值
    public void CalculateAttackNum()    
    {
        //int MyhandCards = GamePanel.transform.Find("MyHandCards").childCount;
        //();
        setMyHandCardNum(MyHandCardList.Count);
        setOtherHandCardNum(OtherHandCardList.Count);
        //Debug.Log("对手手牌" + OtherHandCardList.Count);
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

        //处理弃牌
        foreach (Transform child in MyGameImages.transform.Find("MyDiscard").transform)
        {
            Destroy(child.gameObject);
        }
        if (MyDiscardList.Count > 0)
        {
            Card card = MyDiscardList[MyDiscardList.Count - 1];
            instanceCard(card, MyGameImages.transform.Find("MyDiscard").transform);
        }

        foreach (Transform child in OtherGameImages.transform.Find("OtherDiscard").transform)
        {
            Destroy(child.gameObject);
        }
        if (OtherDiscardList.Count > 0)
        {
            Card card = OtherDiscardList[OtherDiscardList.Count - 1];
            instanceCard(card, OtherGameImages.transform.Find("OtherDiscard").transform);
        }

    }
    public void setHandcardDisabled()
    {
        foreach (Transform child in GamePanel.transform.Find("MyHandCards").transform)
        {
            child.GetComponent<DragHandler>().enabled = false;
            ImageManager.setDullColor(child.Find("mask/Image").GetComponent<UnityEngine.UI.Image>());
        }
    }
    public void setHandcardEnabled()
    {
        Debug.Log("setHandcardEnabled");
        foreach (Transform child in GamePanel.transform.Find("MyHandCards").transform)
        {
            child.GetComponent<DragHandler>().enabled = true;
            ImageManager.setBrightColor(child.Find("mask/Image").GetComponent<UnityEngine.UI.Image>());
        }
    }
   
    //查找场上最强的牌
    public List<GameObject> FindLargestCard()
    {
        List<GameObject> largestCardList = new List<GameObject>();
        int largestNum = 0;

        Transform[] attackLists = {
        MyGameImages.transform.Find("closeAttackList"),
        MyGameImages.transform.Find("remoteAttackList"),
        MyGameImages.transform.Find("cityAttackList"),
        OtherGameImages.transform.Find("otherCloseAttackList"),
        OtherGameImages.transform.Find("otherRemoteAttackList"),
        OtherGameImages.transform.Find("otherCityAttackList")
        };

        foreach (Transform attackList in attackLists)
        {
            foreach (Transform child in attackList.transform)
            {
                MiniCardControl miniCardControl = child.GetComponent<MiniCardControl>();
                if (miniCardControl != null)
                {
                    Card card = miniCardControl.card;
                    if (card is BasicCard && card.skill1 != 1 && miniCardControl.attackNum > largestNum)
                    {
                        largestNum = miniCardControl.attackNum;
                        largestCardList.Clear();
                        largestCardList.Add(child.gameObject);
                    }
                    else if (card is BasicCard && card.skill1 != 1 && miniCardControl.attackNum == largestNum)
                    {
                        largestCardList.Add(child.gameObject);
                    }
                }
            }
        }

        return largestCardList;
    }


    public void onGiveupBtn()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GiveupEventCode, Myplayer_id, raiseEventOptions, SendOptions.SendReliable);
        GamePanel.transform.Find("giveupBtn").gameObject.SetActive(false);
        setHandcardDisabled();
    }

    public void allGiveUp()
    {
        if(!MyGiveup||!OtherGiveup) return;

        MyGiveup = false;
        OtherGiveup = false;

        int myAttack = Convert.ToInt32(MyGameImages.transform.Find("AllNum/AllAttackNum").GetComponent<TextMeshProUGUI>().text);
        int otherAttack = Convert.ToInt32(OtherGameImages.transform.Find("AllNum/AllAttackNum").GetComponent<TextMeshProUGUI>().text);

        GamePanel.transform.Find("PartGameWin").gameObject.SetActive(true);
        if (myAttack == otherAttack)
        {
            if (MygameDeck.deck_camp == "nilfgaardianDeck" && OtherGameDeck.deck_camp == "nilfgaardianDeck")
            {
                GamePanel.transform.Find("PartGameWin/winer").GetComponent<TextMeshProUGUI>().text = "平局";
            }
            else if (MygameDeck.deck_camp == "nilfgaardianDeck")
            {
                setBlood(0);
                GamePanel.transform.Find("PartGameWin/winer").GetComponent<TextMeshProUGUI>().text = "你赢了这一小局";
            }
            else if (OtherGameDeck.deck_camp == "nilfgaardianDeck")
            {
                setBlood(1);
                GamePanel.transform.Find("PartGameWin/winer").GetComponent<TextMeshProUGUI>().text = "你输了这一小局";
            }
            else
            {
                //Debug.Log(4);
                //Debug.Log(MygameDeck.deck_camp);
                //Debug.Log(OtherGameDeck.deck_camp);
                GamePanel.transform.Find("PartGameWin/winer").GetComponent<TextMeshProUGUI>().text = "平局";
            }
        }
        else if (myAttack < otherAttack)
        {
            setBlood(1);
            GamePanel.transform.Find("PartGameWin/winer").GetComponent<TextMeshProUGUI>().text = "你输了这一小局";
        }
        else
        {
            setBlood(0);
            GamePanel.transform.Find("PartGameWin/winer").GetComponent<TextMeshProUGUI>().text = "你赢了这一小局";
            
            if(MygameDeck.deck_camp == "northernDeck")
            {
                int index = UnityEngine.Random.Range(0, MygameCards.Count);
                MyHandCardList.Add(MygameCards[index]);

                int[] data = new int[2];
                data[0] = Myplayer_id;
                data[1] = MygameCards[index].id;
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(DrawCardEventCode, data, raiseEventOptions, SendOptions.SendReliable);

                MygameCards.RemoveAt(index);
                showHandCard();
            }
        }

        ProcessMyAttackList(MyGameImages.transform.Find("closeAttackList"));
        ProcessMyAttackList(MyGameImages.transform.Find("remoteAttackList"));
        ProcessMyAttackList(MyGameImages.transform.Find("cityAttackList"));
        ProcessMyAttackList(MyGameImages.transform.Find("closeTrump"));
        ProcessMyAttackList(MyGameImages.transform.Find("remoteTrump"));
        ProcessMyAttackList(MyGameImages.transform.Find("cityTrump"));
        ProcessOtherAttackList(OtherGameImages.transform.Find("otherCloseAttackList"));
        ProcessOtherAttackList(OtherGameImages.transform.Find("otherRemoteAttackList"));
        ProcessOtherAttackList(OtherGameImages.transform.Find("otherCityAttackList"));
        ProcessOtherAttackList(OtherGameImages.transform.Find("otherCloseTrump"));
        ProcessOtherAttackList(OtherGameImages.transform.Find("otherRemoteTrump"));
        ProcessOtherAttackList(OtherGameImages.transform.Find("otherCityTrump"));
        foreach (Transform child in GamePanel.transform.Find("weatherCards").GetComponentsInChildren<Transform>())
        {
            MiniCardControl miniCardControl = child.GetComponent<MiniCardControl>();
            if (miniCardControl != null)
            {
                tableCards.Remove(child.gameObject);
                Destroy(child.gameObject);
            }
        }


        if (MygameDeck.deck_camp== "monsterDeck")
        {
            if (MyDiscardList.Count != 0)
            {
                //从MyDiscardList随机取一个,如果不满足（card is BasicCard&& card.camp== "monster"&&card.skill1!=1）则重新抽取
                int index = UnityEngine.Random.Range(0, MyDiscardList.Count);
                while (!(MyDiscardList[index] is BasicCard && MyDiscardList[index].camp == "monster" && MyDiscardList[index].skill1 != 1))
                {
                    index = UnityEngine.Random.Range(0, MyDiscardList.Count);
                }
                Card card = MyDiscardList[index];
                MyDiscardList.RemoveAt(index);

                int position = 0;
                if (card.position == "close combat")
                {
                    position = 3;
                }
                if (card.position == "remote attack")
                {
                    position = 2;
                }
                if (card.position == "city attack")
                {
                    position = 1;
                }
                if (card.position == "remote attack,close combat")
                {
                    position = 3;
                }

                int[] data = new int[4];
                data[0] = Myplayer_id;
                data[1] = card.id;
                data[2] = position;
                data[3] = 0;  //1表示由玩家放置
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(FightManager.PutCardsEventCode, data, raiseEventOptions, SendOptions.SendReliable);

                GameObject instance = instanceCard(card, skillManager.GetListByPosition(position));
                tableCards.Add(instance);
            }
        }

        
        TimerManager.StartTimer(2f, () =>
        {
            GamePanel.transform.Find("PartGameWin").gameObject.SetActive(false);
            OtherGameImages.transform.Find("OtherGiveup").gameObject.SetActive(false);
        });
        if (!gameover)
        {
            TimerManager.StartTimer(2.5f, changeGameFlag);
        }
    }

    public bool setBlood(int win)  //0:我赢了 1:我输了
    {
        if (win==0)
        {
            if(OtherBlood == 2)
            {
                OtherGameImages.transform.Find("blood/2").gameObject.SetActive(false);
                OtherBlood = 1;
            }
            else if (OtherBlood == 1)
            {
                OtherGameImages.transform.Find("blood/1").gameObject.SetActive(false);
                OtherBlood = 0;

                gameOver(0);
                return true;
            }
        }
        else
        {
            if (myBlood == 2)
            {
                MyGameImages.transform.Find("blood/2").gameObject.SetActive(false);
                myBlood = 1;
            }
            else if (myBlood == 1)
            {
                MyGameImages.transform.Find("blood/1").gameObject.SetActive(false);
                myBlood = 0;
                gameOver(1);
                return true;
            }
        }
        return false;
    }

    void ProcessMyAttackList(Transform attackList)
    {
        foreach (Transform child in attackList.GetComponentsInChildren<Transform>())
        {
            MiniCardControl miniCardControl = child.GetComponent<MiniCardControl>();
            if (miniCardControl != null)
            {
                MyDiscardList.Add(miniCardControl.card);
                tableCards.Remove(child.gameObject);
                Destroy(child.gameObject);
            }
        }
    }
    void ProcessOtherAttackList(Transform attackList)
    {
        foreach (Transform child in attackList.GetComponentsInChildren<Transform>())
        {
            MiniCardControl miniCardControl = child.GetComponent<MiniCardControl>();
            if (miniCardControl != null)
            {
                OtherDiscardList.Add(miniCardControl.card);
                tableCards.Remove(child.gameObject);
                Destroy(child.gameObject);
            }
        }
    }


    public void gameOver(int win)  //0:我赢了 1:我输了
    {
        gameover = true;
        TimerManager.StartTimer(2.5f, () =>
        {
            GameOver.SetActive(true);
            if (win == 0)
            {
                GameOver.transform.Find("win").GetComponent<TextMeshProUGUI>().text = "你赢了";
                Texture2D texture = Resources.Load<Texture2D>("PostGame/victory");
                Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                GameOver.transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = sprite;
            }
            else
            {
                GameOver.transform.Find("win").GetComponent<TextMeshProUGUI>().text = "你输了";
                Texture2D texture = Resources.Load<Texture2D>("PostGame/defeat");
                Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                GameOver.transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = sprite;
            }
        });
        
    }

    public void onExitBtn()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Lobby");
    }
}

