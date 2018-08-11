using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preload : MonoBehaviour
{
    void Awake()
    {
        Screen.fullScreen = false;
        SceneManager.LoadScene("Game");
    }
}
