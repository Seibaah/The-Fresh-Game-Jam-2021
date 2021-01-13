using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * @desc class holding game state attributes
 * @author Denis
 */
public static class Game_state
{
    public static bool isGamePaused = false;
    public static bool isGameOver = false;
}

/*
 * @desc class that manages the in-game pause menu
 * @author Denis
 */
public class Pause_menu : MonoBehaviour
{
    public GameObject pauseMenuUI;

    /*
     * @desc activate/deactivate pause menu with Esc key
     */
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !Game_state.isGameOver)
        {
            if (Game_state.isGamePaused) Resume();
            else Pause();
        }
    }

    /*
     * @desc allows to resume play by clikcing on the resume button
     */
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Game_state.isGamePaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /*
     * @desc pauses the game and enables the menu
     */
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Game_state.isGamePaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /*
     * @desc loads main menu. No progress is saved.
     */
    public void LoadMenu()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Game_state.isGamePaused = true;

        SceneManager.LoadScene(0);
    }

    /*
     * @desc quits the application
     */
    public static void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    /*
     * @desc game state runtime tracker
     * 
    public void OnGUI()
    {
        GUI.color = new Color(1, 0, 0, 1);

        GUI.Label(new Rect(10, 10, 300, 50), "Pause menu active " + pauseMenuUI.activeSelf);
        GUI.Label(new Rect(10, 100, 300, 50), "Time Scale " + Time.timeScale);
        GUI.Label(new Rect(10, 200, 300, 50), "Is Game paused " + Game_state.isGamePaused);
    }
    */
}
