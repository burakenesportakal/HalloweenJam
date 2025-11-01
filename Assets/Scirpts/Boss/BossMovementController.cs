using UnityEngine;

namespace HalloweenJam.Boss
{
    /// <summary>
    /// Boss hareket sistemi - İleri-geri gidip gelme (patrolling)
    /// </summary>
    public class BossMovementController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float patrolDistance = 5f; // İleri-geri mesafe
        [SerializeField] private float changeDirectionTime = 2f; // Yön değiştirme süresi

        [Header("Movement Bounds")]
        [SerializeField] private Transform leftBound;
        [SerializeField] private Transform rightBound;
        [SerializeField] private bool useCustomBounds = false;

        private Transform playerTarget;
        private Rigidbody2D rb;

        private bool isMoving = false;
        private bool movingRight = true;
        private float lastDirectionChange = 0f;
        private Vector2 startPosition;

        public void Initialize(Transform player)
        {
            playerTarget = player;

            // Rigidbody kontrolü
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f; // 2D platformer değilse
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

            startPosition = transform.position;

            // Bounds kontrolü
            if (!useCustomBounds && (leftBound == null || rightBound == null))
            {
                // Otomatik bounds oluştur
                CreateAutomaticBounds();
            }
        }

        private void CreateAutomaticBounds()
        {
            // Start position'ın etrafında bounds oluştur
            GameObject leftBoundObj = new GameObject("LeftBound");
            leftBoundObj.transform.position = new Vector2(startPosition.x - patrolDistance, startPosition.y);
            leftBound = leftBoundObj.transform;

            GameObject rightBoundObj = new GameObject("RightBound");
            rightBoundObj.transform.position = new Vector2(startPosition.x + patrolDistance, startPosition.y);
            rightBound = rightBoundObj.transform;
        }

        public void UpdateMovement()
        {
            if (!isMoving)
                return;

            // İleri-geri hareket
            Vector2 targetPosition = movingRight ? rightBound.position : leftBound.position;
            
            // Mesafe kontrolü
            float distance = Vector2.Distance(transform.position, targetPosition);
            
            if (distance < 0.5f)
            {
                // Hedefe ulaşıldı, yön değiştir
                ChangeDirection();
            }
            else
            {
                // Hareket
                Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
                rb.linearVelocity = direction * moveSpeed;

                // Sprite flip (sola/sağa bakış)
                if (direction.x > 0)
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                else if (direction.x < 0)
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

            // Zaman bazlı yön değiştirme (player'ı sıkıştırmamak için)
            if (Time.time >= lastDirectionChange + changeDirectionTime)
            {
                ChangeDirection();
                lastDirectionChange = Time.time;
            }
        }

        public void StartMovement()
        {
            isMoving = true;
            lastDirectionChange = Time.time;
        }

        public void StopMovement()
        {
            isMoving = false;
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }

        private void ChangeDirection()
        {
            movingRight = !movingRight;
            lastDirectionChange = Time.time;
        }

        public void SetBounds(Transform left, Transform right)
        {
            leftBound = left;
            rightBound = right;
            useCustomBounds = true;
        }

        public bool IsMoving()
        {
            return isMoving;
        }
    }
}

