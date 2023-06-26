using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class CardControl : MonoBehaviour
{
    public Card card;
    public bool available = true;
    Sprite sprite;
    Texture2D texture;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public CardControl(Card card)
    {
        this.card = card;
    }

    public void display()
    {
        //Debug.Log(card.image);
        texture = Resources.Load<Texture2D>(card.image);
        sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        this.transform.Find("mask/Image").GetComponent<UnityEngine.UI.Image>().sprite = sprite;
        if (card.number == 0)
        {
            ImageManager.setDullColor(this.transform.Find("mask/Image").GetComponent<UnityEngine.UI.Image>());
        }
    }

    public void showNumber()
    {
        //Debug.Log(this.transform.Find("string").transform);
        this.transform.Find("string").GetComponent<TextMeshProUGUI>().text = "数量：";
        this.transform.Find("number").GetComponent<TextMeshProUGUI>().text = card.number.ToString();
    }

    public void showNumber(int num)
    {
        this.transform.Find("string").GetComponent<TextMeshProUGUI>().text = "数量：";
        this.transform.Find("number").GetComponent<TextMeshProUGUI>().text = num.ToString();
    }

    public UnityEngine.UI.Image getImage()
    {
        return this.transform.Find("mask/Image").GetComponent<UnityEngine.UI.Image>();
    }

    public void setSize(float hight,float width)
    {
        this.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(hight, width);
        this.transform.Find("mask").GetComponent<RectTransform>().sizeDelta = new Vector2(hight, width);
        this.transform.Find("mask/Image").GetComponent<RectTransform>().sizeDelta = new Vector2(hight, width);
    }

}
