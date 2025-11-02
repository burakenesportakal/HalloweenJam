# GameUI Bağımsız Kurulum Rehberi

Bu rehber, **PauseMenu** ve **HealthUI**'yi ana UI sahnesinden tamamen bağımsız bir prefab olarak nasıl hazırlayacağınızı anlatır. Bu prefab'ı arkadaşınızın oyun sahnesine eklemesi tek adımda olacak.

---

## 🎯 AMAÇ

**GameCanvasPrefab** tamamen bağımsız bir prefab olacak:
- ✅ Ana UI sahnesinden (UI.unity) bağımsız
- ✅ Kendi UIManager'ı ile çalışır
- ✅ Oyun sahnesine tek prefab eklenerek hazır
- ✅ UI sahnesindeki menülerle hiçbir bağı yok

---

## 📋 ADIM 1: GameCanvas Prefab Oluşturma (Yeni Sahne)

### 1.1 Yeni Test Sahnesi Oluştur

1. **File** → **New Scene** → **Basic (Built-in)**
2. Sahneyi kaydedin: **"GameUITestScene"** (opsiyonel, sadece prefab yapmak için)
3. Bu sahneyi **silmeyin** (prefab güncellemesi için gerekli)

### 1.2 GameCanvas Oluştur

1. **Hierarchy** → Sağ tık → **UI → Canvas** → İsmi: **"GameCanvas"**
2. **GameCanvas** Inspector ayarları:
   - **Render Mode**: `Screen Space - Overlay`
   - **Sort Order**: `0` (varsayılan)

### 1.3 UIManager Ekle

1. **GameCanvas** altında → Sağ tık → **Create Empty** → İsmi: **"UIManager"**
2. **UIManager** GameObject'ine → **Add Component** → **UIManager.cs**
3. Bu UIManager **sadece oyun sahnesi için** çalışacak

### 1.4 PausePanel Oluştur (Sıfırdan)

1. **GameCanvas** altında → Sağ tık → **UI → Panel** → İsmi: **"PausePanel"**
2. **PausePanel** RectTransform:
   - **Anchor**: Stretch-Stretch (tüm ekranı kaplasın)
   - **Left, Right, Top, Bottom**: `0`

3. **PausePanel** Inspector:
   - **Image → Color**: Siyah, Alpha = `200` (yarı saydam arka plan)

4. **PauseMenuManager** script ekle:
   - **PausePanel** GameObject'ine → **Add Component** → **PauseMenuManager.cs**

5. **PausePanel** altında butonlar oluştur:
   - **ResumeButton** (Button - TextMeshPro)
   - **SettingsButton** (Button - TextMeshPro)
   - **MainMenuButton** (Button - TextMeshPro)
   - **QuitButton** (Button - TextMeshPro)

6. **PauseMenuManager** Inspector'da buton referanslarını bağlayın

7. **PausePanel** başlangıçta **pasif** olmalı → Inspector'da **GameObject aktiflik checkbox: ❌ TİKSİZ**

### 1.5 HealthUI Oluştur (Sıfırdan)

1. **GameCanvas** altında → Sağ tık → **Create Empty** → İsmi: **"HealthUI"**
2. **HealthUI** RectTransform:
   - **Anchor**: Top-Left
   - **Position**: X = `100`, Y = `-100` (sol üstte)

3. **HealthUI** GameObject'ine → **Add Component** → **HealthUI.cs**

4. **HealthUI** altında 3 tane Image (kalp ikonları) oluştur:
   - Horizontal Layout Group ekleyin (otomatik dizilim için)
   - 3 tane **Image** GameObject ekleyin → `healthIcons` array'ine atayın

5. **HealthUI** Inspector'da:
   - **Health Icons**: 3 Image referansını atayın
   - **Full Heart Sprite**: Dolu kalp sprite'ını atayın
   - **Empty Heart Sprite**: Boş kalp sprite'ını atayın (opsiyonel)
   - **Initial Health**: `3`

### 1.6 UIManager Referanslarını Bağla

**UIManager** GameObject'ini seç → Inspector'da:

- ✅ **Pause Panel** → PausePanel GameObject'ini ata
- ✅ **Health UI** → HealthUI GameObject'ini ata
- ❌ **Main Menu Panel** → **BOŞ** (None)
- ❌ **Settings Panel** → **BOŞ** (None)
- ❌ **Storyboard Panel** → **BOŞ** (None)
- ✅ **Main Canvas** → GameCanvas GameObject'ini ata

---

## 📋 ADIM 2: GameCanvas'ı Prefab Yapma

### 2.1 Prefab Klasörü Oluştur

1. **Project** panelinde → **Assets** → Sağ tık → **Create → Folder**
2. İsim: **"GameUIPrefabs"** (veya istediğiniz isim)

### 2.2 GameCanvas'ı Prefab Yap

1. **GameCanvas** GameObject'ini seçin (tüm alt objelerle birlikte - Hierarchy'de en üstteki)
2. **Project** panelinde → **GameUIPrefabs** klasörüne **sürükle-bırak**
3. Prefab adı: **"GameCanvasPrefab"**

### 2.3 Prefab Kontrolü

**GameCanvasPrefab** prefab'ını seç → Inspector'da kontrol edin:
- ✅ Canvas component var
- ✅ UIManager var ve referanslar bağlı
- ✅ PausePanel var (başlangıçta pasif)
- ✅ HealthUI var (aktif)

---

## 📋 ADIM 3: Oyun Sahnesinde Kullanım

### 3.1 Oyun Sahnesine Ekleme

1. **Oyun sahnenizi açın** (GameScene.unity veya hangi sahne ise)
2. **Hierarchy** → **Project** panelinden **GameCanvasPrefab**'ı **sürükle-bırak**
3. **GameCanvasPrefab** instance aktif olmalı → Inspector'da **GameObject aktiflik checkbox: ✅ TİKLI**

### 3.2 Kontrol Listesi

- ✅ GameCanvas görünüyor mu?
- ✅ HealthUI sol üstte görünüyor mu? (3 kalp)
- ✅ ESC tuşuna basınca PausePanel açılıyor mu?
- ✅ PausePanel butonları çalışıyor mu?

**Hepsi bu kadar!** Artık GameCanvasPrefab tamamen bağımsız bir prefab.

---

## 📋 ADIM 4: GameManager Entegrasyonu

### 4.1 GameManager Ekle

Oyun sahnesinde:
1. **Hierarchy** → Sağ tık → **Create Empty** → İsmi: **"GameManager"**
2. **GameManager.cs** script'ini ekle
3. Inspector'da ayarlar:
   - **UI Scene Index**: 1
   - **Game Scene Index**: 2
   - **Outro Scene Index**: 3

**NOT:** GameManager'ın GameCanvasPrefab ile hiçbir bağı yok. Sadece ESC tuşu ile pause/unpause yönetiyor.

---

## ⚠️ ÖNEMLİ NOTLAR

### ✅ Bağımsızlık

- **GameCanvasPrefab** tamamen bağımsız
- UI sahnesindeki (UI.unity) MainCanvas ile hiçbir bağı yok
- Kendi UIManager'ı var (UI sahnesindeki UIManager'dan farklı)
- Singleton pattern sayesinde her sahne kendi UIManager'ını kullanır

### ✅ Prefab Güncellemesi

Eğer **GameUITestScene**'de değişiklik yaparsanız:
1. Değişiklik yapın
2. **GameCanvasPrefab** prefab'ına → **Overrides → Apply All** yapın
3. Tüm oyun sahnelerindeki GameCanvasPrefab instance'ları otomatik güncellenir

### ✅ Oyun Sahnesi Yapısı

```
GameScene (oyun sahnesi)
├── GameCanvasPrefab (instance)
│   ├── Canvas
│   ├── UIManager
│   ├── PausePanel (pasif - ESC ile açılır)
│   └── HealthUI (aktif - her zaman görünür)
└── GameManager
```

---

## 🎯 AVANTAJLAR

✅ **Tamamen Bağımsız**: UI sahnesi olmadan da çalışır  
✅ **Kolay Entegrasyon**: Tek prefab eklemek yeterli  
✅ **Düzenli**: Tüm GameUI tek bir prefab'da  
✅ **Güncellenebilir**: Prefab güncellendiğinde tüm sahneler otomatik güncellenir  
✅ **Bakımı Kolay**: GameUI değişiklikleri tek yerden yapılır

---

**Hazır! 🎉 GameCanvasPrefab artık tamamen bağımsız ve oyun sahnesine eklenmeye hazır!**

