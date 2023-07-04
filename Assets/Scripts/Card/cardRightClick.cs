using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class cardRightClick : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public GameObject BigImage;
    public GameObject BigEffect;
    public GameObject bigImage;
    public GameObject bigEffect;
    public GameObject Canvas;
    private bool isRightMouseDown = false;
    private Card card;

    private void Start()
    {
        Canvas = GameObject.Find("Canvas");
    }
    private void OnDestroy()
    {
        DestroyEffect();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CardControl c = GetComponent<CardControl>();
        if (c == null)
        {
            card = GetComponent<MiniCardControl>().card;
        }
        else if (c != null)
        {
            card = c.card;
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            isRightMouseDown = true;
            bigImage = Instantiate(BigImage);
            bigEffect = Instantiate(BigEffect);
            bigImage.GetComponent<CardControl>().card = card;
            bigEffect.transform.SetParent(Canvas.transform, false);
            bigImage.transform.SetParent(Canvas.transform, false);
            bigImage.GetComponent<CardControl>().display();
            string info = card.name + "\n";
            int cardskill = card.skill1;
            if (cardskill == 1 && card.skill2 != -1){
                cardskill = card.skill2;
            }
            switch (cardskill)
            {
                case -1:
                    info += "无技能";
                    break;
                case 1:
                    info += "英雄：不会被任何特别牌或能力影响";
                    break;
                case 2:
                    info += "同胞之情：放在同名牌的旁边，让两张牌力量都变为双倍";
                    break;
                case 3:
                    info += "间谍：放在对方的战场上（算在对手的总分中），并可从你自己的牌组中再抽2张牌";
                    break;
                case 4:
                    info += "提振士气：所有同一列单位获得+1（不含本身）";
                    break;
                case 5:
                    info += "医生：在废牌堆中任选一张单位牌（不能选英雄牌或特别牌），马上打出";
                    break;
                case 6:
                    info += "霜霰：将双方玩家所有近战作战牌的力量将为1";
                    break;
                case 7:
                    info += "浓雾：将双方玩家所有远程作战牌的力量将为1";
                    break;
                case 8:
                    info += "地形雨：将双方玩家所有攻城作战牌的力量将为1";
                    break;
                case 9:
                    info += "天晴：移除所有天气牌（霜霰浓雾和地形雨）效果";
                    break;
                case 10:
                    info += "烧灼：使用后作废，杀死战场上最强的牌";
                    break;
                case 11:
                    info += "领导号角：该列所有单位牌力量加倍。一列只能使用一张";
                    break;
                case 12:
                    info += "诱饵：跟战场上的一张牌互换，使牌回到手牌里";
                    break;
                case 13:
                    info += "烧灼-近战：若敌人所有近战作战单位的力量总和等于或大于10，则摧毁敌人最强的近战作战单位";
                    break;
                case 14:
                    info += "集合：在牌组中找到所有同名的牌，马上打出";
                    break;
                case 15:
                    info += "敏捷：可置放在近战列或远程列，置放后即不得移动";
                    break;
            }
            bigEffect.transform.Find("text").GetComponent<TextMeshProUGUI>().text = info;


        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) // Right mouse button release
        {
 
            if (isRightMouseDown)
            {
                isRightMouseDown = false;
                DestroyEffect();
            }
        }
    }

    public void DestroyEffect()
    {
        Destroy(bigImage);
        Destroy(bigEffect);
    }

}
