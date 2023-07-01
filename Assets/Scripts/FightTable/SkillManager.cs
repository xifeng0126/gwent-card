using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillManager : MonoBehaviour
{
    FightManager fightManager;
    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
    // Start is called before the first frame update
    void Start()
    {
        fightManager = GetComponent<FightManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //SameBrotherhood();
    }

    public void setAttack()
    {
        clearAttack();

        Weather();
        BoostMorale();  //4
        SameBrotherhood();  //2
        LeadershipHorn(); //11
    }

    public void clearAttack()
    {
        //Debug.Log("clear");
        clearEach(GetAttackListByPosition(1));
        clearEach(GetAttackListByPosition(2));
        clearEach(GetAttackListByPosition(3));
        clearEach(GetAttackListByPosition(4));
        clearEach(GetAttackListByPosition(5));
        clearEach(GetAttackListByPosition(6));
    }
    public void clearEach(Transform[] AttackList)
    {
        foreach (Transform child in AttackList)
        {
            MiniCardControl miniCardControl = child.GetComponent<MiniCardControl>();
            if (miniCardControl != null)
            {
                miniCardControl.setAttack();
                miniCardControl.display();
            }
        }
    }

    //2:同胞之情：放在同名牌的旁边，让两张牌力量都变为双倍
    public void SameBrotherhood()
    {
        SameBrotherhoodEach(GetAttackListByPosition(1));
        SameBrotherhoodEach(GetAttackListByPosition(2));
        SameBrotherhoodEach(GetAttackListByPosition(3));
        SameBrotherhoodEach(GetAttackListByPosition(4));
        SameBrotherhoodEach(GetAttackListByPosition(5));
        SameBrotherhoodEach(GetAttackListByPosition(6));

    }
    public void SameBrotherhoodEach(Transform[] AttackList)
    {
        foreach (Transform child in AttackList)
        {
            MiniCardControl miniCardControl = child.GetComponent<MiniCardControl>();
            if (miniCardControl != null && (miniCardControl.card.skill1 == 2 || miniCardControl.card.skill2 == 2))
            {
                int id = miniCardControl.card.id;
                foreach (Transform c in AttackList)
                {
                    MiniCardControl CardControl = c.GetComponent<MiniCardControl>();
                    if (CardControl != null && CardControl.card.id == id && CardControl != miniCardControl)
                    {

                        CardControl.setAttack(CardControl.attackNum * 2);
                        CardControl.displayRed();
                    }
                }
            }
        }
    }

    //4:提振士气
    public void BoostMorale()
    {
        BoostMoraleEach(GetAttackListByPosition(1));
        BoostMoraleEach(GetAttackListByPosition(2));
        BoostMoraleEach(GetAttackListByPosition(3));
        BoostMoraleEach(GetAttackListByPosition(4));
        BoostMoraleEach(GetAttackListByPosition(5));
        BoostMoraleEach(GetAttackListByPosition(6));
    }
    public void BoostMoraleEach(Transform[] AttackList)
    {
        foreach (Transform child in AttackList)
        {
            MiniCardControl miniCardControl = child.GetComponent<MiniCardControl>();
            if (miniCardControl != null && (miniCardControl.card.skill1 == 4 || miniCardControl.card.skill2 == 4))
            {
                foreach (Transform c in AttackList)
                {
                    MiniCardControl CardControl = c.GetComponent<MiniCardControl>();
                    if (CardControl != null && CardControl != miniCardControl && CardControl.card.skill1 != 1)
                    {
                        CardControl.setAttack(CardControl.attackNum + 1);
                        CardControl.displayRed();
                    }
                }
            }
        }
    }


    public void Weather()
    {
        Transform[] weather=fightManager.GamePanel.transform.Find("weatherCards").GetComponentsInChildren<Transform>();
        foreach (Transform child in weather)
        {
            MiniCardControl miniCardControl = child.GetComponent<MiniCardControl>();
            if(miniCardControl != null)
            {
                if (miniCardControl.card.skill1 == 6) //霜霰
                {
                    weatherEach(GetAttackListByPosition(3));
                    weatherEach(GetAttackListByPosition(4));
                }
                if (miniCardControl.card.skill1 == 7) //浓雾
                {
                    weatherEach(GetAttackListByPosition(2));
                    weatherEach(GetAttackListByPosition(5));
                }
                if (miniCardControl.card.skill1 == 8) //地形雨
                {
                    weatherEach(GetAttackListByPosition(1));
                    weatherEach(GetAttackListByPosition(6));
                }
            }
        }
    }

    public void weatherEach(Transform[] AttackList)
    {
        foreach (Transform child in AttackList)
        {
            MiniCardControl miniCardControl = child.GetComponent<MiniCardControl>();
            if (miniCardControl != null && miniCardControl.card.skill1 != 1 )
            {
                miniCardControl.setAttack(1);
                miniCardControl.displayRed();
            }
        }
    }
    
    //11:领导号角
    public void LeadershipHorn()
    {
        LeadershipHornEach(GetAttackListByPosition(3));
        LeadershipHornEach(GetAttackListByPosition(4));

        Transform[] position7= GetAttackListByPosition(7);
        foreach (Transform child in position7)
        {
            MiniCardControl miniCardControl = child.GetComponent<MiniCardControl>();
            if (miniCardControl != null && (miniCardControl.card.skill1 == 11 || miniCardControl.card.skill2 == 11))
            {
                LeadershipHornSide(GetAttackListByPosition(1));
            }
        }
        Transform[] position8 = GetAttackListByPosition(8);
        foreach (Transform child in position8)
        {
            MiniCardControl miniCardControl = child.GetComponent<MiniCardControl>();
            if (miniCardControl != null && (miniCardControl.card.skill1 == 11 || miniCardControl.card.skill2 == 11))
            {
                LeadershipHornSide(GetAttackListByPosition(2));
            }
        }
        Transform[] position9 = GetAttackListByPosition(9);
        foreach (Transform child in position9)
        {
            MiniCardControl miniCardControl = child.GetComponent<MiniCardControl>();
            if (miniCardControl != null && (miniCardControl.card.skill1 == 11 || miniCardControl.card.skill2 == 11))
            {
                LeadershipHornSide(GetAttackListByPosition(3));
            }
        }
        Transform[] position11 = GetAttackListByPosition(11);
        foreach (Transform child in position11)
        {
            MiniCardControl miniCardControl = child.GetComponent<MiniCardControl>();
            if (miniCardControl != null && (miniCardControl.card.skill1 == 11 || miniCardControl.card.skill2 == 11))
            {
                LeadershipHornSide(GetAttackListByPosition(4));
            }
        }
        Transform[] position12 = GetAttackListByPosition(12);
        foreach (Transform child in position12)
        {
            MiniCardControl miniCardControl = child.GetComponent<MiniCardControl>();
            if (miniCardControl != null && (miniCardControl.card.skill1 == 11 || miniCardControl.card.skill2 == 11))
            {
                LeadershipHornSide(GetAttackListByPosition(5));
            }
        }
        Transform[] position13 = GetAttackListByPosition(13);
        foreach (Transform child in position13)
        {
            MiniCardControl miniCardControl = child.GetComponent<MiniCardControl>();
            if (miniCardControl != null && (miniCardControl.card.skill1 == 11 || miniCardControl.card.skill2 == 11))
            {
                LeadershipHornSide(GetAttackListByPosition(6));
            }
        }

    }
    public void LeadershipHornEach(Transform[] AttackList)
    {
        foreach (Transform child in AttackList)
        {
            MiniCardControl miniCardControl = child.GetComponent<MiniCardControl>();
            if (miniCardControl != null && (miniCardControl.card.skill1 == 11 || miniCardControl.card.skill2 == 11))
            {
                foreach (Transform c in AttackList)
                {
                    MiniCardControl CardControl = c.GetComponent<MiniCardControl>();
                    if (CardControl != null && CardControl != miniCardControl && CardControl.card.skill1 != 1)
                    {
                        CardControl.setAttack(CardControl.attackNum * 2);
                        CardControl.displayRed();
                    }
                }
            }
        }
    }
    public void LeadershipHornSide(Transform[] AttackList)
    {
        foreach (Transform child in AttackList)
        {
            MiniCardControl miniCardControl = child.GetComponent<MiniCardControl>();
            if (miniCardControl != null && miniCardControl.card.skill1 != 1)
            {
                miniCardControl.setAttack(miniCardControl.attackNum * 2);
                miniCardControl.displayRed();
            }
        }
    }


    //14:集合
    public void Assembly(int player,Card card ,int position)
    {
        if (player == fightManager.Myplayer_id)
        {
            if (card.id == 86 || card.id == 87 || card.id == 93 || card.id == 112 || card.id == 114 || card.id == 116 ) //矮人好斗分子
            {
                searchMy(card.id, position);
            }
            if(card.id == 117 || card.id == 118|| card.id == 119|| card.id == 120)
            {
                searchMy(117, position);
                searchMy(118, position);
                searchMy(119, position);
                searchMy(120, position);
            }
            if (card.id == 126 || card.id == 127 || card.id == 128)
            {
                searchMy(126, position);
                searchMy(127, position);
                searchMy(128, position);
            }
            if(card.id == 140)
            {
                searchMy(116, 3);
            }
        }
        if(player == fightManager.Otherplayer_id)
        {
            if (card.id == 86 || card.id == 87 || card.id == 93 || card.id == 112 || card.id == 114 || card.id == 116) //矮人好斗分子
            {
                searchOther(card.id, position);
            }
            if (card.id == 117 || card.id == 118 || card.id == 119 || card.id == 120)
            {
                searchOther(117, position);
                searchOther(118, position);
                searchOther(119, position);
                searchOther(120, position);
            }
            if (card.id == 126 || card.id == 127 || card.id == 128)
            {
                searchOther(126, position);
                searchOther(127, position);
                searchOther(128, position);
            }
            if (card.id == 140)
            {
                searchOther(116, 4);
            }
        }
        
    }

    public void searchMy(int card_id,int position)
    {
        for (int i = fightManager.MyHandCardList.Count - 1; i >= 0; i--)
        {
            Card ca = fightManager.MyHandCardList[i];
            if (ca.id == card_id)
            {
                int[] data = new int[4];
                data[0] = fightManager.Myplayer_id;
                data[1] = ca.id;
                data[2] = position;
                data[3] = 0;  //1表示由玩家放置
                PhotonNetwork.RaiseEvent(FightManager.PutCardsEventCode, data, raiseEventOptions, SendOptions.SendReliable);

                GameObject instance = fightManager.instanceCard(ca, GetListByPosition(position));
                fightManager.tableCards.Add(instance);

                fightManager.MyHandCardList.RemoveAt(i);
            }
        }

        for (int i = fightManager.MygameCards.Count - 1; i >= 0; i--)
        {
            Card ca = fightManager.MygameCards[i];
            if (ca.id == card_id)
            {
                int[] data = new int[4];
                data[0] = fightManager.Myplayer_id;
                data[1] = ca.id;
                data[2] = position;
                data[3] = 0;  //1表示由玩家放置
                PhotonNetwork.RaiseEvent(FightManager.PutCardsEventCode, data, raiseEventOptions, SendOptions.SendReliable);

                GameObject instance = fightManager.instanceCard(ca, GetListByPosition(position));
                fightManager.tableCards.Add(instance);

                fightManager.MygameCards.RemoveAt(i);
            }
        }

    }

    public void searchOther(int card_id, int position)
    {
        for (int i = fightManager.OtherHandCardList.Count - 1; i >= 0; i--)
        {
            Card ca = fightManager.OtherHandCardList[i];
            if (ca.id == card_id)
            {
                fightManager.OtherHandCardList.RemoveAt(i);
            }
        }

        for (int i = fightManager.OthergameCards.Count - 1; i >= 0; i--)
        {
            Card ca = fightManager.OthergameCards[i];
            if (ca.id == card_id)
            {
                fightManager.OthergameCards.RemoveAt(i);
            }
        }

    }


    private Transform[] GetAttackListByPosition(int position)
    {
        string listName = GetListNameByPosition(position);
        Transform[] closeAttackList = null;
        if (fightManager.MyGameImages.transform.Find(listName) != null)
        {
            closeAttackList = fightManager.MyGameImages.transform.Find(listName).GetComponentsInChildren<Transform>();
        }
        else
        {
            closeAttackList = fightManager.OtherGameImages.transform.Find(listName).GetComponentsInChildren<Transform>();
        }
        return closeAttackList;
    }

    private string GetListNameByPosition(int position)
    {
        string listName = "";

        switch (position)
        {
            case 1:
                listName = "cityAttackList";
                break;
            case 2:
                listName = "remoteAttackList";
                break;
            case 3:
                listName = "closeAttackList";
                break;
            case 4:
                listName = "otherCloseAttackList";
                break;
            case 5:
                listName = "otherRemoteAttackList";
                break;
            case 6:
                listName = "otherCityAttackList";
                break;
            case 7:
                listName = "cityTrump";
                break;
            case 8:
                listName = "remoteTrump";
                break;
            case 9:
                listName = "closeTrump";
                break;
            case 11:
                listName = "otherCloseTrump";
                break;
            case 12:
                listName = "otherRemoteTrump";
                break;
            case 13:
                listName = "otherCityTrump";
                break;

            default:
                break;
        }

        return listName;
    }

    public Transform GetListByPosition(int position)
    {
        string listName = GetListNameByPosition(position);
        Transform transform = null;
        switch (position)
        {
            case 1:
                transform = fightManager.MyGameImages.transform.Find(listName);
                break;
            case 2:
                transform = fightManager.MyGameImages.transform.Find(listName);
                break;
            case 3:
                transform = fightManager.MyGameImages.transform.Find(listName);
                break;
            case 4:
                transform = fightManager.OtherGameImages.transform.Find(listName);
                break;
            case 5:
                transform = fightManager.OtherGameImages.transform.Find(listName);
                break;
            case 6:
                transform = fightManager.OtherGameImages.transform.Find(listName);
                break;
            default:
                break;
        }
        return transform;
    }
}
