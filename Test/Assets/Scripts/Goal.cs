using System;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            GameManager.Instance.CompleteLevel();
            gameObject.SetActive(false);
        }
            
    }
}
