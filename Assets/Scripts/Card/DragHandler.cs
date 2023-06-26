using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform cardRectTransform;
    private CanvasGroup canvasGroup;
    private Transform initialParent;
    private Vector2 initialPosition;
    private Vector2 offset;

    public GameObject MyGameImages;
    public GameObject OtherGameImages;
    public FightManager fightManager;

    Card card;

    private void Awake()
    {
        cardRectTransform = GetComponent<RectTransform>();

        card = GetComponent<MiniCardControl>().card;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
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
            if(card.position== "close combat")
            {
                highlightMyCloseAttacklist();
            }
            if(card.position== "remote attack")
            {
                highlightMyRemoteAttackList();
            }
            if(card.position== "city attack")
            {
                highlightMyCityAttackList();
            }
            if(card.position== "remote attack,close combat")
            {
                highlightMyRemoteAttackList();
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
        else if(card is SpecialCard && card.position == "weather")  //天气
        {
            highlightWeather();
        }
        else if(card is SpecialCard && (card.skill1 == 11 || card.skill2 == 11))  //领导号角
        {
            highlightMyCloseTrump();
            highlightMyRemoteTrump();
            highlightMyCityTrump();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 更新卡牌位置为鼠标位置加上偏移量
        cardRectTransform.position = eventData.position - offset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        reduceAll();

        // 使用RaycastAll获取鼠标释放位置下的所有UI对象
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // 遍历查找Horizontal Layout Group
        HorizontalLayoutGroup targetLayoutGroup = null;

        for (int i = 0; i < results.Count; i++)
        {
            HorizontalLayoutGroup layoutGroup = results[i].gameObject.GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup != null)
            {
                //targetLayoutGroup = layoutGroup;
                if (card is BasicCard && card.skill1 != 3 && card.skill2 != 3 && card.skill1 != 11 && card.skill2 != 11)  //基本牌
                {
                    if (card.position == "close combat")
                    {
                        if (results[i].gameObject.name == "closeAttackList")
                        {

                        }
                    }
                    if (card.position == "remote attack")
                    {
                        
                    }
                    if (card.position == "city attack")
                    {
                        
                    }
                    if (card.position == "remote attack,close combat")
                    {
                        
                    }
                }
                else if (card is BasicCard && (card.skill1 == 3 || card.skill2 == 3))  //间谍
                {

                }
                else if (card is SpecialCard && card.position == "weather")  //天气
                {

                }
                else if (card is SpecialCard && (card.skill1 == 11 || card.skill2 == 11))  //领导号角
                {

                }
                break;
            }
        }

        if (targetLayoutGroup != null)
        {
            // 将卡牌的父级Transform设置为目标Horizontal Layout Group的Transform
            transform.SetParent(targetLayoutGroup.transform, false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(targetLayoutGroup.GetComponent<RectTransform>());
        }
        else
        {
            // 如果没有找到Horizontal Layout Group，将卡牌的父级Transform设置为初始父级Transform，并将位置重置为初始位置
            transform.SetParent(initialParent, false);
            cardRectTransform.anchoredPosition = initialPosition;
            LayoutRebuilder.ForceRebuildLayoutImmediate(initialParent.GetComponent<RectTransform>());
        }



        // 在此处执行卡牌释放的逻辑
        // ...
        
        

        fightManager.CalculateAttackNum();
        //Destroy(GetComponent<DragHandler>());
    }


    public void SetGamePanel(GameObject myPanel,GameObject otherPanel,FightManager fightManager)
    {
        MyGameImages = myPanel;
        OtherGameImages = otherPanel;
        this.fightManager = fightManager;
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
