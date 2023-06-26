using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsController : MonoBehaviour
{
    GridLayoutGroup grid;   //Layout Group组件
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGridHeight(int num,int childCount)    //设置列表长度(每行数量，总数量)
    {
        //Debug.Log(this.transform.childCount);
        grid = this.GetComponent<GridLayoutGroup>();
        //float childCount = this.transform.childCount;  //获得Layout Group子物体个数
        float height = ((childCount + num - 1) / num) * grid.cellSize.y + 3.0f;  //行数乘以Cell的高度，3.0f是微调
        height += (((childCount + num - 1) / num) - 1) * grid.spacing.y + 50;     //每行之间有间隔
        grid.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public void SetGridZero()
    {
        this.GetComponent<GridLayoutGroup>().GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
        //Debug.Log(this.GetComponent<GridLayoutGroup>().GetComponent<RectTransform>().si);
    }
}
