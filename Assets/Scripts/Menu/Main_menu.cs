using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/*
 * @desc class managing the main menu
 * @author Denis
 */
public class Main_menu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject howToPlayMenu;

    /*
     * @desc launches the game
     */
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Time.timeScale = 1f;
        Game_state.isGamePaused = false;
    }

    /*
     * @desc shows game instructions
     */
    public void HowToPlay()
    {
        mainMenu.SetActive(false);
        howToPlayMenu.SetActive(true);
    }

    /*
     * @desc quits the application
     */
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    /*
     * @desc returns to main menu
     */
    public void Return()
    {
        mainMenu.SetActive(true);
        howToPlayMenu.SetActive(false);
    }
}
