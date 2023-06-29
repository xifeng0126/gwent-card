﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(6);
    }

    public void EditDeck()
    {
        SceneManager.LoadScene(5);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting...");
        SceneManager.LoadScene(3);
    }
}
