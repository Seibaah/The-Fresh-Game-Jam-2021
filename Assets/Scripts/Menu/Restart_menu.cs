using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * @desc menu that shows up when game is over
 * for testing purposes it can be enabled by pressing F1
 * @author Denis
 */
public class Restart_menu : MonoBehaviour
{
    public GameObject restartMenuUI;

    /*
     * @desc press F1 to access menu
     */
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) && !Game_state.isGamePaused)
        {
            GameOver();
        }
    }

    /*
    * @desc loads the game over menu
    */
    void GameOver()
    {
        restartMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Game_state.isGameOver = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /*
     * @desc button that restarts the game without
     * taking the player back to the main menu
     */
    public void Restart()
    {
        SceneManager.LoadScene(1);

        restartMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Game_state.isGameOver = false;
    }

    /*
    * @desc loads main menu. No progress is saved.
    */
    public void LoadMenu()
    {
        SceneManager.LoadScene(0);

        restartMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Game_state.isGameOver = false;
        Game_state.isGamePaused = true;
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
