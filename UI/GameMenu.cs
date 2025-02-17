using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public void Continue()
    {
        Time.timeScale = 1;
        Debug.Log("Continue");
        Destroy(gameObject);
    }
    public void Exit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
