using System;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            LevelManager.Instance.StartTimer(false);
            gameObject.SetActive(false);
        }
            
    }
}
