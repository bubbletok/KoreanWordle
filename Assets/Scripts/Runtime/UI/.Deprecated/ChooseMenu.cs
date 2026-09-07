using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using KW.Core;
using KW.Managers;

namespace KW.UI
{
    [System.Obsolete("This class is not used anymore. Use LoadSceneButton instead.", true)]
    public class ChooseMenu : MonoBehaviour
    {
        // [SerializeField] AudioClip clickSound;
        // public void GoToMain()
        // {
        //     if (clickSound != null && GetComponent<AudioSource>() != null)
        //     {
        //         GetComponent<AudioSource>().clip = clickSound;
        //         GetComponent<AudioSource>().Play();
        //     }
        //     SceneManager.LoadScene("Main");
        // }

        // public void GoToBlockedMain()
        // {
        //     if (clickSound != null && GetComponent<AudioSource>() != null)
        //     {
        //         GetComponent<AudioSource>().clip = clickSound;
        //         GetComponent<AudioSource>().Play();
        //     }
        //     GameManager.Instance.GameStageType = UnityEngine.Random.Range(1, 6);
        //     SceneManager.LoadScene("BlockedMain");

        // }

        // public void GoToDeblockedMain()
        // {
        //     if (clickSound != null && GetComponent<AudioSource>() != null)
        //     {
        //         GetComponent<AudioSource>().clip = clickSound;
        //         GetComponent<AudioSource>().Play();
        //     }
        //     GameManager.Instance.GameStageType = UnityEngine.Random.Range(1, 6);
        //     SceneManager.LoadScene("DeblockedMain");
        // }

        /*public void OneLetter()
        {
            SceneManager.LoadScene("Game1");
            GameManager.Instance.gameType = UnityEngine.Random.Range(1, 9);
        }

        public void TwoLetters()
        {
            SceneManager.LoadScene("Game2");
            GameManager.Instance.gameType = 2;
        }

        public void ThreeLetters()
        {
            SceneManager.LoadScene("Game3");
            GameManager.Instance.gameType = 3;
        }

        public void FourLetters()
        {
            SceneManager.LoadScene("Game4");
            GameManager.Instance.gameType = 4;
        }

        public void FiveLetters()
        {
            SceneManager.LoadScene("Game5");
            GameManager.Instance.gameType = 5;
        }
        public void TodayWord()
        {
            SceneManager.LoadScene("TodayWord");
            GameManager.Instance.gameType = 6;
        }*/

        // public void BlockedTutorial()
        // {
        //     if (clickSound != null && GetComponent<AudioSource>() != null)
        //     {
        //         GetComponent<AudioSource>().clip = clickSound;
        //         GetComponent<AudioSource>().Play();
        //     }
        //     SceneManager.LoadScene("BlockedTutorial");
        // }

        // public void DeblockedTutorial()
        // {
        //     if (clickSound != null && GetComponent<AudioSource>() != null)
        //     {
        //         GetComponent<AudioSource>().clip = clickSound;
        //         GetComponent<AudioSource>().Play();
        //     }
        //     SceneManager.LoadScene("DeblockedTutorial");
        // }

        // public void Stastics()
        // {
        //     SceneManager.LoadScene("Statistics");
        // }
        // public void ExitGame()
        // {
        //     Application.Quit();
        // }

        // public void ReGame()
        // {
        //     GameManager.Instance.GameStageType = UnityEngine.Random.Range(1, 6);
        //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // }
    }

}