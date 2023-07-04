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
                    info += "�޼���";
                    break;
                case 1:
                    info += "Ӣ�ۣ����ᱻ�κ��ر��ƻ�����Ӱ��";
                    break;
                case 2:
                    info += "ͬ��֮�飺����ͬ���Ƶ��Աߣ�����������������Ϊ˫��";
                    break;
                case 3:
                    info += "��������ڶԷ���ս���ϣ����ڶ��ֵ��ܷ��У������ɴ����Լ����������ٳ�2����";
                    break;
                case 4:
                    info += "����ʿ��������ͬһ�е�λ���+1����������";
                    break;
                case 5:
                    info += "ҽ�����ڷ��ƶ�����ѡһ�ŵ�λ�ƣ�����ѡӢ���ƻ��ر��ƣ������ϴ��";
                    break;
                case 6:
                    info += "˪������˫��������н�ս��ս�Ƶ�������Ϊ1";
                    break;
                case 7:
                    info += "Ũ����˫���������Զ����ս�Ƶ�������Ϊ1";
                    break;
                case 8:
                    info += "�����꣺��˫��������й�����ս�Ƶ�������Ϊ1";
                    break;
                case 9:
                    info += "���磺�Ƴ����������ƣ�˪��Ũ��͵����꣩Ч��";
                    break;
                case 10:
                    info += "���ƣ�ʹ�ú����ϣ�ɱ��ս������ǿ����";
                    break;
                case 11:
                    info += "�쵼�Žǣ��������е�λ�������ӱ���һ��ֻ��ʹ��һ��";
                    break;
                case 12:
                    info += "�ն�����ս���ϵ�һ���ƻ�����ʹ�ƻص�������";
                    break;
                case 13:
                    info += "����-��ս�����������н�ս��ս��λ�������ܺ͵��ڻ����10����ݻٵ�����ǿ�Ľ�ս��ս��λ";
                    break;
                case 14:
                    info += "���ϣ����������ҵ�����ͬ�����ƣ����ϴ��";
                    break;
                case 15:
                    info += "���ݣ����÷��ڽ�ս�л�Զ���У��÷ź󼴲����ƶ�";
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
