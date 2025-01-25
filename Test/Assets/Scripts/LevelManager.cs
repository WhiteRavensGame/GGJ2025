using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    void Start()
    {
        //works differently as the Level Manager parameters are different from the other managers
        if(Instance == null || Instance.gameObject.IsDestroyed() )
        {
            Instance = this;
            
        }
        else
        {
            Destroy(this.gameObject);
        }
        
    }

    

}
