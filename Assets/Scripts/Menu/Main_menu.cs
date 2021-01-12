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
    /*
     * @desc launches the game
     */
   public static void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Time.timeScale = 1f;
        Game_state.isGamePaused = false;
    }

    /*
     * @desc quits the application
     */
    public static void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
