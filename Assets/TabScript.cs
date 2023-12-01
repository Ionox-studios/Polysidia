using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabScript : MonoBehaviour
{
    public GameObject[] tabs;

    void Awake(){
        TurnOnTabs(1);
    }
    public void TurnOnTabs(int tab) 
    {
        for (int i = 0; i < tabs.Length; i++) 
        {
           tabs[i].SetActive(false);
        }       
        tabs[tab - 1]. SetActive(true);
    }

}
