using UnityEngine;

public class Exit : MonoBehaviour
{
    
        public int keycardCount = 0; // number of collected keycards

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Keycard"))
            {
                keycardCount++; // increase by 1
                Destroy(collision.gameObject); // remove the keycard
                Debug.Log("Keycard collected! Total: " + keycardCount);
            }
        }
    
}
