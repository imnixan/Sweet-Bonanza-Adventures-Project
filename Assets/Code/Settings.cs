using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class Settings : MonoBehaviour
{
    [SerializeField]
    private Image soundImage,
        musicImage;

    [SerializeField]
    private TextMeshProUGUI timerText;

    [SerializeField]
    private GameObject buyButton,
        getFreeButton;

    [SerializeField]
    private RectTransform settingsBar;

    [SerializeField]
    private Sprite[] soundStatus,
        musicStatus;

    [SerializeField]
    private RectTransform shop;

    [SerializeField]
    private TextMeshProUGUI scoresLeft;

    private void Start()
    {
        timerText.gameObject.SetActive(false);
        getFreeButton.SetActive(false);
        settingsBar.sizeDelta = new Vector2(189, 0);
        soundImage.sprite = soundStatus[PlayerPrefs.GetInt("SoundSettings", 1)];
        musicImage.sprite = musicStatus[PlayerPrefs.GetInt("MusicSettings", 1)];
    }

    public void ChangeSound()
    {
        PlayerPrefs.SetInt("SoundSettings", PlayerPrefs.GetInt("SoundSettings", 1) == 1 ? 0 : 1);
        PlayerPrefs.Save();
        soundImage.sprite = soundStatus[PlayerPrefs.GetInt("SoundSettings", 1)];
    }

    public void ChangeMusic()
    {
        PlayerPrefs.SetInt("MusicSettings", PlayerPrefs.GetInt("MusicSettings", 1) == 1 ? 0 : 1);
        PlayerPrefs.Save();
        musicImage.sprite = musicStatus[PlayerPrefs.GetInt("MusicSettings", 1)];
    }

    public void OpenSettings()
    {
        if (settingsBar.sizeDelta.y == 0)
        {
            settingsBar.DOSizeDelta(new Vector2(189, 644), 0.5f);
        }
        if (settingsBar.sizeDelta.y == 644)
        {
            settingsBar.DOSizeDelta(new Vector2(189, 0), 0.5f);
        }
    }

    public void PlayGame()
    {
        if (PlayerPrefs.GetInt("Lives", 10) > 0)
        {
            SceneManager.LoadScene("ColorLine");
        }
        else
        {
            scoresLeft.text = PlayerPrefs.GetInt("Scores").ToString();
            shop.DOAnchorPosX(0, 0.5f);
            if (PlayerPrefs.GetInt("Scores") < 500)
            {
                buyButton.SetActive(false);
                timerText.gameObject.SetActive(true);
                StartCoroutine(WaitFreeLive());
            }
            else
            {
                buyButton.SetActive(true);
                timerText.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator WaitFreeLive()
    {
        int secondsLeft = 59;

        while (secondsLeft > 0)
        {
            secondsLeft--;
            timerText.text = secondsLeft.ToString();
            yield return new WaitForSecondsRealtime(1);
        }

        timerText.gameObject.SetActive(false);
        getFreeButton.SetActive(true);
    }

    public void GetFree()
    {
        PlayerPrefs.SetInt("Lives", 1);
        PlayerPrefs.Save();
        CloseShop();
    }

    public void BuyLive()
    {
        if (PlayerPrefs.GetInt("Scores") >= 500)
        {
            PlayerPrefs.SetInt("Scores", PlayerPrefs.GetInt("Scores") - 500);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("Lives", 1);
            PlayerPrefs.Save();
            CloseShop();
        }
    }

    public void CloseShop()
    {
        StopAllCoroutines();
        shop.DOAnchorPosX(2000, 0.5f);
    }
}
