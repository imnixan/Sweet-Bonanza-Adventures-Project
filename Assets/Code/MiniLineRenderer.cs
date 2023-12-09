using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniLineRenderer : MonoBehaviour
{
    private LineRenderer lr;

    public void ConnectDots(Color color, List<RectTransform> dots)
    {
        if(!lr)
        {
            lr=GetComponent<LineRenderer>();
        }
        lr.startWidth = dots[0].sizeDelta.x / 1000;
        lr.endWidth = lr.startWidth;
        lr.startColor = color;
        lr.endColor = lr.startColor;
        lr.positionCount = 0;
        foreach(RectTransform dot in dots)
        {
            lr.positionCount ++;
            lr.SetPosition(lr.positionCount - 1, dot.position);
            dot.GetComponent<Image>().color = color;
        }
    }
}
