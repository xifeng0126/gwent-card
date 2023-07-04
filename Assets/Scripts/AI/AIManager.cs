using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SceneController;

public class AIManager : MonoBehaviour
{
    public GameObject tempObject;
    public GameObject cardPrefab;

    private SceneController controller;
    private GameObject field;

    public void AIInitialize()
    {
        Debug.Log("AI: 启动中...");
        // Initialize Controller
        controller = GetComponent<SceneController>();
        field = GetComponent<SceneController>().EnemyField;
        if (controller != null || field != null)
        {

            Debug.Log("AI:启动成功.");
        }
        else
            Debug.LogError("Could not initialize AI, field or scene controller not found!");
    }

    //----------------------------------------------AI Phases------------------------------------------------------------//
    // 重抽两张牌
    public void AIExchangeCards()
    {
        Debug.Log("AI: 换牌ing");

        controller.EnemyInfo.CardsExchanged = 0;

        IEnumerable<int> duplicates_enum = controller.EnemyInfo.HandList.GroupBy(x => x).SelectMany(g => g.Skip(1));
        List<int> duplicates = duplicates_enum.ToList();


        for (int i = 0; i < 2; i++)
        {
            // 有浓雾就换掉
            while (controller.EnemyInfo.HandList.Contains(204))
            {
                if (controller.EnemyInfo.CardsExchanged < 2)
                {
                    controller.ExchangeCard(204, false);
                    controller.EnemyInfo.CardsExchanged++;
                }
                else
                    break;
            }

            // 有重复的牌就从中取一张换掉
            foreach (int card in duplicates)
            {
                if (controller.EnemyInfo.CardsExchanged < 2)
                {
                    controller.ExchangeCard(card, false);
                    controller.EnemyInfo.CardsExchanged++;
                }
            }

            // 交换同名牌
            for (int j = 0; j < controller.EnemyInfo.HandList.Count; j++)
            {
                int card = controller.EnemyInfo.HandList[j];
                if (controller.EnemyInfo.CardsExchanged < 2)
                {
                    AiCard my_card = controller.GetCardStats(card);
                    string card_name = controller.TrimToChar(my_card._idstr, '_');

                    List<AiCard> other_cards = controller.GetCardsList(controller.EnemyInfo.HandList);

                    foreach (AiCard other_card in other_cards)
                    {
                        if (other_card._idstr.Contains(card_name) && other_card._id != card)
                        {
                            controller.ExchangeCard(card, false);
                            controller.EnemyInfo.CardsExchanged++;
                            j--;
                            break;
                        }
                    }
                }
            }
        }

        controller.EnemyInfo.CanExchange = false;

        Debug.Log("AI: 完成换牌。");
    }

    // AI 回合
    public void AIStartTurn()
    {

        Debug.Log("AI: 回合开始...");
        List<AiCard> hand_cards = controller.GetCardsList(controller.EnemyInfo.HandList); // 手牌中的所有卡牌列表
        List<AiCard> hand_units = controller.GetCardsList(GetHandUnits()); // 手牌中的单位卡列表
        List<AiCard> hand_specials = controller.GetCardsList(GetHandSpecials()); // 手牌中的特殊卡列表

        AiCard card_to_play;
        string card_to_play_row;

        // 获取玩家战力情况
        int pl_all_score = controller.GetTotalScore(true, "all");
        int pl_close_score = controller.GetTotalScore(true, "close");
        int pl_range_score = controller.GetTotalScore(true, "range");
        int pl_siege_score = controller.GetTotalScore(true, "siege");

        // 获取ai战力情况
        int ai_all_score = controller.GetTotalScore(false, "all");
        int ai_close_score = controller.GetTotalScore(false, "close");
        int ai_range_score = controller.GetTotalScore(false, "range");
        int ai_siege_score = controller.GetTotalScore(false, "siege");

        // 获取天气情况
        bool reduce_close = SearchWeather("close");
        bool reduce_range = SearchWeather("range");
        bool reduce_siege = SearchWeather("siege");

        // 获取AI的双倍卡情况
        bool ai_close_double = SearchDouble("close");
        bool ai_range_double = SearchDouble("range");
        bool ai_siege_double = SearchDouble("siege");

        // 获取AI的战力+1卡情况
        bool ai_close_morale = SearchMorale("close");
        bool ai_range_morale = SearchMorale("range");
        bool ai_siege_morale = SearchMorale("siege");

        int needed_score;
        List<AiCard> eligible_cards = new List<AiCard>();

        if (hand_units.Count > 0) // 当手中还有单位卡
        {
            if (controller.PlayerInfo.hasPassed || controller.PlayerInfo.HandList.Count == 0)
            {
                // 当玩家跳过回合或者玩家手牌为空时，AI追加战力超过玩家
                switch (controller.EnemyInfo.Lives)
                {
                    case 1:
                        // 只剩下最后一条命时全力出击
                        needed_score = pl_all_score - ai_all_score;
                        Debug.Log("需要的战力（情况1_1）: " + needed_score);
                        Debug.Log("AI: 对手跳过回合，我必须超越对手，否则我会输");
                        if (needed_score < 0) // 已经超越，跳过回合
                        {
                            controller.SkipTurn(false);
                            return;
                        }
                        else
                        {
                            if (reduce_close)
                            {
                                if (controller.EnemyInfo.HandList.Contains(201)) // 先清除天气负面效果
                                {
                                    PlaceCard(201, "weather");
                                    return;
                                }
                            }

                            if (pl_range_score >= 10)//对方远程战力大于10，使用领袖卡技能（浓雾）使其战力下降
                            {
                                if (controller.EnemyInfo.canLeader && controller.PlayerInfo.RangeList.Count > 3)
                                {
                                    PlayLeader();
                                    return;
                                }
                            }

                            if (ai_close_score >= 5 && !ai_close_double)//如果近战战力大于5，对近战使用双倍卡
                            {
                                if (controller.EnemyInfo.HandList.Contains(202))     // 使用"指挥号角"卡
                                {
                                    PlaceCard(202, "sp_close");
                                    return;
                                }
                                else if (controller.EnemyInfo.HandList.Contains(22)) // 使用"丹德里恩"卡
                                {
                                    PlaceCard(22, "close");
                                    return;
                                }
                            }

                            eligible_cards.Clear();//清空待选卡列表
                            foreach (AiCard card in hand_units)
                            {
                                if (card.strength > needed_score)
                                    eligible_cards.Add(card);//符合条件的卡加入待选卡列表
                            }
                            if (eligible_cards.Count < 1) // 没有比需要的战力更高的卡牌，召唤最高战力的卡牌
                            {
                                // 从手牌中找到最高战力的卡牌
                                Debug.Log("打出最高战力的卡牌");
                                card_to_play = GetMaximumStrength(hand_units);
                                card_to_play_row = GetCardRow(card_to_play);
                                PlaceCard(card_to_play._id, card_to_play_row);
                                return;
                            }
                            else
                            {
                                // 有满足的卡则找到最低的满足条件的卡牌
                                card_to_play = GetMinimumStrength(eligible_cards);
                                card_to_play_row = GetCardRow(card_to_play);
                                PlaceCard(card_to_play._id, card_to_play_row);
                                return;
                            }
                        }
                    case 2:
                        // 当还有两条命时
                        needed_score = pl_all_score - ai_all_score;
                        Debug.Log("需要的战力（情况1_2）: " + needed_score);
                        if (needed_score < 0) // 已经超越对手，跳过回合
                        {
                            controller.SkipTurn(false);
                            return;
                        }
                        else 
                        {
                            // 打出最强的卡牌
                            card_to_play = GetMaximumStrength(hand_units);
                            card_to_play_row = GetCardRow(card_to_play);
                            PlaceCard(card_to_play._id, card_to_play_row);
                            return;
                        }
                }
                return;
            }
            // 对手还在出牌阶段
            else
            {
                switch (controller.EnemyInfo.Lives)
                {
                    case 1:
                        // 还有一条命，全力出击
                        needed_score = pl_all_score - ai_all_score;
                        Debug.Log("需要的战力（情况2_1）: " + needed_score);
                        Debug.Log("AI: 需要出牌！！！");
                        if (needed_score <= -40) // 战力已超越较大，跳过回合
                        {
                            controller.SkipTurn(false);
                            return;
                        }
                        else
                        {
                            if (reduce_close)
                            {
                                if (controller.EnemyInfo.HandList.Contains(201)) // 先清除天气负面效果
                                {
                                    PlaceCard(201, "weather");
                                    return;
                                }
                            }

                            if (pl_range_score >= 10)
                            {
                                if (controller.EnemyInfo.canLeader && controller.PlayerInfo.RangeList.Count > 3)
                                {
                                    PlayLeader();
                                    return;
                                }
                            }

                            if (ai_close_score >= 5 && !ai_close_double)
                            
                                if (controller.EnemyInfo.HandList.Contains(202))  
                                {
                                    PlaceCard(202, "sp_close");
                                    return;
                                }
                                else if (controller.EnemyInfo.HandList.Contains(22)) 
                                {
                                    PlaceCard(22, "close");
                                    return;
                                }
                            }

                            eligible_cards.Clear();
                            foreach (AiCard card in hand_units)
                            {
                                if (card.strength > needed_score)
                                    eligible_cards.Add(card);
                            }
                            if (eligible_cards.Count < 1)
                            {
                                Debug.Log("打出最高战力的卡牌");
                                card_to_play = GetMaximumStrength(hand_units);
                                card_to_play_row = GetCardRow(card_to_play);
                                PlaceCard(card_to_play._id, card_to_play_row);
                                return;
                            }
                            else
                            {
                                card_to_play = GetMinimumStrength(eligible_cards);
                                card_to_play_row = GetCardRow(card_to_play);
                                PlaceCard(card_to_play._id, card_to_play_row);
                                return;
                            }
                    case 2:
                        // 两条命，被动出牌
                        needed_score = pl_all_score - ai_all_score;
                        Debug.Log("需要的战力（情况2_2）: " + needed_score);
                        Debug.Log("AI: 正在观望...");
                        if (needed_score < -25 || needed_score > 30) // AI超越25分或玩家超越30分，跳过回合
                        {
                            controller.SkipTurn(false);
                            return;
                        }
                        else // needed_score在区间[-25, 30]
                        {
                            if (needed_score < 0)
                            {// 超越玩家
                                if (hand_units.Count >= 5)// 手上单位牌数大于5，则追加，否则跳过回合
                                {
                                    // 打出最强的单位卡牌
                                    card_to_play = GetMaximumStrength(hand_units);
                                    card_to_play_row = GetCardRow(card_to_play);
                                    PlaceCard(card_to_play._id, card_to_play_row);
                                }
                                else
                                {
                                    controller.SkipTurn(false);
                                    return;
                                }
                                return;
                            }
                            else // 被玩家超越
                            {
                                if (hand_units.Count >= 5)
                                {
                                    // 打出最弱的单位牌
                                    card_to_play = GetMinimumStrength(hand_units);
                                    card_to_play_row = GetCardRow(card_to_play);
                                    PlaceCard(card_to_play._id, card_to_play_row);
                                    return;
                                }
                                else
                                {
                                    controller.SkipTurn(false);
                                    return;
                                }
                            }
                        }
                }
            }
        }
        // 如果手牌中还有其他类型的卡牌，则随机选择一张打出
        else if (hand_cards.Count > 0)
        {
            card_to_play = GetRandomCard(hand_cards);
            card_to_play_row = GetCardRow(card_to_play);
            PlaceCard(card_to_play._id, card_to_play_row);
            return;
        }
        // 如果对手有领袖牌未使用且牌组中有"浓雾"卡牌，则使用领袖牌
        else if (controller.EnemyInfo.canLeader && controller.PlayerInfo.HandList.Contains(301))
        {
            PlaceCard(301, "leader");
            return;
        }
        // 如果上述情况均不满足，则跳过回合
        else
        {
            controller.SkipTurn(false);
            return;
        }
    }
        //-----------------------------------------------------AiCard Placing-------------------------------------------------//
        // Place the card
        private void PlaceCard(int cardId, string card_row)
    {
        AiCard card = controller.GetCardStats(cardId);
        GameObject instantiatedCard = Instantiate(cardPrefab);
        instantiatedCard.name = card._id.ToString();
        instantiatedCard.transform.Find("Stats").GetComponent<CardStats>().name = card.name;
        instantiatedCard.transform.Find("Stats").GetComponent<CardStats>()._id = card._id;
        instantiatedCard.transform.Find("Stats").GetComponent<CardStats>()._idstr = card._idstr;
        instantiatedCard.transform.Find("Stats").GetComponent<CardStats>().faction = card.faction;
        instantiatedCard.transform.Find("Stats").GetComponent<CardStats>().unique = card.unique;
        instantiatedCard.transform.Find("Stats").GetComponent<CardStats>().strength = card.strength;
        instantiatedCard.transform.Find("Stats").GetComponent<CardStats>().row = card.row;
        instantiatedCard.transform.Find("Stats").GetComponent<CardStats>().ability = card.ability;
        instantiatedCard.tag = "Enemy";

        GameObject cardRowGO = GetCardRowGO(card_row);
        instantiatedCard.transform.SetParent(tempObject.transform, false);

        controller.DirectlyPlaceCard(instantiatedCard, cardRowGO, false);
    }

    // Gets the card row GO
    private GameObject GetCardRowGO(string row)
    {
        switch (row)
        {
            // Unit Cards
            case "close":
                return field.transform.Find("Close").Find("Unit").gameObject;
            case "range":
                return field.transform.Find("Range").Find("Unit").gameObject;
            case "siege":
                return field.transform.Find("Siege").Find("Unit").gameObject;
            // Special Cards
            case "sp_close":
                return field.transform.Find("Close").Find("Special").gameObject;
            case "sp_range":
                return field.transform.Find("Range").Find("Special").gameObject;
            case "sp_siege":
                return field.transform.Find("Siege").Find("Special").gameObject;

            case "weather":
                return controller.WeatherField;

            // Fix
            default:
                Debug.LogError("GetCardRowGO: Unexpected Error!");
                return null;
        }
    }

    private string GetCardRow(AiCard card)
    {
        switch (card.row)
        {
            case "close":
                return card.row;
            case "range":
                return card.row;
            case "siege":
                return card.row;
            case "close_range":
                return "close";

            case "weather":
                return "weather";
            case "one_time":
                return "weather";
            case "special":
                return "sp_close";

            default:
                Debug.LogError("GetCardRow: Unexpected Error ! Cannot Find valid Row");
                return null;
        }
    }

    // 使用牌组中的天气牌（浓雾）
    private void PlayLeader()
    {
        if (controller.EnemyInfo.DeckList.Contains(204))  
        {
            controller.EnemyInfo.DeckList.Remove(204);
            field.transform.Find("Leader").GetComponent<LeaderManager>().DisableButtonRC();
            controller.EnemyInfo.canLeader = false;
            PlaceCard(204, "weather");
        }
    }

 
    private bool SearchWeather(string row)
    {
        bool reduced = false;
        switch (row)
        {
            case "close": // 冰霜
                if (controller.weatherList.Contains(200))
                    reduced = true;
                break;
            case "range": // 浓雾
                if (controller.weatherList.Contains(204))
                    reduced = true;
                break;
            case "siege": // 地形雨
                if (controller.weatherList.Contains(206))
                    reduced = true;
                break;
        }
        return reduced;
    }

    // 查询是否有双倍卡（领导号角）
    private bool SearchDouble(string row)
    {
        bool doubled = false;
        switch (row)
        {
            case "close": 
                if (controller.EnemyInfo.SpCloseList.Contains(202) || controller.EnemyInfo.CloseList.Contains(22))
                    doubled = true;
                break;
            case "range":
                if (controller.EnemyInfo.SpRangeList.Contains(202))
                    doubled = true;
                break;
            case "siege":
                if (controller.EnemyInfo.SpSiegeList.Contains(202))
                    doubled = true;
                break;
        }
        return doubled;
    }

    // 查询提振士气（该行卡战力分别加1）
    private bool SearchMorale(string row)
    {
        bool moraled = false;
        List<AiCard> cards_list;
        switch (row)
        {
            case "close":
                cards_list = controller.GetCardsList(controller.EnemyInfo.CloseList);
                foreach (AiCard card in cards_list)
                    if (card.ability == "morale_boost")
                    {
                        moraled = true;
                        break;
                    }
                break;
            case "range":
                cards_list = controller.GetCardsList(controller.EnemyInfo.RangeList);
                foreach (AiCard card in cards_list)
                    if (card.ability == "morale_boost")
                    {
                        moraled = true;
                        break;
                    }
                break;
            case "siege":
                cards_list = controller.GetCardsList(controller.EnemyInfo.SiegeList);
                foreach (AiCard card in cards_list)
                    if (card.ability == "morale_boost")
                    {
                        moraled = true;
                        break;
                    }
                break;
        }
        return moraled;
    }

    // Get a list of unit card ids in hand (except dandelion)
    private List<int> GetHandUnits()
    {
        List<int> returned_list = new List<int>();
        List<AiCard> cards_list = controller.GetCardsList(controller.EnemyInfo.HandList);
        foreach (AiCard card in cards_list)
        {
            if (card.faction != "Special" && card.ability != "commander_horn")
            {
                returned_list.Add(card._id);
            }
        }
        return returned_list;
    }

    //获取手牌中的特殊牌
    private List<int> GetHandSpecials()
    {
        List<int> returned_list = new List<int>();
        List<AiCard> cards_list = controller.GetCardsList(controller.EnemyInfo.HandList);
        foreach (AiCard card in cards_list)
        {
            if (card.faction == "Special")
            {
                returned_list.Add(card._id);
            }
        }
        return returned_list;
    }

    //----------------------------------------------AiCard List Manipulation
    private AiCard GetMinimumStrength(List<AiCard> cards_list)
    {
        AiCard card = cards_list[0];
        for (int i = 1; i < cards_list.Count; i++)
        {
            if (cards_list[i].strength <= card.strength)
                card = cards_list[i];
        }
        return card;
    }

    private AiCard GetMaximumStrength(List<AiCard> cards_list)
    {
        AiCard card = cards_list[0];
        for (int i = 1; i < cards_list.Count; i++)
        {
            if (cards_list[i].strength >= card.strength)
                card = cards_list[i];
        }
        return card;
    }

    private AiCard GetRandomCard(List<AiCard> cards_list)
    {
        System.Random rnd = new System.Random();
        int r = rnd.Next(cards_list.Count);
        return cards_list[r];
    }

}

