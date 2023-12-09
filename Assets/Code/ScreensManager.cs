using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreensManager : MonoBehaviour
{
    [SerializeField] private Image screenCurtain;
    [SerializeField] private GameObject gameScreen, menuScreen;
    private Color transparentColor, normalColor;
    private ScreenState currentScreen;
    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        normalColor = screenCurtain.color;
        transparentColor = normalColor * new Color(1,1,1,0);
        screenCurtain.color = transparentColor;
        currentScreen = ScreenState.Menu;
        ChangeScreen();
    }
    
    private enum ScreenState
    {
        Menu,
        Game
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && currentScreen == ScreenState.Game)
        {
            GoToMenu();
        }
    }

    public void StartGame()
    {
        currentScreen = ScreenState.Game;
        StopAllCoroutines();
        StartCoroutine(ChangeScreenAnimation());
    }

    public void GoToMenu()
    {
        currentScreen = ScreenState.Menu;
        StopAllCoroutines();
        StartCoroutine(ChangeScreenAnimation());
    }

    IEnumerator ChangeScreenAnimation()
    {

        Color tempColor = transparentColor;
        for(int i = 0; i < 10; i++ )
        {
            tempColor.a+=0.1f;
            screenCurtain.color = tempColor;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        
        ChangeScreen();
        yield return new WaitForSeconds(0.1f);
        
        for(int i = 0; i < 10; i++ )
        {
            tempColor.a-=0.1f;
            screenCurtain.color = tempColor;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

    }    

    private void ChangeScreen()
    {
        gameScreen.SetActive(false);
        menuScreen.SetActive(false);
        gameScreen.SetActive(currentScreen == ScreenState.Game);
        menuScreen.SetActive(currentScreen == ScreenState.Menu);
    }




}
