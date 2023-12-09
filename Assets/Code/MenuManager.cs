using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private RectTransform soundDot, vibroDot;
    [SerializeField] private List<RectTransform> redDots, greenDots, uiRedDots;
    [SerializeField] private MiniLineRenderer uiLr, settingsGreenLr, settingsRedLr;
    [SerializeField] private ScreensManager screensManager;
    [SerializeField] private TextMeshProUGUI best;
    private Image soundDotImage, vibroDotImage;
    private string sound = "SoundDot";
    private string vibro = "VibroDot";
    private void Start()
    {
        uiLr.ConnectDots(Color.red, uiRedDots);
        InitialSettings();
    }

    private void OnEnable()
    {
        if(PlayerPrefs.HasKey("BestScore"))
        {
            best.text = "Best: \n" + PlayerPrefs.GetInt("BestScore");
        }
    }

    private void InitialSettings()
    {
        if(!PlayerPrefs.HasKey(sound))
        {
            PlayerPrefs.SetString(sound, "Green");
            PlayerPrefs.SetString(vibro, "Green");
            PlayerPrefs.Save();
        }
        ConnectSoundDots();
        ConnectVibroDots();
    }

    public void ChangeSoundSettings()
    {
        PlayerPrefs.SetString(sound, PlayerPrefs.GetString(sound) == "Red"? "Green" : "Red");
        PlayerPrefs.Save();
        ConnectSoundDots();
    }

    public void ChangeVibroSettings()
    {
        PlayerPrefs.SetString(vibro, PlayerPrefs.GetString(vibro) == "Red"? "Green" : "Red");
        PlayerPrefs.Save();
        ConnectVibroDots();
    }

    private void ConnectSoundDots()
    {
        if(PlayerPrefs.GetString(sound) == "Green")
        {
            redDots.Remove(soundDot);
            greenDots.Insert(0, soundDot);
            settingsGreenLr.ConnectDots(Color.green, greenDots);
        }
        else
        {
            greenDots.Remove(soundDot);
            redDots.Insert(0, soundDot);
            settingsGreenLr.ConnectDots(Color.red, redDots);
        }
    }

    private void ConnectVibroDots()
    {
        if(PlayerPrefs.GetString(vibro) == "Green")
        {
            redDots.Remove(vibroDot);
            greenDots.Add(vibroDot);
            settingsGreenLr.ConnectDots(Color.green, greenDots);
        }
        else
        {
            greenDots.Remove(vibroDot);
            redDots.Add(vibroDot);
            settingsGreenLr.ConnectDots(Color.red, redDots);
        }
    }

    public void StartGame()
    {
        screensManager.StartGame();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
