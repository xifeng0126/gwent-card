using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageController : MonoBehaviour
{
    public static GameObject messageBox;
    private static GameObject currentMessageBox; // 记录当前的消息框对象



    void Start()
    {
        messageBox = Resources.Load<GameObject>("Prefab/MessageBox");
        //Debug.Log(messageBox);
    }

    void Update()
    {

    }

    public static void ShowMessage(string str, GameObject pos, float duration = 3f)
    {
        GameObject instant = Instantiate(messageBox, pos.transform.position, Quaternion.identity);

        instant.transform.SetParent(pos.transform);
        TextMeshProUGUI message = instant.transform.Find("back/text").GetComponent<TextMeshProUGUI>();

        if (message != null)
        {
            message.text = str;
            instant.SetActive(true);
            currentMessageBox = instant;
            //Debug.Log(message.text);
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
