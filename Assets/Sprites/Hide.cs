using UnityEngine;

public class Hide : MonoBehaviour
{


    private GameObject hidingSpot;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hidingspot"))
        {
            hidingSpot = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Hidingspot"))
        {
            hidingSpot = null;
        }
    }

    private void Update()
    {
        if (hidingSpot != null && Input.GetKeyDown(KeyCode.X))
        {
            PlayerController player = GetComponent<PlayerController>();
            
            // ÖNEMLİ: Player enemy kontrol ediyorsa pozisyon değiştirme
            if (player != null && player.IsControllingEnemy())
            {
                Debug.Log("Player enemy kontrol ediyor, gizlenme pozisyonu değiştirilemez!");
                return; // Player enemy kontrol ediyor, pozisyon değiştirme işlemi yapılmasın
            }
            
            // Move player to the center of the hiding spot
            transform.position = hidingSpot.GetComponent<Collider2D>().bounds.center;
        }
    }
}


