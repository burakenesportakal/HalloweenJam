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
            // Move player to the center of the hiding spot
            transform.position = hidingSpot.GetComponent<Collider2D>().bounds.center;
        }
    }
}


