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
    public static void ShowMessage(string str, float duration = 3f)
    {
        messageBox = Resources.Load<GameObject>("Prefab/MessageBox");
        Transform centerTransform = null;
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            centerTransform = hit.transform;
        }
        else
        {
            Debug.LogError("No object found at the screen center. Make sure there are objects with colliders intersecting the ray.");
        }
        Debug.Log(centerTransform);
        GameObject instant = Instantiate(messageBox, centerTransform.position, Quaternion.identity);
        TextMeshProUGUI message = instant.transform.Find("back/text").GetComponent<TextMeshProUGUI>();
        if (message != null)
        {
            message.text = str;
            instant.SetActive(true);
            currentMessageBox = instant;
        }
        else
        {
            Debug.LogError("Unable to find the 'Text' component in the message box.");
        }
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
