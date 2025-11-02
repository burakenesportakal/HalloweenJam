# UI ve Oyun Sahnesi Entegrasyon Rehberi

Bu rehber, UI sistemini oyununuzun sahne yapısına nasıl entegre edeceğinizi adım adım anlatır.

---

## 🎯 SAHNE YAPISI

Oyununuz 3 sahneye sahip:

1. **Intro Sahnesi** (Başlangıç) - Intro animasyonları, logo vs.
2. **UI Sahnesi (Menu Scene)** - Ana menü, ayarlar, storyboard
3. **Oyun Sahnesi (Game Scene)** - Oyun içi UI (Pause, Health)

### UI Bölünmesi:

**UI Sahnesinde:**
- ✅ MainMenuPanel
- ✅ SettingsPanel
- ✅ StoryboardPanel
- ❌ PausePanel (oyun sahnesinde)
- ❌ HealthUI (oyun sahnesinde)

**Oyun Sahnesinde:**
- ❌ MainMenuPanel (UI sahnesinde)
- ❌ SettingsPanel (UI sahnesinde)
- ❌ StoryboardPanel (UI sahnesinde)
- ✅ PausePanel
- ✅ HealthUI

---

## 📋 ADIM 1: UI Sahnesinde Prefab Oluşturma (Menu Scene)

### 1.1 PausePanel ve HealthUI'yi Prefab Yapma

1. **UI sahnesini açın** (UI.unity)
2. **Project** panelinde → **Prefabs** klasörü oluşturun (yoksa)
3. **MainCanvas** altında:
   - **PausePanel** GameObject'ini seçin → Project paneline **sürükleyin** → Prefab oluşur
   - **HealthUI** GameObject'ini seçin → Project paneline **sürükleyin** → Prefab oluşur

4. Prefab isimleri:
   - **PausePanelPrefab**
   - **HealthUIPrefab**

### 1.3 Prefab'ları UI Sahnesinden Gizleme (Opsiyonel)

1. **UI sahnesinde** prefab'lar görünür olabilir (mavi renk - prefab instance)
2. Oyun içinde görünmemeleri için:
   - **PausePanelPrefab** instance → Inspector'da **GameObject aktiflik checkbox: ❌ TİKSİZ**
   - **HealthUIPrefab** instance → Inspector'da **GameObject aktiflik checkbox: ❌ TİKSİZ**

3. **UIManager** GameObject'ini seçin → Inspector'da:
   - **Pause Panel** referansını **boşaltın** (None) - UI sahnesinde kullanılmayacak
   - **Health UI** referansını **boşaltın** (None) - UI sahnesinde kullanılmayacak

### 1.4 UI Sahnesi Yapısı

**MainCanvas** altında şunlar olmalı:
```
MainCanvas
├── GameManager (DontDestroyOnLoad)
├── UIManager
├── MainMenuPanel
├── SettingsPanel
├── StoryboardPanel
├── PausePanelPrefab (instance - pasif/gizli)
└── HealthUIPrefab (instance - pasif/gizli)
```

**NOT:** PausePanelPrefab ve HealthUIPrefab UI sahnesinde görünür olabilir ama **pasif** (aktif değil) olmalı. Oyun sahnesinde aktif olacaklar.

---

## 📋 ADIM 2: Oyun Sahnesinde UI Oluşturma (Game Scene)

### 2.1 GameCanvas Oluşturma

1. **Oyun sahnenizi açın** (GameScene.unity veya ne ise)
2. **Hierarchy** → Sağ tık → **UI → Canvas** → İsmi: **"GameCanvas"**
3. **GameCanvas** ayarları:
   - **Render Mode**: Screen Space - Overlay
   - **Sort Order**: 0 (veya istediğiniz değer)

### 2.2 PausePanel Prefab'ını Eklemek

1. **GameCanvas** altında → **Prefabs klasöründen** → **PausePanelPrefab**'ı **sürükle-bırak**
2. **PausePanelPrefab** instance'ı aktif olmalı → Inspector'da **GameObject aktiflik checkbox: ✅ TİKLI**
3. **PauseMenuManager** script'i zaten prefab'da var, kontrol edin:
   - Tüm buton referansları bağlı olmalı
   - Gerekirse Inspector'da kontrol edin ve bağlayın

### 2.3 HealthUI Prefab'ını Eklemek

1. **GameCanvas** altında → **Prefabs klasöründen** → **HealthUIPrefab**'ı **sürükle-bırak**
2. **HealthUIPrefab** instance'ı aktif olmalı → Inspector'da **GameObject aktiflik checkbox: ✅ TİKLI**
3. **HealthUI.cs** script'i zaten prefab'da var, kontrol edin:
   - Health Icons array'i dolu olmalı (3 kalp ikonu)
   - Full Heart Sprite ve Empty Heart Sprite atanmış olmalı
   - Gerekirse Inspector'da kontrol edin ve atayın

### 2.4 UIManager Eklemek

1. **GameCanvas** altında → Sağ tık → **Create Empty** → İsmi: **"UIManager"**
2. **UIManager.cs** script'ini ekleyin
3. Inspector'da:
   - **Pause Panel** → PausePanel GameObject'ini ata
   - **Health UI** → HealthUI GameObject'ini ata
   - **Main Menu Panel** → **BOŞ** (None)
   - **Settings Panel** → **BOŞ** (None)
   - **Storyboard Panel** → **BOŞ** (None)
   - **Main Canvas** → GameCanvas GameObject'ini ata

### 2.5 Oyun Sahnesi Yapısı

**GameCanvas** altında şunlar olmalı:
```
GameCanvas
├── UIManager
├── PausePanel
│   └── PauseMenuManager (script)
└── HealthUI
    └── HealthUI (script)
```

---

## 📋 ADIM 3: GameManager'ı Oyun Sahnesine Ekleme

GameManager DontDestroyOnLoad kullanıyor, bu yüzden:

### 3.1 Seçenek 1: GameManager'ı UI Sahnesinde Bırakma (Önerilen)

1. **UI sahnesinde** GameManager zaten var
2. GameManager **DontDestroyOnLoad** ile oyun sahnesine geçince de aktif kalır
3. Oyun sahnesinde ayrı bir GameManager'a **gerek yok**

### 3.2 Seçenek 2: GameManager'ı Her Sahneye Ekleme

Eğer her sahne için ayrı GameManager isterseniz:

1. **Oyun sahnesinde** → Sağ tık → **Create Empty** → İsmi: **"GameManager"**
2. **GameManager.cs** script'ini ekleyin
3. **Singleton pattern** sayesinde UI sahnesindeki GameManager destroy edilir

**ÖNERİ:** Seçenek 1'i kullanın - GameManager UI sahnesinde kalsın.

---

## 📋 ADIM 4: Sahne Geçişleri

### 4.1 Build Settings

1. **File** → **Build Settings**
2. **Scenes In Build** listesine ekleyin:
   - **Index 0**: **Intro.unity** (Intro sahnesi)
   - **Index 1**: **UI.unity** (Menu sahnesi)
   - **Index 2**: **GameScene.unity** (Oyun sahnesi)

### 4.2 Intro → Menu Geçişi

**Intro sahnesinden** ana menüye geçiş için:

```csharp
// Intro bittiğinde (örnek script)
UnityEngine.SceneManagement.SceneManager.LoadScene(1); // UI sahnesi
```

### 4.3 Storyboard → Game Scene Geçişi

**GameManager.cs**'te `StartGame()` metodunu güncelleyin:

```csharp
/// <summary>
/// Oyunu başlatır (storyboard'dan sonra)
/// </summary>
public void StartGame()
{
    CurrentState = GameState.Playing;
    Time.timeScale = 1f;

    // Tüm menü panellerini gizle
    if (UIManager.Instance != null)
        UIManager.Instance.HideAllPanels();

    // Oyun sahnesine geç
    UnityEngine.SceneManagement.SceneManager.LoadScene(2); // Game Scene build index
    
    Debug.Log("Game Started!");
}
```

### 4.4 Pause Menu → Main Menu (Ana Menü Sahnesine Dönüş)

**GameManager.cs**'te `ReturnToMainMenu()` metodunu güncelleyin:

```csharp
/// <summary>
/// Ana menüye döner (pause menüsünden)
/// </summary>
public void ReturnToMainMenu()
{
    CurrentState = GameState.MainMenu;
    Time.timeScale = 1f;

    if (UIManager.Instance != null)
    {
        UIManager.Instance.HidePauseMenu();
        // Ana menüyü göster (UI sahnesine geçince otomatik gösterilecek)
    }

    // UI sahnesine dön
    UnityEngine.SceneManagement.SceneManager.LoadScene(1); // UI sahnesi
    
    // Oyun durumunu sıfırla
    ResetGame();
}
```

**ÖNEMLİ:** UI sahnesine döndüğünüzde, GameManager zaten DontDestroyOnLoad ile korunuyor. UI sahnesindeki UIManager otomatik olarak `ShowMainMenu()` çağırılacak (GameManager.Start()'ta).

---

## 📋 ADIM 5: Oyun İçi Sistemlerle Entegrasyon

### 5.1 Health Sistemi Entegrasyonu

Oyuncu health script'inizden:

```csharp
using UnityEngine;
using HalloweenJam.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        // Oyun sahnesindeki UIManager'a bildir
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }

    private void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDeath();
        }
    }
}
```

### 5.2 Pause Sistemi

**ESC tuşu** zaten GameManager'da çalışıyor. Oyun sahnesinde:
- **ESC** → PauseMenu açılır
- **Resume** → Oyun devam eder
- **Ana Menüye Dön** → UI sahnesine döner

---

## 📋 ADIM 6: GameManager'ı Scene Yüklendiğinde Kontrol Etme

GameManager'ın her sahne değişiminde doğru çalışması için:

**GameManager.cs**'e ekleyin:

```csharp
using UnityEngine.SceneManagement;

private void OnEnable()
{
    // Sahne yüklendiğinde
    SceneManager.sceneLoaded += OnSceneLoaded;
}

private void OnDisable()
{
    SceneManager.sceneLoaded -= OnSceneLoaded;
}

private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    // UI sahnesinde ana menüyü göster
    if (scene.buildIndex == 1) // UI sahnesi
    {
        ShowMainMenu();
    }
    // Oyun sahnesinde oyunu başlat
    else if (scene.buildIndex == 2) // Game sahnesi
    {
        StartGame();
    }
}
```

---

## 📋 ADIM 7: Test Checklist

### UI Sahnesi:
- [ ] MainMenuPanel görünüyor mu?
- [ ] Settings butonu çalışıyor mu?
- [ ] Play butonu Storyboard'u açıyor mu?
- [ ] Storyboard'dan sonra oyun sahnesine geçiyor mu?

### Oyun Sahnesi:
- [ ] HealthUI görünüyor mu? (sol üstte 3 kalp)
- [ ] ESC tuşu PauseMenu açıyor mu?
- [ ] Resume butonu çalışıyor mu?
- [ ] Ana Menüye Dön butonu UI sahnesine dönüyor mu?
- [ ] Health sistemi UI'ya haber veriyor mu?

---

## 🎯 ÖZET: Yapılacaklar Listesi

1. ✅ **UI Sahnesinde**: PausePanel ve HealthUI'yi **Prefab yapın**
2. ✅ **UI Sahnesinde**: Prefab instance'larını **pasif yapın** (görünmez)
3. ✅ **UI Sahnesinde**: UIManager'dan PausePanel ve HealthUI referanslarını **boşaltın**
4. ✅ **Oyun Sahnesinde**: GameCanvas oluşturun
5. ✅ **Oyun Sahnesinde**: Prefab'lardan PausePanel ve HealthUI'yi ekleyin
6. ✅ **Oyun Sahnesinde**: Prefab instance'larını **aktif yapın**
7. ✅ **Oyun Sahnesinde**: UIManager ekleyin ve referansları atayın
8. ✅ **Build Settings**: Sahne sıralamasını ayarlayın (Intro: 0, UI: 1, Game: 2)
9. ✅ **Player Health Script**: UIManager.UpdateHealth() çağrısı ekleyin

### 💡 Prefab Avantajları:
- ✅ Aynı yapıyı iki sahneye ekleyebilirsiniz
- ✅ Bir yerden değişiklik yapın, her yerde güncellenir
- ✅ Referanslar prefab'da korunur
- ✅ Daha düzenli ve bakımı kolay

---

## ⚠️ ÖNEMLİ NOTLAR

1. **GameManager DontDestroyOnLoad**: UI sahnesinden oyun sahnesine geçerken GameManager korunur
2. **UIManager Her Sahnede Ayrı**: UI sahnesinde MenuUIManager, oyun sahnesinde GameUIManager
3. **Singleton Pattern**: Her sahne değişiminde yeni UIManager oluşur (önceki destroy edilir)
4. **Null Kontrolü**: UIManager metodları null kontrolü yapar, olmayan paneller için hata vermez

---

## 🔗 Örnek Sahne Akışı

```
1. Intro Sahnesi (Index 0)
   └─> Otomatik veya buton ile
   
2. UI Sahnesi (Index 1)
   ├─> MainMenuPanel görünür
   ├─> Play butonu → StoryboardPanel
   ├─> Storyboard sonu → GameManager.StartGame()
   └─> Oyun Sahnesine geç (Index 2)
   
3. Oyun Sahnesi (Index 2)
   ├─> HealthUI görünür (sol üstte)
   ├─> ESC tuşu → PausePanel
   ├─> Resume → Oyun devam
   └─> Ana Menüye Dön → UI Sahnesine dön (Index 1)
```

---

Bu rehberi takip ederek UI sisteminizi sahne yapısına entegre edebilirsiniz! Sorun yaşarsanız test checklist'e bakın.
