using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject credits;
    [SerializeField] GameObject buttons;


   public void playGame(){
        SceneManager.LoadScene(2);
   }

   public void playTutorial(){
        SceneManager.LoadScene(1);
   }

   public void showCredits(){
        credits.SetActive(true);
        buttons.SetActive(false);
   }

   public void BackToMenu(){
        credits.SetActive(false);
        buttons.SetActive(true);
   }

   public void QuitGame(){
        Application.Quit();
   }
}
