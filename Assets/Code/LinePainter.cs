using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePainter : MonoBehaviour
{
    [SerializeField]
    private GameManager gm;
    private Camera camera;
    private List<Dot> dotsLine = new List<Dot>();
    private Dot tempDot;
    private LineRenderer lr;
    private Vector2 tapPosition;
    private RaycastHit2D hit;
    public bool fingerHold;

    void Start() { }

    public void SetLineSize(float size)
    {
        camera = Camera.main;
        lr = GetComponent<LineRenderer>();
        lr.startWidth = size / 1000;
        lr.endWidth = lr.startWidth;
    }

    // Update is called once per frame
    void Update()
    {
        if (fingerHold)
        {
            DrawLine();
            CheckFingerUp();
            CatchNewDots();
        }
        else
        {
            CheckFingerDown();
        }
    }

    private void CheckFingerDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            tapPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(tapPosition, Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Dot"))
                {
                    dotsLine.Clear();
                    tempDot = hit.collider.GetComponent<Dot>();
                    dotsLine.Add(tempDot);
                    gm.SetStartDot(tempDot);
                    fingerHold = true;
                    lr.startColor = gm.GetPopColor(tempDot.Sprite);
                    lr.endColor = lr.startColor;
                }
            }
        }
    }

    private void CheckFingerUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            lr.positionCount = 0;
            fingerHold = false;
            gm.DestroyDots(dotsLine);
        }
    }

    private void CatchNewDots()
    {
        tapPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        FollowFingerByLine(tapPosition);
        hit = Physics2D.Raycast(tapPosition, Vector2.zero);
        if (hit && hit.collider.gameObject.CompareTag("Dot"))
        {
            Dot tempDot = hit.collider.GetComponent<Dot>();
            if (!dotsLine.Contains(tempDot))
            {
                if (gm.CanConnectWithThisDot(tempDot))
                {
                    dotsLine.Add(tempDot);
                    tempDot.PlayConnectAnim();
                }
            }
            else if (dotsLine[dotsLine.Count - 1] != tempDot)
            {
                int thisDotIndex = dotsLine.IndexOf(tempDot);
                gm.SetStartDot(tempDot);
                RedrawLine(thisDotIndex);
            }
        }
    }

    private void RedrawLine(int newLastDot)
    {
        for (int i = dotsLine.Count - 1; i > newLastDot; i--)
        {
            dotsLine.RemoveAt(i);
            lr.positionCount--;
        }
    }

    private void FollowFingerByLine(Vector2 fingerPosition)
    {
        if (lr.positionCount > 1)
        {
            lr.SetPosition(lr.positionCount - 1, fingerPosition);
        }
    }

    private void DrawLine()
    {
        if (lr.positionCount != dotsLine.Count + 1)
        {
            lr.positionCount = 0;
            foreach (Dot dot in dotsLine)
            {
                lr.positionCount++;
                lr.SetPosition(lr.positionCount - 1, dot.GlobalCoords);
            }
            lr.positionCount++;
        }
    }
}
