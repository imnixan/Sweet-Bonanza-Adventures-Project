using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private FieldGenerator fieldGenerator;

    [SerializeField]
    private TextMeshProUGUI hpLeft,
        hpLeftBot,
        scoresWin;

    [SerializeField]
    private GameObject dotPrefab;

    [SerializeField]
    private Sprite redCandy,
        greenCandy,
        blueCandy,
        orangeCandy;

    [SerializeField]
    private LinePainter linePainter;

    [SerializeField]
    private GameObject popEffect;

    [SerializeField]
    private GameUIManager guimanager;

    [SerializeField]
    private AudioClip[] choose;

    [SerializeField]
    private RectTransform startWindow,
        winWindow,
        loseWindow,
        botUI,
        topUI;

    [SerializeField]
    private AudioClip boop;
    private RectTransform field;
    private int greenDotsDestroyed;
    private int redDotsDestroyed;

    [SerializeField]
    private int rounds = 12;
    private int fieldSize = 7;
    private FieldCell[,] gameField;
    private Sprite CurrentCandy;
    private Vector2 lastDotPosition;
    private int minLine = 3;
    private AudioSource sound;

    private int scores;

    private bool IsSoundOn
    {
        get { return PlayerPrefs.GetString("SoundSettings", "ON") == "ON"; }
    }

    private void OnEnable()
    {
        sound = GetComponent<AudioSource>();
        if (PlayerPrefs.GetInt("MusicSettings", 1) == 1)
        {
            sound.Play();
        }
        linePainter.enabled = false;
        hpLeft.text = PlayerPrefs.GetInt("Lives", 5).ToString();
        hpLeftBot.text = hpLeft.text;
        gameField = fieldGenerator.GenerateField(
            this,
            fieldSize,
            new Sprite[] { greenCandy, redCandy, blueCandy, orangeCandy },
            dotPrefab
        );
        linePainter.SetLineSize(gameField[0, 0].GetComponent<RectTransform>().sizeDelta.x);
        field = fieldGenerator.GetComponent<RectTransform>();
    }

    public void SetStartDot(Dot dot)
    {
        CurrentCandy = dot.Sprite;
        lastDotPosition = dot.FieldCoords;
    }

    public bool CanConnectWithThisDot(Dot dot)
    {
        bool correctColor = dot.Sprite == CurrentCandy;
        bool correctPosition = Vector2.Distance(lastDotPosition, dot.FieldCoords) < 1.5;
        if (correctColor && correctPosition)
        {
            if (IsSoundOn)
            {
                sound.PlayOneShot(choose[Random.Range(0, choose.Length)]);
            }
            lastDotPosition = dot.FieldCoords;
        }
        return correctColor && correctPosition;
    }

    public void DestroyDots(List<Dot> dots)
    {
        if (dots.Count >= minLine)
        {
            Vibrate();
            if (IsSoundOn)
            {
                sound.PlayOneShot(boop);
            }
            AddScores(dots.Count);
            StartCoroutine(IEDestroyDots(dots));
        }
    }

    IEnumerator IEDestroyDots(List<Dot> dots)
    {
        foreach (Dot dot in dots)
        {
            Color popColor = GetPopColor(dot.Sprite);

            Instantiate(popEffect, dot.GlobalCoords, new Quaternion(), field)
                .GetComponent<ParticleSystem>()
                .startColor = popColor;
            Destroy(dot.gameObject);
        }
        yield return null;
        fieldGenerator.FillEmptySpaces();
    }

    public Color GetPopColor(Sprite candy)
    {
        if (candy == greenCandy)
        {
            return Color.green;
        }
        if (candy == redCandy)
        {
            return Color.red;
        }
        if (candy == blueCandy)
        {
            return Color.blue;
        }
        if (candy == orangeCandy)
        {
            return Color.yellow + Color.red;
        }
        return Color.white;
    }

    private void AddScores(int dotsCount)
    {
        rounds--;
        scores += dotsCount * 5;
        guimanager.UpdateMoves(rounds, scores);
        if (CurrentCandy == redCandy)
        {
            redDotsDestroyed += dotsCount;
            guimanager.UpdateRedDotsCount(redDotsDestroyed);
        }
        else if (CurrentCandy == greenCandy)
        {
            greenDotsDestroyed += dotsCount;
            guimanager.UpdateGreenDotsCount(greenDotsDestroyed);
        }

        if (greenDotsDestroyed >= 20 && redDotsDestroyed >= 20 && rounds >= 0)
        {
            winWindow.DOAnchorPosX(0, 0.5f);
            botUI.DOAnchorPosY(-500, 0.5f);
            topUI.DOAnchorPosY(500, 0.5f);
            scoresWin.text = scores.ToString();
            PlayerPrefs.SetInt("Scores", PlayerPrefs.GetInt("Scores") + scores);
            PlayerPrefs.Save();
        }
        else if (rounds == 0)
        {
            PlayerPrefs.SetInt("Lives", PlayerPrefs.GetInt("Lives", 5) - 1);
            PlayerPrefs.Save();
            botUI.DOAnchorPosY(-500, 0.5f);
            topUI.DOAnchorPosY(500, 0.5f);
            loseWindow.DOAnchorPosX(0, 0.5f);
        }
    }

    private void OnDisable()
    {
        rounds = 0;
        greenDotsDestroyed = 0;
        redDotsDestroyed = 0;
        for (int i = field.childCount - 1; i >= 0; i--)
        {
            Destroy(field.GetChild(i).gameObject);
        }
    }

    public void RestartGame()
    {
        PlayerPrefs.SetInt("Lives", PlayerPrefs.GetInt("Lives", 5) - 1);
        if (PlayerPrefs.GetInt("Lives", 5) > 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene("main");
        }
    }

    private void Vibrate()
    {
        Handheld.Vibrate();
    }

    public void Play()
    {
        startWindow.DOAnchorPosX(2000, 0.5f);
        botUI.DOAnchorPosY(0, 0.5f);
        topUI.DOAnchorPosY(0, 0.5f);
        linePainter.enabled = true;
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Menu()
    {
        SceneManager.LoadScene("main");
    }
}
