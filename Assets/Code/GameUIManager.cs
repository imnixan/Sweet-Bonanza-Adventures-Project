using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI redDotsCounter,
        greenDotsCounter,
        moves,
        scores;

    [SerializeField]
    Dot greenDot,
        redDot;

    void Start()
    {
        greenDot.CreateDot();
        redDot.CreateDot();
    }

    public void UpdateRedDotsCount(int newValue)
    {
        StartCoroutine(UpdateDotsCount(redDot, redDotsCounter, newValue));
    }

    public void UpdateGreenDotsCount(int newValue)
    {
        StartCoroutine(UpdateDotsCount(greenDot, greenDotsCounter, newValue));
    }

    public void UpdateMoves(int movesCount, int newscores)
    {
        moves.text = movesCount.ToString();
        scores.text = newscores.ToString();
    }

    IEnumerator UpdateDotsCount(Dot dot, TextMeshProUGUI counter, int newValue)
    {
        int currentValue = Int32.Parse(counter.text);
        int showValue = 20 - newValue;
        if (showValue < 0)
        {
            showValue = 0;
        }

        dot.PlayConnectAnim();
        while (currentValue >= showValue)
        {
            counter.text = currentValue.ToString();
            yield return null;
            currentValue--;
        }
    }

    private void OnDisable()
    {
        redDotsCounter.text = "20";
        greenDotsCounter.text = "20";
        moves.text = "12";
    }
}
