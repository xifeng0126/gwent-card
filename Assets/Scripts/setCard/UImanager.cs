using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
    setCardManager setCardManager;
    CardListController cardListController;
    public CardsController cardController;

    public GameObject Cards;
    public GameObject Card;

    public GameObject campButton;
    public GameObject cardTypeButton;

    public TextMeshProUGUI campSkill;

    bool northernFlag = false;
    bool nilfgaardianFlag = false;
    bool monsterFlag = false;
    bool scoiataelFlag = false;

    public int camp_id = 0;   //0:北方领域，1：尼弗迦德 2：怪物 3：松鼠党
    public int cardType_id = 0;  //0：领袖 1：特殊 2：基础 3；中立


    // Start is called before the first frame update
    void Start()
    {
        setCardManager = this.GetComponent<setCardManager>();
        cardListController = this.GetComponent<CardListController>();

        setAllcampButtonDull();
        setAllcardTypeButtonDull();
        //northern();
        //setCardsAvailable();

        //deck = new Deck();
    }

    public void initial()
    {
        //setAllcampButtonDull();
        //setAllcardTypeButtonDull();
        northern();
        //setCardsAvailable();
    }


    public void northern() //显示北方领域卡牌
    {
        cleanCards();
        setFlag(1);
        northernFlag = true;
        setAllcampButtonDull();
        ImageManager.setBrightColor(campButton.transform.Find("northern").GetComponent<Image>());

        leader();
        cardListController.showList(camp_id);

        campSkill.text = "阵营技能：赢得一局后可抽一张牌";
    }
    public void nilfgaardian()
    {
        cleanCards();
        setFlag(2);
        nilfgaardianFlag = true;
        setAllcampButtonDull();
        ImageManager.setBrightColor(campButton.transform.Find("nilfgaardian").GetComponent<Image>());

        leader();
        cardListController.showList(camp_id);

        campSkill.text = "阵营技能：平局时获得胜利";
    }
    public void monster()
    {
        cleanCards();
        setFlag(3);
        monsterFlag = true;
        setAllcampButtonDull();
        ImageManager.setBrightColor(campButton.transform.Find("monster").GetComponent<Image>());

        leader();
        cardListController.showList(camp_id);

        campSkill.text = "阵营技能：每局结束后随机选择一张怪物单位牌待在战场上";
    }
    public void scoiatael()
    {
        cleanCards();
        setFlag(4);
        scoiataelFlag = true;
        setAllcampButtonDull();
        ImageManager.setBrightColor(campButton.transform.Find("scoiatael").GetComponent<Image>());

        leader();
        cardListController.showList(camp_id);

        campSkill.text = "阵营技能：战斗开始时你可以决定谁先行动";
    }
    public void leader()
    {
        cleanCards();
        cardType_id = 0;
        setAllcardTypeButtonDull();
        ImageManager.setBrightColor(cardTypeButton.transform.Find("Leader").GetComponent<Image>());

        List<Card> cardList = null;

        if (northernFlag)
        {
            //Debug.Log(setCardManager.northernCards);
            cardList = setCardManager.northernCards;
        }
        else if (nilfgaardianFlag)
        {
            cardList = setCardManager.nilfgaardianCards;
        }
        else if (monsterFlag)
        {
            cardList = setCardManager.monsterCards;
        }
        else if (scoiataelFlag)
        {
            cardList = setCardManager.scoiataelCards;
        }

        if (cardList != null)
        {
            foreach (Card card in cardList)
            {
                if (card is LeaderCard)
                {
                    instanceCard(card);
                }
                cardController.SetGridHeight(5, 4);
            }
        }
        setCardsAvailable();
    }
    public void special()
    {
        cleanCards();
        cardType_id = 1;
        setAllcardTypeButtonDull();
        ImageManager.setBrightColor(cardTypeButton.transform.Find("Special").GetComponent<Image>());

        foreach (Card card in setCardManager.neutralCards)
        {
            if (card is SpecialCard)
            {
                instanceCard(card);
            }
            cardController.SetGridHeight(5, 7);
        }
        setCardsAvailable();
    }
    public void basic()
    {
        cleanCards();
        cardType_id = 2;
        setAllcardTypeButtonDull();
        ImageManager.setBrightColor(cardTypeButton.transform.Find("Basic").GetComponent<Image>());

        List<Card> cardList = null;
        int num = 0;

        if (northernFlag)
        {
            cardList = setCardManager.northernCards;
            num = 25;
        }
        else if (nilfgaardianFlag)
        {
            cardList = setCardManager.nilfgaardianCards;
            num = 29;
        }
        else if (monsterFlag)
        {
            cardList = setCardManager.monsterCards;
            num = 33;
        }
        else if (scoiataelFlag)
        {
            cardList = setCardManager.scoiataelCards;
            num = 22;
        }

        if (cardList != null)
        {
            foreach (Card card in cardList)
            {
                if (card is BasicCard)
                {
                    instanceCard(card);
                }
                cardController.SetGridHeight(5, num);
            }
        }
        setCardsAvailable();
    }
    public void Neutral()
    {
        cleanCards();
        cardType_id = 3;
        setAllcardTypeButtonDull();
        ImageManager.setBrightColor(cardTypeButton.transform.Find("Neutral").GetComponent<Image>());

        foreach (Card card in setCardManager.neutralCards)
        {
            if (card is BasicCard)
            {
                instanceCard(card);
            }
            cardController.SetGridHeight(5, 9);
        }
        setCardsAvailable();
    }

    public void freshCards()
    {
        switch(cardType_id)
        {
            case 0:
                leader();
                break;
            case 1:
                special();
                break;
            case 2:
                basic();
                break;
            case 3:
                Neutral();
                break;
        }
    }

   
    //三个函数用于处理卡组列表显示
    public void setAllCardsNotAvailable()
    {
        foreach (Transform child in Cards.transform)
        {
            CardControl cardControl = child.GetComponent<CardControl>();
            cardControl.available = false;
            ImageManager.setDullColor(cardControl.getImage());
        }
    }
    public void setCardsAvailable()
    {
        //Debug.Log(setCardManager.northernDeck);
        switch (camp_id)
        {
            case 0:
                SetAvailable(setCardManager.northernDeck);
                break;
            case 1:
                SetAvailable(setCardManager.nilfgaardianDeck);
                break;
            case 2:
                SetAvailable(setCardManager.monsterDeck);
                break;
            case 3:
                SetAvailable(setCardManager.scoiataelDeck);
                break;
            default:
                break;
        }
    }
    void SetAvailable(Deck deck)
    {
        switch (cardType_id)
        {
            case 0:
                if (deck.getLeaderNum() == 1)
                    setAllCardsNotAvailable();
                break;
            case 1:
                if (deck.getSpecialNum() == 10)
                    setAllCardsNotAvailable();
                break;
            case 2:
                if (deck.getBaseNum() == 25)
                    setAllCardsNotAvailable();
                break;
            case 3:
                if (deck.getBaseNum() == 25)
                    setAllCardsNotAvailable();
                break;
            default:
                break;
        }
    }


    public void cleanCards()
    {
        foreach (Transform child in Cards.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void setFlag(int camp)
    {
        switch (camp)
        {
            case 1:
                nilfgaardianFlag = false;
                monsterFlag = false;
                scoiataelFlag = false;
                camp_id = 0;
                break;
            case 2:
                northernFlag = false;
                monsterFlag = false;
                scoiataelFlag = false;
                camp_id = 1;
                break;
            case 3:
                northernFlag = false;
                nilfgaardianFlag = false;
                scoiataelFlag = false;
                camp_id = 2;
                break;
            case 4:
                northernFlag = false;
                nilfgaardianFlag = false;
                monsterFlag = false;
                camp_id = 3;
                break;
            default:
                break;
        }
    }
    public void setAllcampButtonDull()
    {
        ImageManager.setDullColor(campButton.transform.Find("northern").GetComponent<Image>());
        ImageManager.setDullColor(campButton.transform.Find("nilfgaardian").GetComponent<Image>());
        ImageManager.setDullColor(campButton.transform.Find("monster").GetComponent<Image>());
        ImageManager.setDullColor(campButton.transform.Find("scoiatael").GetComponent<Image>());
    }
    public void setAllcardTypeButtonDull()
    {
        ImageManager.setDullColor(cardTypeButton.transform.Find("Leader").GetComponent<Image>());
        ImageManager.setDullColor(cardTypeButton.transform.Find("Special").GetComponent<Image>());
        ImageManager.setDullColor(cardTypeButton.transform.Find("Basic").GetComponent<Image>());
        ImageManager.setDullColor(cardTypeButton.transform.Find("Neutral").GetComponent<Image>());
    }
    

    public void instanceCard(Card card)   // 实例化卡牌, 并添加点击事件
    {
        GameObject instance = Instantiate(Card, Cards.transform.position, Cards.transform.rotation, Cards.transform);
        CardControl cardControl = instance.GetComponent<CardControl>();
        cardControl.card = card;
        cardControl.display();
        cardControl.showNumber(setCardManager.getNumber(card.camp,card.id,camp_id));

        if (setCardManager.getNumber(cardControl.card.camp, cardControl.card.id, camp_id) == 0)
        {
            ImageManager.setDullColor(cardControl.getImage());
            cardControl.available = false;
        }
        else
        {
            cardControl.available = true;
        }   

        Button button = instance.AddComponent<Button>();
        button.onClick.AddListener(() =>
        {
            OnCardClick(cardControl);
        });
    }

    public void OnCardClick(CardControl cardControl)  // 点击卡牌，读写每个阵营的cardsList，并更新卡组
    {
        //Debug.Log(cardControl.available);
        setCardsAvailable();

        if (setCardManager.getNumber(cardControl.card.camp, cardControl.card.id, camp_id) == 0||cardControl.available == false)
        {
            return;
        }
        setCardManager.subCardNum(cardControl.card.camp, cardControl.card.id, camp_id, 1);
        cardControl.showNumber(setCardManager.getNumber(cardControl.card.camp, cardControl.card.id, camp_id));
        if (setCardManager.getNumber(cardControl.card.camp, cardControl.card.id, camp_id) == 0)
        {
            ImageManager.setDullColor(cardControl.getImage());
            cardControl.available = false;
        }

        switch (camp_id)
        {
            case 0:
                AddCard(setCardManager.northernDeck, cardControl.card);
                break;
            case 1:
                AddCard(setCardManager.nilfgaardianDeck, cardControl.card);
                break;
            case 2:
                AddCard(setCardManager.monsterDeck, cardControl.card);
                break;
            case 3:
                AddCard(setCardManager.scoiataelDeck, cardControl.card);
                break;
            default:
                break;
        }
        //cardListController.
        setCardsAvailable();
        cardListController.showList(camp_id);
    }

    public void AddCard(Deck deck,Card card)
    {
        switch (cardType_id)
        {
            case 0:
                deck.Leader = card;
                break;
            case 1:
                deck.Special.Add(card);
                break;
            case 2:
                deck.Base.Add(card);
                break;
            case 3:
                deck.Base.Add(card);
                break;
            default:
                break;
        }
    }//向卡组中添加卡牌

}
