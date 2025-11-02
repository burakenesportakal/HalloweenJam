using UnityEngine;

public class ConeLightController : MonoBehaviour
{
    [Header("Target Object")]
    [SerializeField] private GameObject targetObject; // Öldüğünde destroy edilecek GameObject

    [Header("Character Reference")]
    [SerializeField] private Entity characterEntity; // Enemy veya Player

    [Header("Settings")]
    [SerializeField] private bool destroyOnDeath = true;

    private void Awake()
    {
        // Eğer targetObject atanmamışsa, bu GameObject'i kullan
        if (targetObject == null)
        {
            targetObject = gameObject;
        }

        // Eğer characterEntity atanmamışsa, parent'ta veya kendinde ara
        if (characterEntity == null)
        {
            characterEntity = GetComponentInParent<Entity>();
            if (characterEntity == null)
            {
                characterEntity = GetComponent<Entity>();
            }
        }

        if (characterEntity == null)
        {
            Debug.LogWarning($"ConeLightController: Entity component bulunamadı! GameObject: {gameObject.name}");
        }

        if (targetObject == null)
        {
            Debug.LogWarning($"ConeLightController: Target Object atanmamış! GameObject: {gameObject.name}");
        }
    }

    private void Update()
    {
        if (targetObject == null || characterEntity == null) return;

        // Current health kontrolü
        int currentHealth = characterEntity.GetCurrentHealth();

        // Health 0 veya altındaysa GameObject'i yok et
        if (currentHealth <= 0 && destroyOnDeath)
        {
            Destroy(targetObject);
            // Bu script de yok olacak çünkü targetObject yok oldu
        }
    }

    // Inspector'da test için
    private void OnValidate()
    {
        if (targetObject == null)
        {
            targetObject = gameObject;
        }

        if (characterEntity == null)
        {
            characterEntity = GetComponentInParent<Entity>();
            if (characterEntity == null)
            {
                characterEntity = GetComponent<Entity>();
            }
        }
    }
}
