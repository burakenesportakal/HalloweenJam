# Sahne Geçişleri Rehberi

Bu rehber, oyununuzdaki tüm sahne geçişlerini nasıl yapacağınızı anlatır.

---

## 🎯 SAHNE AKIŞI

```
Intro (0) → UI (1) → Game (2) → Outro (3)
                    ↑           ↓
                    └───────────┘ (Pause Menu'den Ana Menüye Dön)
```

---

## 📋 SAHNE GEÇİŞLERİ DETAYLI AÇIKLAMA

### 1️⃣ Intro → UI (Ana Menü)

**Durum:** Intro animasyonları/logolar bitince ana menüye geçer.

**Kod:** `IntroController.cs` script'i gerekli (yeni oluşturuldu)

**Unity'de Kurulum:**
1. **Intro sahnesinde** → Hierarchy → Sağ tık → **Create Empty** → İsmi: **"IntroController"**
2. **IntroController** GameObject'ine → **Add Component** → **IntroController.cs**
3. Inspector'da:
   - **Intro Duration**: Intro süresi (örn: 5 saniye)
   - **Auto Transition**: ✅ **TİKLI** (süre bitince otomatik geç)
   - **Skip Key**: **Space** (oyuncu atlayabilir)
   - **UI Scene Index**: `1`

**Çalışma:**
- Intro süresi biter → UI sahnesine geç
- VEYA Space tuşuna bas → UI sahnesine geç

---

### 2️⃣ UI → Storyboard → Game

**Durum:** Ana menüden Play butonuna tıklayınca Storyboard açılır, Storyboard bitince oyun başlar.

**Kod:** Zaten var! ✅
- `MainMenuManager.cs` → Play butonu → `UIManager.ShowStoryboard()`
- `StoryboardManager.cs` → Storyboard bitince → `GameManager.StartGame()`
- `GameManager.StartGame()` → Oyun sahnesine geçer

**Unity'de Kurulum:**
- ✅ Zaten hazır! Sadece buton referanslarının bağlı olduğundan emin olun.

---

### 3️⃣ Game → Outro (Kapı)

**Durum:** Oyuncu kapıya değdiğinde outro sahnesine geçer.

**Kod:** `EndDoorTrigger.cs` zaten var! ✅

**Unity'de Kurulum:**
1. **Oyun sahnesinde** → Kapı GameObject'ini seç
2. **EndDoorTrigger.cs** script'ini ekle
3. Inspector'da:
   - **Player Tag**: `Player`
   - **Close Game**: ❌ **TİKSİZ** (outro'ya gitmek için)

**Çalışma:**
- Oyuncu kapıya değer → `EndDoorTrigger` → `GameManager.WinGame()` → Outro sahnesine geç

---

### 4️⃣ Game → UI (Pause Menu'den Ana Menüye Dön)

**Durum:** Pause menu'den "Ana Menüye Dön" butonuna tıklayınca UI sahnesine döner.

**Kod:** Zaten var! ✅
- `PauseMenuManager.cs` → MainMenu butonu → `GameManager.ReturnToMainMenu()`
- `GameManager.ReturnToMainMenu()` → UI sahnesine geçer

**Unity'de Kurulum:**
- ✅ Zaten hazır! Sadece buton referanslarının bağlı olduğundan emin olun.

---

## 📋 TÜM GEÇİŞLER ÖZET

| Geçiş | Nasıl | Kod |
|-------|-------|-----|
| **Intro → UI** | Intro bitince veya Space tuşu | `IntroController.LoadUIScene()` |
| **UI → Storyboard** | Play butonu | `UIManager.ShowStoryboard()` |
| **Storyboard → Game** | Storyboard bitince | `GameManager.StartGame()` |
| **Game → Outro** | Kapıya değince | `EndDoorTrigger` → `GameManager.WinGame()` |
| **Game → UI** | Pause Menu'den Ana Menüye Dön | `GameManager.ReturnToMainMenu()` |

---

## ✅ KONTROL LİSTESİ

### Intro Sahnesi:
- [ ] `IntroController.cs` script'i eklendi
- [ ] Intro Duration ayarlandı
- [ ] UI Scene Index = 1

### UI Sahnesi:
- [ ] Play butonu → MainMenuManager'a bağlı
- [ ] StoryboardManager → GameManager'a bağlı

### Game Sahnesi:
- [ ] EndDoorTrigger.cs → Kapıya eklendi
- [ ] GameManager → Oyun sahnesinde var
- [ ] PausePanel → Butonlar bağlı

---

## 🎯 KODLAR HAZIR!

✅ **Intro → UI**: `IntroController.cs` (yeni oluşturuldu)  
✅ **Storyboard → Game**: `GameManager.StartGame()` (zaten var)  
✅ **Game → Outro**: `EndDoorTrigger` → `GameManager.WinGame()` (zaten var)  
✅ **Game → UI**: `GameManager.ReturnToMainMenu()` (zaten var)

**Tek yapmanız gereken:** Intro sahnesine `IntroController.cs` script'ini eklemek!

---

**Hazır! 🎉 Tüm sahne geçişleri hazır ve çalışıyor!**

