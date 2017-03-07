using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour {
    
    public GameObject pauseMenu;
    bool isPaused = false;

	// Use this for initialization
	void Start () {
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            Pause_action(isPaused);
            pauseMenu.SetActive(isPaused);
        }
    }
    public void Pause_action(bool pause = false)
    {
        if (!pause)
        {
            Time.timeScale = 1;
        }
        else
        {
        Time.timeScale = 0;
        }
    }
    public void Quit()
    {
        Application.Quit();
    }
}
