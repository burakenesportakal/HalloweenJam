# UI ve Oyun Sahnesi Entegrasyon Rehberi

Bu rehber, UI sistemini oyununuzun sahne yapısına nasıl entegre edeceğinizi adım adım anlatır.

---

## 🎯 SAHNE YAPISI

Oyununuz 4 sahneye sahip:

1. **Intro Sahnesi** (Index 0) - Intro animasyonları, logo vs.
2. **UI Sahnesi (Menu Scene)** (Index 1) - Ana menü, ayarlar, storyboard
3. **Oyun Sahnesi (Game Scene)** (Index 2) - Oyun içi UI (Pause, Health)
4. **Outro Sahnesi** (Index 3) - Oyun sonu, kazanma ekranı

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

## 📋 ADIM 2: GameCanvas Prefab Oluşturma (Önerilen - Kolay Entegrasyon)

### 2.1 Geçici GameCanvas Oluşturma (UI Sahnesinde veya Ayrı Test Sahnesinde)

**Seçenek A: UI Sahnesinde Oluştur (Önerilen)**
1. **UI sahnesini açın** (UI.unity)
2. **MainCanvas** altında → Sağ tık → **UI → Canvas** → İsmi: **"GameCanvas"**
3. **GameCanvas** ayarları:
   - **Render Mode**: Screen Space - Overlay
   - **Sort Order**: 1 (MainCanvas'tan farklı olmalı, üstte görünsün)

**Seçenek B: Yeni Test Sahnesi Oluştur**
1. **File** → **New Scene** → **Basic (Built-in)**
2. **Hierarchy** → Sağ tık → **UI → Canvas** → İsmi: **"GameCanvas"**

### 2.2 GameCanvas İçeriğini Oluşturma

1. **UIManager** ekle:
   - **GameCanvas** altında → Sağ tık → **Create Empty** → İsmi: **"UIManager"**
   - **UIManager.cs** script'ini ekle

2. **PausePanelPrefab** ekle:
   - **GameCanvas** altında → **Prefabs klasöründen** → **PausePanelPrefab**'ı **sürükle-bırak**
   - **PausePanelPrefab** instance → Inspector'da **GameObject aktiflik checkbox: ✅ TİKLI**
   - Buton referanslarını kontrol edin (zaten prefab'da olmalı)

3. **HealthUIPrefab** ekle:
   - **GameCanvas** altında → **Prefabs klasöründen** → **HealthUIPrefab**'ı **sürükle-bırak**
   - **HealthUIPrefab** instance → Inspector'da **GameObject aktiflik checkbox: ✅ TİKLI**
   - Health icons ve sprite'ları kontrol edin (zaten prefab'da olmalı)

4. **UIManager Referanslarını Bağla:**
   - **UIManager** GameObject'ini seç → Inspector'da:
     - **Pause Panel** → PausePanel GameObject'ini ata
     - **Health UI** → HealthUI GameObject'ini ata
     - **Main Menu Panel** → **BOŞ** (None)
     - **Settings Panel** → **BOŞ** (None)
     - **Storyboard Panel** → **BOŞ** (None)
     - **Main Canvas** → GameCanvas GameObject'ini ata

### 2.3 GameCanvas'ı Prefab Yapma

1. **GameCanvas** GameObject'ini seçin (tüm alt objelerle birlikte)
2. **Project** panelinde → **Prefabs** klasörüne **sürükle-bırak**
3. Prefab adı: **"GameCanvasPrefab"** veya **"GameUIPrefab"**

### 2.4 Oyun Sahnesinde GameCanvas Prefab'ını Kullanma

1. **Oyun sahnenizi açın** (GameScene.unity)
2. **Hierarchy** → **Prefabs klasöründen** → **GameCanvasPrefab**'ı **sürükle-bırak**
3. **GameCanvasPrefab** instance aktif olmalı → Inspector'da **GameObject aktiflik checkbox: ✅ TİKLI**

**Hepsi bu kadar!** UIManager referansları zaten prefab'da bağlı, oyun sahnesinde sadece prefab'ı eklemeniz yeterli.

### 2.5 (Opsiyonel) UI Sahnesinden GameCanvas'ı Kaldırma

Eğer UI sahnesinde GameCanvas oluşturduysanız:
1. **UI sahnesinde** → **GameCanvas** GameObject'ini **SİLİN** (artık prefab olarak var)
2. Veya **pasif yapın** (aktiflik checkbox: ❌ TİKSİZ)

### 2.6 Oyun Sahnesi Yapısı (Prefab ile)

**Oyun sahnesinde:**
```
GameCanvasPrefab (instance)
├── Canvas
├── UIManager
├── PausePanel (instance)
│   └── PauseMenuManager (script)
└── HealthUI (instance)
    └── HealthUI (script)
```

**NOT:** Artık oyun sahnesinde tek bir prefab eklemeniz yeterli!

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

### 4.1 Build Settings (Unity 6)

**Unity 6'da Build Settings'e erişim:**

1. **File** menüsüne tıklayın
2. **Build Profiles** seçeneğini seçin (veya kısayol: `Ctrl+Shift+B`)
3. Açılan **Build Profiles** penceresinde:
   - Sol üstteki **"+"** butonuna tıklayarak yeni bir profile oluşturun
   - Veya mevcut bir profile'ı seçin
   - **Scenes** bölümünde **"+"** butonuna tıklayarak sahneleri ekleyin

**Alternatif Yöntem (Klasik Build Settings):**
- Hala klasik Build Settings penceresini istiyorsanız: `Ctrl+Shift+B` tuşlarına basın
- Ya da: **Edit** → **Project Settings** → **Editor** → **Build Settings** bölümüne gidin

**Scenes In Build listesine ekleyin:**
   - **Index 0**: **Intro.unity** (Intro sahnesi)
   - **Index 1**: **UI.unity** (Menu sahnesi)
   - **Index 2**: **GameScene.unity** (Oyun sahnesi)
   - **Index 3**: **Outro.unity** (Outro sahnesi)

**NOT:** Eğer Build Profiles penceresinde sahneler görünmüyorsa, sahne dosyalarını **Project** panelinden sürükleyip **Scenes** listesine bırakın.

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

## 📋 ADIM 5: Oyun Sonu Kapısı (End Door)

### 5.1 End Door GameObject Oluşturma

1. **Oyun sahnenizde** → Hierarchy → Sağ tık → **Create Empty** → İsmi: **"EndDoor"**
2. **EndDoor** GameObject'ine **BoxCollider2D** ekle:
   - Inspector'da **Add Component** → **Physics 2D → Box Collider 2D**
   - **Is Trigger**: ✅ **TİKLI** (oyuncu içinden geçebilsin ama algılansın)
   - **Size**: Kapının boyutuna göre ayarlayın (örnek: 2 x 3)

### 5.2 EndDoorTrigger Script Eklemek

1. **EndDoor** GameObject'ini seç → Inspector'da **Add Component** → **EndDoorTrigger.cs**
2. Inspector'da:
   - **Player Tag**: Oyuncunuzun tag'i (genelde "Player")
   - **Has Entered**: Otomatik yönetiliyor, dokunmayın

### 5.3 Kapı Görseli (Opsiyonel)

1. **EndDoor** GameObject'ine **SpriteRenderer** ekleyin
2. Kapı sprite'ınızı atayın
3. Veya kapıyı görsel bir GameObject yapın ve EndDoor trigger'ı altına koyun

### 5.4 Çalışma Mantığı

- Oyuncu **EndDoor**'a değdiğinde (trigger veya collision)
- **EndDoorTrigger** otomatik olarak **GameManager.WinGame()** çağırır
- **GameManager** outro sahnesine geçer

---

## 📋 ADIM 6: Oyun İçi Sistemlerle Entegrasyon

### 6.1 Health Sistemi Entegrasyonu

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
- [ ] EndDoor kapıya değince outro sahnesine geçiyor mu?

---

## 🎯 ÖZET: Yapılacaklar Listesi

1. ✅ **UI Sahnesinde**: PausePanel ve HealthUI'yi **Prefab yapın**
2. ✅ **UI Sahnesinde**: Prefab instance'larını **pasif yapın** (görünmez)
3. ✅ **UI Sahnesinde**: UIManager'dan PausePanel ve HealthUI referanslarını **boşaltın**
4. ✅ **GameCanvas Prefab Oluştur**: GameCanvas, UIManager, PausePanel ve HealthUI'yi birleştirip prefab yapın
5. ✅ **Oyun Sahnesinde**: **GameCanvasPrefab**'ı ekleyin (tek seferde hepsi gelir!)
6. ✅ **Build Settings**: Sahne sıralamasını ayarlayın (Intro: 0, UI: 1, Game: 2, Outro: 3)
7. ✅ **GameManager Inspector**: Outro Scene Index = 3
8. ✅ **End Door Oluştur**: Oyun sahnesinde EndDoor GameObject + EndDoorTrigger script ekleyin
9. ✅ **Player Health Script**: UIManager.UpdateHealth() çağrısı ekleyin

### 🎯 Prefab Yaklaşımının Avantajları:
- ✅ **Tek Prefab**: GameCanvas, UIManager, PausePanel ve HealthUI hepsi bir arada
- ✅ **Kolay Entegrasyon**: Oyun sahnesine sadece bir prefab ekleyin
- ✅ **Hızlı**: Arkadaşınız 30 saniyede ekleyebilir
- ✅ **Düzenli**: Tüm UI yapısı tek bir prefab'da

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
