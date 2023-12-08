using UnityEngine;
using System.Collections;
using System;

using System.Collections.Generic;
using JuiceFresh.Scripts.Integrations;
using UnityEngine.Advertisements;

using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Target
{
    SCORE,
    COLLECT,
    ITEMS,
    BLOCKS,
    CAGES,
    BOMBS,
}

public enum LIMIT
{
    MOVES,
    TIME
}

public enum Ingredients
{
    None = 0,
    Ingredient1,
    Ingredient2,
    Ingredient3,
    Ingredient4
}

public enum CollectItems
{
    None = 0,
    Item1,
    Item2,
    Item3,
    Item4,
    Item5,
    Item6
}

public enum CollectStars
{
    STAR_1 = 1,
    STARS_2 = 2,
    STARS_3 = 3
}

public enum RewardedAdsType
{
    GetLifes,
    GetGems,
    GetGoOn
}

public class InitScript : MonoBehaviour
{
    public static InitScript Instance;
    public static int openLevel;

    public static float RestLifeTimer;
    public static string DateOfExit;
    public static DateTime today;
    public static DateTime DateOfRestLife;
    public static string timeForReps;
    private static int Lifes;

    public List<CollectedIngredients> collectedIngredients = new List<CollectedIngredients>();

    public RewardedAdsType currentReward;

    public static int lifes
    {
        get { return InitScript.Lifes; }
        set { InitScript.Lifes = value; }
    }

    public int CapOfLife = 5;
    public float TotalTimeForRestLifeHours = 0;
    public float TotalTimeForRestLifeMin = 15;
    public float TotalTimeForRestLifeSec = 60;
    public int FirstGems = 20;
    public static int Gems;
    public static int waitedPurchaseGems;
    private int BoostExtraMoves;
    private int BoostPackages;
    private int BoostStripes;
    private int BoostExtraTime;
    private int BoostBomb;
    private int BoostColorful_bomb;
    private int BoostHand;
    private int BoostRandom_color;

    public static bool sound = false;
    public static bool music = false;
    private bool adsReady;
    public bool enableUnityAds;
    public bool enableGoogleMobileAds;
    public bool enableChartboostAds;
    public string rewardedVideoZone;
    public string nonRewardedVideoZone;
    public int ShowChartboostAdsEveryLevel;
    public int ShowAdmobAdsEveryLevel;
    private bool leftControl;

    public string admobUIDAndroid;
    public string admobUIDIOS;

    public int ShowRateEvery;
    public string RateURL;
    public string RateURLIOS;

    public int rewardedGems = 5;
    public bool losingLifeEveryGame;
    public static Sprite profilePic;
    public GameObject facebookButton;

    //1.3.3
    public string admobRewardedUIDAndroid;
    public string admobRewardedUIDIOS;

    // Use this for initialization
    void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
        RestLifeTimer = PlayerPrefs.GetFloat("RestLifeTimer");
        //		if (Application.isEditor)//TODO comment it
        //			PlayerPrefs.DeleteAll ();

        DateOfExit = PlayerPrefs.GetString("DateOfExit", "");
        Gems = PlayerPrefs.GetInt("Gems");
        lifes = PlayerPrefs.GetInt("Lifes");
        if (PlayerPrefs.GetInt("Lauched") == 0)
        { //First lauching
            lifes = CapOfLife;
            PlayerPrefs.SetInt("Lifes", lifes);
            Gems = FirstGems;
            PlayerPrefs.SetInt("Gems", Gems);
            PlayerPrefs.SetInt("Music", 1);
            PlayerPrefs.SetInt("Sound", 1);

            PlayerPrefs.SetInt("Lauched", 1);
            PlayerPrefs.Save();
        }
        GameObject.Find("Music").GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("Music");
        SoundBase.Instance.GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("Sound");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            leftControl = true;
        if (Input.GetKeyUp(KeyCode.LeftControl))
            leftControl = false;

        if (Input.GetKeyUp(KeyCode.U))
        {
            for (int i = 1; i < GameObject.Find("Levels").transform.childCount; i++)
            {
                SaveLevelStarsCount(i, 1);
            }
        }
    }

    public void SaveLevelStarsCount(int level, int starsCount)
    {
        Debug.Log(string.Format("Stars count {0} of level {1} saved.", starsCount, level));
        PlayerPrefs.SetInt(GetLevelKey(level), starsCount);
    }

    private string GetLevelKey(int number)
    {
        return string.Format("Level.{0:000}.StarsCount", number);
    }

    public void SetGems(int count)
    { //1.3.3
        Gems = count;
        PlayerPrefs.SetInt("Gems", Gems);
        PlayerPrefs.Save();
    }

    public void AddGems(int count)
    {
        Gems += count;
        PlayerPrefs.SetInt("Gems", Gems);
        PlayerPrefs.Save();
    }

    public void SpendGems(int count)
    {
        SoundBase.Instance.PlaySound(SoundBase.Instance.cash);
        Gems -= count;
        PlayerPrefs.SetInt("Gems", Gems);
        PlayerPrefs.Save();
    }

    public void RestoreLifes()
    {
        lifes = CapOfLife;
        PlayerPrefs.SetInt("Lifes", lifes);
        PlayerPrefs.Save();
    }

    public void AddLife(int count)
    {
        lifes += count;
        if (lifes > CapOfLife)
            lifes = CapOfLife;
        PlayerPrefs.SetInt("Lifes", lifes);
        PlayerPrefs.Save();
    }

    public int GetLife()
    {
        if (lifes > CapOfLife)
        {
            lifes = CapOfLife;
            PlayerPrefs.SetInt("Lifes", lifes);
            PlayerPrefs.Save();
        }
        return lifes;
    }

    public void PurchaseSucceded()
    {
        AddGems(waitedPurchaseGems);
        waitedPurchaseGems = 0;
    }

    public void SpendLife(int count)
    {
        if (lifes > 0)
        {
            lifes -= count;
            PlayerPrefs.SetInt("Lifes", lifes);
            PlayerPrefs.Save();
        }
        //else
        //{
        //    GameObject.Find("Canvas").transform.Find("RestoreLifes").gameObject.SetActive(true);
        //}
    }

    public void BuyBoost(BoostType boostType, int price, int count)
    {
        PlayerPrefs.SetInt("" + boostType, count);
        PlayerPrefs.Save();
#if PLAYFAB || GAMESPARKS
        NetworkManager.dataManager.SetBoosterData();
#endif

        //   ReloadBoosts();
    }

    public void SpendBoost(BoostType boostType)
    {
        PlayerPrefs.SetInt("" + boostType, PlayerPrefs.GetInt("" + boostType) - 1);
        PlayerPrefs.Save();
#if PLAYFAB || GAMESPARKS
        NetworkManager.dataManager.SetBoosterData();
#endif
    }

    //void ReloadBoosts()
    //{
    //    BoostExtraMoves = PlayerPrefs.GetInt("" + BoostType.ExtraMoves);
    //    BoostPackages = PlayerPrefs.GetInt("" + BoostType.Packages);
    //    BoostStripes = PlayerPrefs.GetInt("" + BoostType.Stripes);
    //    BoostExtraTime = PlayerPrefs.GetInt("" + BoostType.ExtraTime);
    //    BoostBomb = PlayerPrefs.GetInt("" + BoostType.Bomb);
    //    BoostColorful_bomb = PlayerPrefs.GetInt("" + BoostType.Colorful_bomb);
    //    BoostHand = PlayerPrefs.GetInt("" + BoostType.Hand);
    //    BoostRandom_color = PlayerPrefs.GetInt("" + BoostType.Random_color);

    //}
    //public void onMarketPurchase(PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra)
    //{
    //    PurchaseSucceded();
    //}

    void OnApplicationFocus(bool focusStatus)
    { //1.3.3
        if (MusicBase.Instance)
        {
            MusicBase.Instance.GetComponent<AudioSource>().Play();
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            if (RestLifeTimer > 0)
            {
                PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
            }
            PlayerPrefs.SetInt("Lifes", lifes);
            PlayerPrefs.SetString("DateOfExit", DateTime.Now.ToString());
            PlayerPrefs.Save();
        }
    }

    void OnApplicationQuit()
    { //1.4  added
        if (RestLifeTimer > 0)
        {
            PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
        }
        PlayerPrefs.SetInt("Lifes", lifes);
        PlayerPrefs.SetString("DateOfExit", DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    public void OnLevelClicked(object sender, LevelReachedEventArgs args)
    {
        if (EventSystem.current.IsPointerOverGameObject(-1))
            return;
        if (
            !GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.activeSelf
            && !GameObject.Find("CanvasGlobal").transform.Find("LiveShop").gameObject.activeSelf
        )
        {
            PlayerPrefs.SetInt("OpenLevel", args.Number);
            PlayerPrefs.Save();
            LevelManager.THIS.MenuPlayEvent();
            LevelManager.THIS.LoadLevel();
            openLevel = args.Number;
            //  currentTarget = targets[args.Number];
            GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.SetActive(true);
        }
    }

    void OnEnable()
    {
        LevelsMap.LevelSelected += OnLevelClicked;
    }

    void OnDisable()
    {
        LevelsMap.LevelSelected -= OnLevelClicked;

        //		if(RestLifeTimer>0){
        PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
        //		}
        PlayerPrefs.SetInt("Lifes", lifes);
        PlayerPrefs.SetString("DateOfExit", DateTime.Now.ToString());
        PlayerPrefs.Save();
#if GOOGLE_MOBILE_ADS
        interstitial.OnAdLoaded -= HandleInterstitialLoaded;
        interstitial.OnAdFailedToLoad -= HandleInterstitialFailedToLoad;
#endif
    }
}
