using UnityEngine;

public class CameraFlipFix : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool fixRotation = true; // Rotation'ı sabit tut
    [SerializeField] private bool fixScale = true; // Scale'i sabit tut (flip engellemek için)
    
    private Vector3 originalLocalScale;
    private Quaternion originalLocalRotation;
    
    private void Awake()
    {
        // Orijinal local rotation ve scale'i kaydet
        originalLocalRotation = transform.localRotation;
        originalLocalScale = transform.localScale;
    }
    
    private void LateUpdate()
    {
        // Her frame sonunda rotation ve scale'i sıfırla/sabit tut
        // Bu sayede parent flip attığında kamera etkilenmez
        
        if (fixRotation)
        {
            transform.localRotation = originalLocalRotation;
        }
        
        if (fixScale)
        {
            // Scale'i sabit tut (flip'i engelle)
            // X scale'i negatif olursa kamera ters döner, bunu engelle
            Vector3 currentScale = transform.localScale;
            
            // Eğer parent flip attıysa (scale.x negatif olduysa), scale'i düzelt
            if (currentScale.x < 0)
            {
                transform.localScale = new Vector3(
                    Mathf.Abs(currentScale.x),
                    currentScale.y,
                    currentScale.z
                );
            }
            else
            {
                // Orijinal scale'i koru
                transform.localScale = originalLocalScale;
            }
        }
    }
    
    private void OnEnable()
    {
        // Enable olduğunda orijinal değerleri tekrar kaydet
        originalLocalRotation = transform.localRotation;
        originalLocalScale = transform.localScale;
    }
}

