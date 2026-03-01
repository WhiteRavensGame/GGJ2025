using UnityEngine;

public class EnergyPickup : MonoBehaviour
{
    [SerializeField] private float energyRecoverPercentAmt = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if (collision.tag == "Player")
       {
            Ball bPlayer = collision.gameObject.GetComponent<Ball>();
            bPlayer.RecoverEnergyByPercent(energyRecoverPercentAmt);
            gameObject.SetActive(false);
       }

    }
}
