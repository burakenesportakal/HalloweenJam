using UnityEngine;

public class CinemachineFlipFix : MonoBehaviour
{
    [Header("Cinemachine References")]
    [SerializeField] private MonoBehaviour virtualCamera; // CinemachineVirtualCamera veya CinemachineCamera2D
    
    [Header("Settings")]
    [SerializeField] private bool fixFollowTargetRotation = true; // Follow target'ın rotation'ını sabit tut
    [SerializeField] private bool fixCameraRotation = true; // Camera'nın rotation'ını sabit tut
    [SerializeField] private bool fixCameraScale = true; // Camera'nın scale'ini sabit tut (flip'ten etkilenmemesi için)
    
    private Transform followTarget;
    private Quaternion originalCameraRotation;
    private Vector3 originalCameraScale;
    private Camera actualCamera;
    
    private void Awake()
    {
        // Virtual Camera'ı otomatik bul (runtime'da tip kontrolü yapıyoruz)
        if (virtualCamera == null)
        {
            // Önce bu GameObject'te ara
            MonoBehaviour[] components = GetComponents<MonoBehaviour>();
            foreach (var comp in components)
            {
                string typeName = comp.GetType().Name;
                if (typeName.Contains("Cinemachine") && typeName.Contains("Camera"))
                {
                    virtualCamera = comp;
                    break;
                }
            }
        }
        
        // Eğer hala bulamadıysak, scene'de ara
        if (virtualCamera == null)
        {
            MonoBehaviour[] allComponents = FindObjectsOfType<MonoBehaviour>();
            foreach (var comp in allComponents)
            {
                string typeName = comp.GetType().Name;
                if (typeName.Contains("Cinemachine") && typeName.Contains("Camera"))
                {
                    virtualCamera = comp;
                    break;
                }
            }
        }
        
        if (virtualCamera == null)
        {
            Debug.LogWarning("CinemachineFlipFix: Cinemachine Camera component'i bulunamadı! Lütfen script'i Cinemachine Camera GameObject'ine ekleyin.");
            enabled = false;
            return;
        }
        
        // Actual camera'yı bul (Virtual Camera'nın child'ındaki Camera component)
        actualCamera = virtualCamera.GetComponentInChildren<Camera>();
        if (actualCamera == null)
        {
            actualCamera = Camera.main;
        }
        
        // Original camera rotation ve scale'i kaydet
        originalCameraRotation = virtualCamera.transform.rotation;
        originalCameraScale = virtualCamera.transform.localScale;
        
        // Follow target'ı al (reflection kullanarak)
        followTarget = GetFollowTarget(virtualCamera);
        
        Debug.Log($"CinemachineFlipFix: Virtual Camera bulundu! Type: {virtualCamera.GetType().Name}, Follow Target: {(followTarget != null ? followTarget.name : "None")}");
    }
    
    // Reflection kullanarak Follow target'ı al
    private Transform GetFollowTarget(MonoBehaviour cameraComponent)
    {
        try
        {
            System.Type type = cameraComponent.GetType();
            var followProperty = type.GetProperty("Follow");
            if (followProperty != null)
            {
                return followProperty.GetValue(cameraComponent) as Transform;
            }
            
            // Alternatif: Field olarak kontrol et
            var followField = type.GetField("m_Follow");
            if (followField != null)
            {
                return followField.GetValue(cameraComponent) as Transform;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"CinemachineFlipFix: Follow target alınamadı: {e.Message}");
        }
        
        return null;
    }
    
    // Follow target'ı set et (reflection kullanarak)
    private void SetFollowTarget(MonoBehaviour cameraComponent, Transform target)
    {
        try
        {
            System.Type type = cameraComponent.GetType();
            var followProperty = type.GetProperty("Follow");
            if (followProperty != null)
            {
                followProperty.SetValue(cameraComponent, target);
                return;
            }
            
            // Alternatif: Field olarak kontrol et
            var followField = type.GetField("m_Follow");
            if (followField != null)
            {
                followField.SetValue(cameraComponent, target);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"CinemachineFlipFix: Follow target set edilemedi: {e.Message}");
        }
    }
    
    private void LateUpdate()
    {
        if (virtualCamera == null) return;
        
        // Follow target'ın rotation'ını sabit tut
        if (fixFollowTargetRotation && followTarget != null)
        {
            // Follow target (player) enemy'nin child'ı olduğunda rotation'ı sıfırla
            if (followTarget.parent != null)
            {
                // Player enemy'nin child'ı ise rotation'ı sabit tut
                followTarget.localRotation = Quaternion.identity;
            }
        }
        
        // Virtual Camera rotation'ını sabit tut (2D için önemli)
        if (fixCameraRotation)
        {
            // 2D kameralar için Z rotation'ı 0'da tut
            Vector3 eulerAngles = virtualCamera.transform.rotation.eulerAngles;
            virtualCamera.transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, 0f);
        }
        
        // Camera scale'ini sabit tut (flip'ten etkilenmemesi için)
        if (fixCameraScale)
        {
            Vector3 currentScale = virtualCamera.transform.localScale;
            // Eğer scale negatif olmuşsa (flip edilmişse) düzelt
            if (currentScale.x < 0)
            {
                virtualCamera.transform.localScale = new Vector3(
                    Mathf.Abs(currentScale.x),
                    currentScale.y,
                    currentScale.z
                );
            }
            else
            {
                // Orijinal scale'i koru
                virtualCamera.transform.localScale = originalCameraScale;
            }
        }
        
        // Actual Camera rotation'ını da sabit tut
        if (actualCamera != null && fixCameraRotation)
        {
            Vector3 cameraEuler = actualCamera.transform.rotation.eulerAngles;
            actualCamera.transform.rotation = Quaternion.Euler(cameraEuler.x, cameraEuler.y, 0f);
        }
    }
    
    private void OnEnable()
    {
        // Enable olduğunda orijinal rotation ve scale'i tekrar kaydet
        if (virtualCamera != null)
        {
            originalCameraRotation = virtualCamera.transform.rotation;
            originalCameraScale = virtualCamera.transform.localScale;
            followTarget = GetFollowTarget(virtualCamera); // Reflection kullanarak al
            
            // Actual camera'yı tekrar bul
            actualCamera = virtualCamera.GetComponentInChildren<Camera>();
            if (actualCamera == null)
            {
                actualCamera = Camera.main;
            }
        }
    }
    
    // Follow target değiştiğinde çağrılabilir
    public void UpdateFollowTarget(Transform newTarget)
    {
        followTarget = newTarget;
        if (virtualCamera != null)
        {
            SetFollowTarget(virtualCamera, newTarget);
        }
    }
}

