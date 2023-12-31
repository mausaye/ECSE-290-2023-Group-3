using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanel : MonoBehaviour
{
    public GameObject pauseMenu;

    public bool isPaused; 
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false); 
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                resumeGame(); 
            }
            else
            {
                pauseGame(); 
            }
        }
    }

    public void pauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true; 
    }

    public void resumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false; 
    }

    public void backToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Tutorial Scene"); 
    }

    public void quit()
    {
        Application.Quit(); 
    }
}
