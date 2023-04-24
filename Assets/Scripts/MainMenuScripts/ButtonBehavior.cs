using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonBehavior : MonoBehaviour
{
   public void loadScreen(string sceneName){
        SceneManager.LoadScene(sceneName);
   }

   public void Exit(){
      Application.Quit();
   }
}
