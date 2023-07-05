using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class DragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler,IPointerExitHandler,IPointerEnterHandler
{
    private RectTransform cardRectTransform;
    private Transform initialParent;
    private Vector2 initialPosition;
    private Vector2 offset;

    public GameObject MyGameImages;
    public GameObject OtherGameImages;
    public FightManager fightManager;
    public SkillManager skillManager;

    Card card;
    int position; // 123 456 从下到上


    private Vector3 raisedPosition = new Vector3(0f, 10f, 0f); // 抬高的位置
    private bool isHovering = false;


    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

    private void Awake()
    {
        cardRectTransform = GetComponent<RectTransform>();

        card = GetComponent<MiniCardControl>().card;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log(1);
        if (!isHovering)
        {
            transform.position += raisedPosition;
            isHovering = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log(2);
        if (isHovering)
        {
            transform.position -= raisedPosition;
            isHovering = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) return;
        // 记录鼠标点击位置与卡牌初始位置的偏移量
        offset = eventData.position - (Vector2)cardRectTransform.position;

        // 将卡牌置于顶层（确保拖拽时卡牌位于其他UI元素之上）
        cardRectTransform.SetAsLastSibling();

        // 保存卡牌的初始父级Transform和位置
        initialParent = transform.parent;
        initialPosition = cardRectTransform.anchoredPosition;

        // 可选：禁用交互以防止干扰其他UI元素
        // canvasGroup.blocksRaycasts = false;
        
        if(card is BasicCard && card.skill1 != 3 && card.skill2 != 3 && card.skill1 != 11 && card.skill2 != 11)  //基本牌
        {
            //Debug.Log(card.position);
            if(card.position== "close combat")
            {
                highlightMyCloseAttacklist();
            }
            if(card.position== "remote attack")
            {
                //Debug.Log("remote attack");
                highlightMyRemoteAttackList();
            }
            if(card.position== "city attack")
            {
                highlightMyCityAttackList();
            }
            if(card.position== "remote attack,close combat")
            {
                highlightMyCloseAttacklist();
                highlightMyRemoteAttackList();
            }
        }
        else if(card is BasicCard && (card.skill1 == 3 || card.skill2 == 3))  //间谍
        {
            if (card.position == "close combat")
            {
                hightlightOtherCloseAttacklist();
            }
            if (card.position == "remote attack")
            {
                hightlightOtherRemoteAttackList();
            }
            if (card.position == "city attack")
            {
                hightlightOtherCityAttackList();
            }
        }
        else if(card is SpecialCard && card.position == "weather" && card.skill1!=9)  //天气
        {
            highlightWeather();
        }
        else if(card is SpecialCard && (card.skill1 == 11 || card.skill2 == 11))  //领导号角
        {
            highlightMyCloseTrump();
            highlightMyRemoteTrump();
            highlightMyCityTrump();
        }
        else if (card is BasicCard && card.skill1 == 11)
        {
            highlightMyCloseAttacklist();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) return;
        // 更新卡牌位置为鼠标位置加上偏移量
        cardRectTransform.position = eventData.position - offset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) return;
        reduceAll();

        // 使用RaycastAll获取鼠标释放位置下的所有UI对象
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        //可放置牌
        HorizontalLayoutGroup targetLayoutGroup = null;
        for (int i = 0; i < results.Count; i++)
        {
            HorizontalLayoutGroup layoutGroup = results[i].gameObject.GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup != null)
            {
                //基本牌
                if (card is BasicCard && card.skill1 != 3 && card.skill2 != 3 && card.skill1 != 11 && card.skill2 != 11)  
                {
                    if (card.position == "close combat")
                    {
                        if (results[i].gameObject.name == "closeAttackList")
                        {
                            targetLayoutGroup = layoutGroup;
                            position = 3;

                            //skillManager.SameBrotherhood(card, position);
                            //skillManager.BoostMorale(card, position);
                            //int[] data= new int[3];
                            //data[0] = fightManager.Myplayer_id;
                            //data[1] = card.id;
                            //data[2] = 4;
                            //PhotonNetwork.RaiseEvent(FightManager.SameBrotherEventCode, data, raiseEventOptions, SendOptions.SendReliable);
                        }
                    }
                    if (card.position == "remote attack")
                    {
                        if (results[i].gameObject.name == "remoteAttackList")
                        {
                            targetLayoutGroup = layoutGroup;
                            position = 2;

                            //skillManager.SameBrotherhood(card, position);
                            //skillManager.BoostMorale(card, position);
                            //int[] data = new int[3];
                            //data[0] = fightManager.Myplayer_id;
                            //data[1] = card.id;
                            //data[2] = 5;
                            //PhotonNetwork.RaiseEvent(FightManager.SameBrotherEventCode, data, raiseEventOptions, SendOptions.SendReliable);
                        }
                    }
                    if (card.position == "city attack")
                    {
                        if (results[i].gameObject.name == "cityAttackList")
                        {
                            targetLayoutGroup = layoutGroup;
                            position = 1;

                            //skillManager.SameBrotherhood(card, position);
                            //skillManager.BoostMorale(card, position);
                            //int[] data = new int[3];
                            //data[0] = fightManager.Myplayer_id;
                            //data[1] = card.id;
                            //data[2] = 6;
                            //PhotonNetwork.RaiseEvent(FightManager.SameBrotherEventCode, data, raiseEventOptions, SendOptions.SendReliable);
                        }
                    }
                    if (card.position == "remote attack,close combat")
                    {
                        if (results[i].gameObject.name == "closeAttackList" )
                        {
                            targetLayoutGroup = layoutGroup;
                            position = 3;
                        }
                        if (results[i].gameObject.name == "remoteAttackList")
                        {
                            targetLayoutGroup = layoutGroup;
                            position = 2;
                        }
                    }
                }
                //间谍
                else if (card is BasicCard && (card.skill1 == 3 || card.skill2 == 3))  
                {
                    if (card.position == "close combat")
                    {
                        if (results[i].gameObject.name == "otherCloseAttackList")
                        {
                            targetLayoutGroup = layoutGroup;
                            position = 4;
                        }
                    }
                    if (card.position == "remote attack")
                    {
                        if (results[i].gameObject.name == "otherRemoteAttackList")
                        {
                            targetLayoutGroup = layoutGroup;
                            position = 5;
                        }
                    }
                    if (card.position == "city attack")
                    {
                        if (results[i].gameObject.name == "otherCityAttackList")
                        {
                            targetLayoutGroup = layoutGroup;
                            position = 6;
                        }
                    }

                }
                //天气
                else if (card is SpecialCard && card.position == "weather")  
                {
                    if (results[i].gameObject.name == "weatherCards")
                    {
                        targetLayoutGroup = layoutGroup;
                        position = 10;
                    }
                }
                //领导号角
                else if (card is SpecialCard && (card.skill1 == 11 || card.skill2 == 11))  
                {
                    if (results[i].gameObject.name == "closeTrump")
                    {
                        targetLayoutGroup = layoutGroup;
                        position = 9;
                    }
                    if (results[i].gameObject.name == "remoteTrump")
                    {
                        targetLayoutGroup = layoutGroup;
                        position = 8;
                    }
                    if (results[i].gameObject.name == "cityTrump")
                    {
                        targetLayoutGroup = layoutGroup;
                        position = 7;
                    }
                }
                //丹德里恩
                else if (card is BasicCard &&card.skill1 == 11)  
                {
                    if (card.position == "close combat")
                    {
                        if (results[i].gameObject.name == "closeAttackList")
                        {
                            targetLayoutGroup = layoutGroup;
                            position = 3;
                        }
                    }
                }
                
                break;
            }
        }

        //诱饵
        if (card.skill1 == 12)
        {
            //Debug.Log("诱饵");
            for (int j = 0; j < results.Count; j++)
            {
                MiniCardControl miniCardControl = results[j].gameObject.GetComponent<MiniCardControl>();
                DragHandler dragHandler = results[j].gameObject.GetComponent<DragHandler>();
                if (miniCardControl != null && dragHandler == null && miniCardControl.card.skill1 != 1)  //在场上的牌且不是英雄牌
                {
                    fightManager.MyHandCardList.Add(miniCardControl.card);
                    //fightManager.MyHandCardList.Remove(card);
                    //fightManager.MyDiscardList.Add(card);
                    //Destroy(this.gameObject);
                    //fightManager.showHandCard();

                    int[] data1 = new int[3];
                    data1[0] = fightManager.Myplayer_id;
                    data1[1] = 1;   //1表示诱饵
                    data1[2] = miniCardControl.card.id;
                    PhotonNetwork.RaiseEvent(FightManager.RemoveCardEventCode,data1, raiseEventOptions, SendOptions.SendReliable);
                    //Debug.Log(results[j].gameObject.GetInstanceID());
                    //PhotonNetwork.Destroy(results[j].gameObject);

                    int[] data = new int[2];
                    data[0] = fightManager.Myplayer_id;
                    data[1] = card.id;
                    PhotonNetwork.RaiseEvent(FightManager.PutSkillCardsEventCode, data, raiseEventOptions, SendOptions.SendReliable);

                    Debug.Log(miniCardControl.attackNum);
                    Destroy(this.gameObject);
                    //Debug.Log("dragdis");
                    fightManager.showHandCard();
                    fightManager.CalculateAttackNum();

                    PhotonNetwork.RaiseEvent(FightManager.ClearAttackEventCode, 1, raiseEventOptions, SendOptions.SendReliable);
                    return;
                }
            }
        }
        //天晴
        if(card.skill1==9)
        {
            bool flag = true;
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.name == "MyHandCards")
                {
                    flag = false;
                }
            }
            if (flag == true)
            {
                int[] data=new int[2];
                data[0] = fightManager.Myplayer_id;
                data[1] = card.id;
                PhotonNetwork.RaiseEvent(FightManager.PutSkillCardsEventCode, data, raiseEventOptions, SendOptions.SendReliable);
                return;
            }
        }
        //灼烧
        if(card .skill1==10||card.skill2==10)
        {
            bool flag = true;
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.name == "MyHandCards")
                {
                    flag = false;
                }
            }
            if (flag == true)
            {
                int[] data = new int[2];
                data[0] = fightManager.Myplayer_id;
                data[1] = card.id;
                PhotonNetwork.RaiseEvent(FightManager.PutSkillCardsEventCode, data, raiseEventOptions, SendOptions.SendReliable);
                return;
            }
        }


        if (targetLayoutGroup != null)
        {

            // 将卡牌的父级Transform设置为目标Horizontal Layout Group的Transform
            transform.SetParent(targetLayoutGroup.transform, false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(targetLayoutGroup.GetComponent<RectTransform>());

            int[] data=new int[4];
            data[0] = fightManager.Myplayer_id;
            data[1] = card.id;
            data[2] = position;
            data[3] = 1;  //1表示由玩家放置
            
            PhotonNetwork.RaiseEvent(FightManager.PutCardsEventCode, data, raiseEventOptions, SendOptions.SendReliable);

            //fightManager.MyHandCardList.Remove(card);
            fightManager.tableCards.Add(this.gameObject);
            //this.AddComponent<Image>();

            if (card is BasicCard && (card.skill1 == 3 || card.skill2 == 3))
            {
                //从MygameCards中抽2张牌
                for (int j = 0; j < 2; j++)
                {
                    int index = UnityEngine.Random.Range(0, fightManager.MygameCards.Count);
                    fightManager.MyHandCardList.Add(fightManager.MygameCards[index]);

                    int[] data1 = new int[2];
                    data1[0] = fightManager.Myplayer_id;
                    data1[1] = fightManager.MygameCards[index].id;
                    PhotonNetwork.RaiseEvent(FightManager.DrawCardEventCode, data1, raiseEventOptions, SendOptions.SendReliable);

                    fightManager.MygameCards.RemoveAt(index);
                }
            }

            fightManager.CalculateAttackNum();
            Destroy(GetComponent<DragHandler>());
        }
        else
        {
            // 如果没有找到Horizontal Layout Group，将卡牌的父级Transform设置为初始父级Transform，并将位置重置为初始位置
            transform.SetParent(initialParent, false);
            cardRectTransform.anchoredPosition = initialPosition;
            LayoutRebuilder.ForceRebuildLayoutImmediate(initialParent.GetComponent<RectTransform>());
        }

        fightManager.showHandCard();
        fightManager.CalculateAttackNum();
    }


    public void SetGamePanel(GameObject myPanel,GameObject otherPanel,FightManager fightManager,SkillManager skillManager)
    {
        MyGameImages = myPanel;
        OtherGameImages = otherPanel;
        this.fightManager = fightManager;
        this.skillManager = skillManager;
    }




    public void highlightMyCloseAttacklist()
    {
        MyGameImages.transform.Find("closeAttackList").GetComponent<Image>().color = new Color(255, 255, 255, 0.45f);
    }
    public void reducelightMyCloseAttacklist()
    {
        MyGameImages.transform.Find("closeAttackList").GetComponent<Image>().color = new Color(255, 255, 255, 0);
    }
    public void highlightMyRemoteAttackList()
    {
        MyGameImages.transform.Find("remoteAttackList").GetComponent<Image>().color = new Color(255, 255, 255, 0.45f);
    }
    public void reducelightMyRemoteAttackList()
    {
        MyGameImages.transform.Find("remoteAttackList").GetComponent<Image>().color = new Color(255, 255, 255, 0);
    }
    public void highlightMyCityAttackList()
    {
        MyGameImages.transform.Find("cityAttackList").GetComponent<Image>().color = new Color(255, 255, 255, 0.45f);
    }
    public void reducelightMyCityAttackList()
    {
        MyGameImages.transform.Find("cityAttackList").GetComponent<Image>().color = new Color(255, 255, 255, 0);
    }
    public void highlightMyCloseTrump()
    {
        MyGameImages.transform.Find("closeTrump").GetComponent<Image>().color = new Color(255, 255, 255, 0.45f);
    }
    public void reducelightMyCloseTrump()
    {
        MyGameImages.transform.Find("closeTrump").GetComponent<Image>().color = new Color(255, 255, 255, 0);
    }
    public void highlightMyRemoteTrump()
    {
        MyGameImages.transform.Find("remoteTrump").GetComponent<Image>().color = new Color(255, 255, 255, 0.45f);
    }
    public void reducelightMyRemoteTrump()
    {
        MyGameImages.transform.Find("remoteTrump").GetComponent<Image>().color = new Color(255, 255, 255, 0);
    }
    public void highlightMyCityTrump()
    {
        MyGameImages.transform.Find("cityTrump").GetComponent<Image>().color = new Color(255, 255, 255, 0.45f);
    }
    public void reducelightMyCityTrump()
    {
        MyGameImages.transform.Find("cityTrump").GetComponent<Image>().color = new Color(255, 255, 255, 0);
    }
    public void hightlightOtherCloseAttacklist()
    {
        OtherGameImages.transform.Find("otherCloseAttackList").GetComponent<Image>().color = new Color(255, 255, 255, 0.45f);
    }
    public void reducelightOtherCloseAttacklist()
    {
        OtherGameImages.transform.Find("otherCloseAttackList").GetComponent<Image>().color = new Color(255, 255, 255, 0);
    }
    public void hightlightOtherRemoteAttackList()
    {
        OtherGameImages.transform.Find("otherRemoteAttackList").GetComponent<Image>().color = new Color(255, 255, 255, 0.45f);
    }
    public void reducelightOtherRemoteAttackList()
    {
        OtherGameImages.transform.Find("otherRemoteAttackList").GetComponent<Image>().color = new Color(255, 255, 255, 0);
    }
    public void hightlightOtherCityAttackList()
    {
        OtherGameImages.transform.Find("otherCityAttackList").GetComponent<Image>().color = new Color(255, 255, 255, 0.45f);
    }
    public void reducelightOtherCityAttackList()
    {
        OtherGameImages.transform.Find("otherCityAttackList").GetComponent<Image>().color = new Color(255, 255, 255, 0);
    }
    public void highlightWeather()
    {
        fightManager.GamePanel.transform.Find("weatherCards").GetComponent<Image>().color = new Color(255, 255, 255, 0.45f);
    }
    public void reducelightWeather()
    {
        fightManager.GamePanel.transform.Find("weatherCards").GetComponent<Image>().color = new Color(255, 255, 255, 0);
    }
    public void reduceAll()
    {
        reducelightMyCloseAttacklist();
        reducelightMyRemoteAttackList();
        reducelightMyCityAttackList();
        reducelightMyCloseTrump();
        reducelightMyRemoteTrump();
        reducelightMyCityTrump();
        reducelightOtherCloseAttacklist();
        reducelightOtherRemoteAttackList();
        reducelightOtherCityAttackList();
        reducelightWeather();
    }


    
}
