using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour
{
    public GameObject creditUi;
    public void QuizSwich()
    {
        SceneManager.LoadScene("StageSelect");
    }

    public void Logout()
    {
        SceneManager.LoadScene("Login");
    }

    public void QuizData()
    {
        SceneManager.LoadScene("DataSelect");
    }

    public void Credits()
    {
        creditUi.SetActive(true);
    }

    public void CreditsHome()
    {
        creditUi.SetActive(false);
    }

}
