using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageController : MonoBehaviour
{
    public static GameObject messageBox;
    public static GameObject notice;
    private static GameObject currentMessageBox; // 记录当前的消息框对象



    void Start()
    {
        messageBox = Resources.Load<GameObject>("Prefab/MessageBox");
        notice = GameObject.Find("Canvas/notice");
        Debug.Log(messageBox);
        Debug.Log(notice);
    }

    void Update()
    {

    }

    public static void ShowMessage(string str, Vector3 pos, float duration = 3f)
    {
        GameObject instant = Instantiate(messageBox, pos, Quaternion.identity);

        instant.transform.SetParent(notice.transform);
        TextMeshProUGUI message = instant.transform.Find("back/text").GetComponent<TextMeshProUGUI>();

        if (message != null)
        {
            message.text = str;
            instant.SetActive(true);
            currentMessageBox = instant;
            Debug.Log(message.text);
        }
        else
        {
            Debug.LogError("Unable to find the 'Text' component in the message box.");
        }
    }

    public static void HideMessage()
    {
        if (currentMessageBox != null)
        {
            currentMessageBox.SetActive(false);
        }
    }

    public void OnConfirmButtonClicked()
    {
        HideMessage();
    }
}
