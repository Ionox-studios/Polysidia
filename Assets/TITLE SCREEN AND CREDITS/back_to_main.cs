using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class back_to_main : MonoBehaviour
{
   public void Playgame()
   {
     Debug.Log("ButtonClicked");
        SceneManager.LoadScene("MainMenu");
   }
    public void QuitGame()
   {
    Application.Quit();
   }
 }
   