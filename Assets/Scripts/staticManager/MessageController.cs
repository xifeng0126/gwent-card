using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageController : MonoBehaviour
{
    public TextMeshProUGUI message;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showMessage(string message,GameObject position)
    {
        this.message.text = message;
        //GameObject instance = Instantiate(this, position.transform.position, position.transform.rotation, position.transform);
    }
}
