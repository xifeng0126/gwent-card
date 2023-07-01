using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;


[System.Serializable]
public class MiniCardControl : MonoBehaviour
{
    public Card card;
    Sprite sprite;
    Texture2D texture;

    public int attackNum;

    string heroAttack = "Textures/cardhero";
    string baseAttack = "Textures/cardpower";

    public GameObject attackNumImage;
    public GameObject attackNumber;

    // Start is called before the first frame update
    void Start()
    {
        //attackNumImage.SetActive(false);
        //attackNumber.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public MiniCardControl(Card card)
    {
        this.card = card;
    }

    public void setAttack()
    {
        if(card is BasicCard)
        {
            attackNum = ((BasicCard)card).attack;
        }
        display();
    }

    public void setAttack(int attack)
    {
        //Debug.Log("red" + attack);
        if (card is BasicCard)
        {
            attackNum = attack;
        }
    }

    public void display()
    {
        //Debug.Log("att:" + attackNum);
        texture = Resources.Load<Texture2D>(card.image);
        sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        this.transform.Find("mask/Image").GetComponent<UnityEngine.UI.Image>().sprite = sprite;
        if(card is BasicCard && card.skill1 == 1)
        {
            attackNumImage.SetActive(true);
            attackNumber.SetActive(true);
            texture = Resources.Load<Texture2D>("Textures/cardhero" );
            sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            attackNumImage.transform.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
            attackNumber.transform.GetComponent<TextMeshProUGUI>().text = attackNum.ToString();
            attackNumber.transform.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1);
            attackNumImage.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            attackNumImage.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-18.5f, 37);
        }
        else if (card is BasicCard && card.skill1 != 1)
        {
            //Debug.Log(attackNum);
            attackNumImage.SetActive(true);
            attackNumber.SetActive(true);
            texture = Resources.Load<Texture2D>("Textures/cardpower");
            sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            attackNumImage.transform.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
            attackNumber.transform.GetComponent<TextMeshProUGUI>().text = attackNum.ToString();
            attackNumber.transform.GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0);
            attackNumImage.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
            attackNumImage.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-29.5f, 46);
        }
        else
        {
            attackNumImage.SetActive(false);
            attackNumber.SetActive(false);
        }
    }

    public void displayRed()
    {
        texture = Resources.Load<Texture2D>(card.image);
        sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        this.transform.Find("mask/Image").GetComponent<UnityEngine.UI.Image>().sprite = sprite;
        if (card is BasicCard && card.skill1 == 1)
        {
            attackNumImage.SetActive(true);
            attackNumber.SetActive(true);
            texture = Resources.Load<Texture2D>("Textures/cardhero");
            sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            attackNumImage.transform.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
            attackNumber.transform.GetComponent<TextMeshProUGUI>().text = attackNum.ToString();
            attackNumber.transform.GetComponent<TextMeshProUGUI>().color = new Color(1, 0, 0);
            attackNumImage.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            attackNumImage.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-18.5f, 37);
        }
        else if (card is BasicCard && card.skill1 != 1)
        {
            //Debug.Log(attackNum);
            attackNumImage.SetActive(true);
            attackNumber.SetActive(true);
            texture = Resources.Load<Texture2D>("Textures/cardpower");
            sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            attackNumImage.transform.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
            attackNumber.transform.GetComponent<TextMeshProUGUI>().text = attackNum.ToString();
            attackNumber.transform.GetComponent<TextMeshProUGUI>().color = new Color(1, 0, 0);
            attackNumImage.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
            attackNumImage.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-29.5f, 46);
        }
        else
        {
            attackNumImage.SetActive(false);
            attackNumber.SetActive(false);
        }
    }

}
