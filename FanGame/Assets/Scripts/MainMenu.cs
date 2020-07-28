using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{

    public void PlayYellow()
    {
        PlayerPrefs.SetInt("Character Selected", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void PlayBortz()
    {
        PlayerPrefs.SetInt("Character Selected", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void PlayDia()
    {
        PlayerPrefs.SetInt("Character Selected", 2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }



}
