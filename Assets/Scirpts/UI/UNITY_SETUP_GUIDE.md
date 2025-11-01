# Unity UI Sistemi Kurulum Rehberi

Bu rehber, oyununuz için tüm UI elementlerini Unity'de nasıl kuracağınızı adım adım anlatır.

---

## 🎨 ADIM 0: Font ve Renk Ayarlama

Tüm UI text'lerinize font ve renk uygulamak için Editor Window kullanacağız:

### 0.1 Font Asset Oluşturma (İlk Kez)
1. Font dosyanızı (TTF/OTF) Unity'ye import edin
2. **Project** panelinde font dosyasına **sağ tık** → **Create → TextMeshPro → Font Asset**
3. Font Asset oluşturuldu (aynı klasörde görünecek)

### 0.2 Editor Window'u Açma
1. Unity Editor → **Window → UI Style Manager**
2. Pencere açılacak

### 0.3 Font ve Renk Uygulama
1. **Font Asset** alanına oluşturduğunuz Font Asset'i sürükleyin
2. **Text Color** alanından text rengini seçin
3. **(Opsiyonel) Root Transform**: Eğer sadece belirli bir alt hiyerarşideki text'leri güncellemek istiyorsanız, root GameObject'i sürükleyin (boş bırakırsanız tüm scene'deki text'ler güncellenir)
4. **Include Inactive Objects**: Pasif objeleri de dahil etmek için tıklayın (genelde kapalı kalabilir)
5. **"Font + Renk Uygula"** butonuna basın
6. Tüm TextMeshProUGUI text'ler otomatik güncellenir!

### 0.4 Ayrı Ayrı Uygulama (İsterseniz)
- **"Sadece Font"**: Sadece font'u değiştirmek için
- **"Sadece Renk"**: Sadece rengi değiştirmek için

### ⚠️ Önemli Notlar:
- Font Asset olmadan font uygulanamaz (önce Font Asset oluşturmalısınız)
- Bu window her zaman açık kalabilir, ihtiyaç duydukça kullanabilirsiniz
- Text'leri değiştirdikten sonra bir daha uygulamak için tekrar butona basın

---

## 📋 Oluşturulan Scriptler

1. **UIManager.cs** - Merkezi UI yönetimi
2. **MainMenuManager.cs** - Ana menü paneli
3. **SettingsManager.cs** - Ayarlar menüsü
4. **StoryboardManager.cs** - Hikaye ekranı
5. **PauseMenuManager.cs** - Duraklatma menüsü
6. **AlarmUI.cs** - Alarm sayacı + Can barı (sadece alarmda görünür)
7. **GameManager.cs** - Oyun durumu yönetimi
8. **AudioManager.cs** - Ses sistemi

---

## 🎯 ADIM 1: Ana Canvas Oluşturma

1. **Hierarchy** → Sağ tık → **UI → Canvas**
2. Canvas'ın adını **"MainCanvas"** olarak değiştir
3. **Canvas** component ayarları:
   - **Render Mode**: `Screen Space - Overlay`
   - **Canvas Scaler** → **UI Scale Mode**: `Scale With Screen Size`
   - **Reference Resolution**: `1920 x 1080`
   - **Match**: `0.5` (Width ve Height arasında ortalama)

---

## 🎯 ADIM 2: GameManager ve UIManager Oluşturma

### GameManager
1. **Hierarchy** → Sağ tık → **Create Empty** → İsmi: **"GameManager"**
2. **GameManager.cs** scriptini ekle
3. **DontDestroyOnLoad** ayarını kontrol et (script içinde zaten var)

### UIManager
1. **MainCanvas** altında → Sağ tık → **Create Empty** → İsmi: **"UIManager"**
2. **UIManager.cs** scriptini ekle
3. ⚠️ **Not**: 
   - UIManager Canvas altında olmalı çünkü Canvas'ı otomatik bulur (`GetComponentInParent<Canvas>()`)
   - Alternatif olarak scene root'ta da olabilir ama Canvas referansını manuel vermeniz gerekir
   - Manager script'leri (MainMenuManager, SettingsManager vb.) ise direkt panel GameObject'lerine eklenecek (ADIM 3, 4, 5, 6'da göreceksiniz)

---

## 🎯 ADIM 3: Ana Menü Paneli (MainMenuPanel)

### Panel Oluşturma
1. **MainCanvas** altında → Sağ tık → **UI → Panel** → İsmi: **"MainMenuPanel"**
2. Panel **RectTransform** ayarları:
   - **Anchor**: Stretch-Stretch (tüm ekranı kaplasın)
   - **Left, Right, Top, Bottom**: `0`

### UI Elementleri
1. **GameTitle** (Text - TextMeshPro):
   - MainMenuPanel altında → **UI → Text - TextMeshPro**
   - İsmi: **"GameTitle"**
   - **RectTransform**: Center, Top anchor
   - **Position**: Y = `-100`
   - **Font Size**: `72`
   - **Alignment**: Center
   - **Text**: "TAKE OVER"

2. **PlayButton** (Button):
   - MainMenuPanel altında → **UI → Button - TextMeshPro**
   - İsmi: **"PlayButton"**
   - **RectTransform**: Center anchor
   - **Position**: Y = `0`
   - **Size**: `300 x 60`
   - Text: **"OYNA"**

3. **SettingsButton** (Button):
   - MainMenuPanel altında → **UI → Button - TextMeshPro**
   - İsmi: **"SettingsButton"**
   - **RectTransform**: Center anchor
   - **Position**: Y = `-80`
   - **Size**: `300 x 60`
   - Text: **"AYARLAR"**

4. **QuitButton** (Button):
   - MainMenuPanel altında → **UI → Button - TextMeshPro**
   - İsmi: **"QuitButton"**
   - **RectTransform**: Center anchor
   - **Position**: Y = `-160`
   - **Size**: `300 x 60`
   - Text: **"ÇIKIŞ"**

### Script Bağlama
1. **MainMenuPanel** GameObject'ini seç (Hierarchy'de)
2. Inspector'da **Add Component** → **MainMenuManager.cs** scriptini ekle
3. ⚠️ **ÖNEMLİ**: MainMenuManager script'i **direkt MainMenuPanel GameObject'ine** eklenmeli! Ayrı bir GameObject oluşturmayın!
4. **UIManager** GameObject'ini seç → Inspector'da **Main Menu Panel** referansına MainMenuPanel'i ata
   - Manager script'i otomatik bulunur, manuel bağlamaya gerek yok!

---

## 🎯 ADIM 4: Ayarlar Menüsü (SettingsPanel)

### Panel Oluşturma
1. **MainCanvas** altında → Sağ tık → **UI → Panel** → İsmi: **"SettingsPanel"**
2. Panel **RectTransform**: Stretch-Stretch (tüm ekranı kaplasın)

### UI Elementleri

1. **SettingsTitle** (Text - TextMeshPro):
   - SettingsPanel altında → **UI → Text - TextMeshPro**
   - **Font Size**: `48`
   - **Text**: "AYARLAR"
   - **Position**: Center, Top (Y = -50)

2. **MasterVolumeSlider** (Slider):
   - SettingsPanel altında → **UI → Slider**
   - İsmi: **"MasterVolumeSlider"**
   - **Position**: Center (Y = 100)
   - **Size**: `400 x 30`
   - **Min Value**: `0`, **Max Value**: `1`, **Value**: `1`

3. **MasterVolumeText** (Text - TextMeshPro):
   - MasterVolumeSlider'ın üstünde veya yanında
   - İsmi: **"MasterVolumeText"**
   - **Text**: "Master: 100%"
   - **Font Size**: `24`

4. **MusicVolumeSlider** (Slider):
   - MasterVolumeSlider'ın altında (Y = 50)
   - İsmi: **"MusicVolumeSlider"**
   - Aynı ayarlar

5. **MusicVolumeText** (Text - TextMeshPro):
   - MusicVolumeSlider yanında
   - İsmi: **"MusicVolumeText"**

6. **SFXVolumeSlider** (Slider):
   - MusicVolumeSlider'ın altında (Y = 0)
   - İsmi: **"SFXVolumeSlider"**
   - Aynı ayarlar

7. **SFXVolumeText** (Text - TextMeshPro):
   - SFXVolumeSlider yanında
   - İsmi: **"SFXVolumeText"`

8. **LanguageDropdown** (Dropdown - TextMeshPro):
   - Slider'ların altında
   - İsmi: **"LanguageDropdown"**
   - **⚠️ Unity 6 + TextMeshPro**: **UI → Dropdown - TextMeshPro** seçeneğini kullanın
   - (Eğer yoksa: Normal Dropdown oluşturun, içindeki Label'ı silin, TextMeshPro label ekleyin)
   - **Size**: `300 x 40`
   - **Component**: TMP_Dropdown olmalı (UnityEngine.UI.Dropdown değil!)

9. **LanguageLabel** (Text - TextMeshPro) [Opsiyonel]:
   - LanguageDropdown'ın üstünde
   - **Text**: "Dil:"

10. **BackButton** (Button):
    - SettingsPanel altında → **UI → Button - TextMeshPro**
    - İsmi: **"BackButton"**
    - **Position**: Center, Bottom (Y = 50)
    - **Size**: `200 x 50`
    - Text: **"GERİ"`

### Script Bağlama
1. **SettingsPanel** GameObject'ini seç (Hierarchy'de)
2. Inspector'da **Add Component** → **SettingsManager.cs** scriptini ekle
3. ⚠️ **ÖNEMLİ**: SettingsManager script'i **direkt SettingsPanel GameObject'ine** eklenmeli! (Zaten doğru yapıyorsunuz ✅)
4. Inspector'da SettingsManager script'indeki tüm referansları bağla:
   - Slider'ları, Text'leri, Dropdown'ı script'e sürükle
5. **UIManager** GameObject'ini seç → Inspector'da:
   - **Settings Panel** referansına SettingsPanel'i ata
   - Manager script'i otomatik bulunur, manuel bağlamaya gerek yok!

---

## 🎯 ADIM 5: Storyboard Paneli (StoryboardPanel)

### Panel Oluşturma
1. **MainCanvas** altında → Sağ tık → **UI → Panel** → İsmi: **"StoryboardPanel"**
2. Panel **RectTransform**: Stretch-Stretch
3. **⚠️ Önemli**: Başlangıçta gizli olmalı! → Inspector'da Active checkbox: ❌ **TİKSIZ**

### UI Elementleri

1. **StoryText** (Text - TextMeshPro):
   - StoryboardPanel altında → **UI → Text - TextMeshPro**
   - İsmi: **"StoryText"**
   - **RectTransform**: Stretch (Left/Right = 200, Top/Bottom = 150)
   - **Font Size**: `32`
   - **Alignment**: Center, Middle
   - **Text**: "Hikaye metinleri burada gösterilecek..." (geçici, script değiştirecek)

2. **NextButton** (Button - TextMeshPro):
   - StoryboardPanel altında → **UI → Button - TextMeshPro**
   - İsmi: **"NextButton"**
   - **Position**: Center, Bottom (Y = 100)
   - **Size**: `200 x 50`
   - Button içindeki **Text** GameObject'ini seç → **TextMeshProUGUI** component'inde Text: **"İLERİ"**

3. **SkipButton** (Button - TextMeshPro):
   - StoryboardPanel altında → **UI → Button - TextMeshPro**
   - İsmi: **"SkipButton"**
   - **Position**: Center, Bottom (Y = 50)
   - **Size**: `200 x 50`
   - Button içindeki **Text** GameObject'ini seç → **TextMeshProUGUI** component'inde Text: **"ATLA"**

### Unity 6 Notu:
- Unity 6'da "UI → Button - TextMeshPro" seçeneği varsa kullanın
- Yoksa normal "UI → Button" oluşturun, içindeki Text GameObject'ini silin ve yerine **UI → Text - TextMeshPro** ekleyin

### Script Bağlama
1. **StoryboardPanel** GameObject'ini seç (Hierarchy'de)
2. Inspector'da **Add Component** → **StoryboardManager.cs** ekle
3. ⚠️ **ÖNEMLİ**: StoryboardManager script'i **direkt StoryboardPanel GameObject'ine** eklenmeli!
4. Inspector'da StoryboardManager script'indeki referansları ata:
   - **Story Text** → StoryText GameObject'ini sürükle
   - **Next Button** → NextButton GameObject'ini sürükle
   - **Skip Button** → SkipButton GameObject'ini sürükle
5. **UIManager** GameObject'ini seç → Inspector'da:
   - **Storyboard Panel** referansına StoryboardPanel'i ata
   - Manager script'i otomatik bulunur, manuel bağlamaya gerek yok!
6. (Opsiyonel) **StoryboardManager** script'inde Inspector'da **Story Pages** listesine hikaye metinlerinizi ekleyin (veya kod içinde düzenleyin)

---

## 🎯 ADIM 6: Duraklatma Menüsü (PausePanel)

### Panel Oluşturma
1. **MainCanvas** altında → Sağ tık → **UI → Panel** → İsmi: **"PausePanel"**
2. Panel **RectTransform**: Stretch-Stretch
3. Panel **Image** → **Color**: Siyah, Alpha: `0.7` (yarı saydam arka plan)

### UI Elementleri

1. **ResumeButton** (Button):
   - PausePanel altında → **UI → Button - TextMeshPro**
   - İsmi: **"ResumeButton"**
   - **Position**: Center (Y = 100)
   - **Size**: `300 x 60`
   - Text: **"DEVAM ET"`

2. **SettingsButton** (Button):
   - ResumeButton'un altında (Y = 0)
   - İsmi: **"SettingsButton"**
   - **Size**: `300 x 60`
   - Text: **"AYARLAR"`

3. **MainMenuButton** (Button):
   - SettingsButton'un altında (Y = -80)
   - İsmi: **"MainMenuButton"**
   - **Size**: `300 x 60`
   - Text: **"ANA MENÜYE DÖN"`

4. **QuitButton** (Button):
   - MainMenuButton'un altında (Y = -160)
   - İsmi: **"QuitButton"**
   - **Size**: `300 x 60`
   - Text: **"ÇIKIŞ"`

### Script Bağlama
1. **PausePanel** GameObject'ini seç (Hierarchy'de)
2. Inspector'da **Add Component** → **PauseMenuManager.cs** scriptini ekle
3. ⚠️ **ÖNEMLİ**: PauseMenuManager script'i **direkt PausePanel GameObject'ine** eklenmeli!
4. Inspector'da PauseMenuManager script'indeki buton referanslarını ata (script otomatik bulur ama manuel de atayabilirsiniz)
5. **UIManager** GameObject'ini seç → Inspector'da:
   - **Pause Panel** referansına PausePanel'i ata
   - Manager script'i otomatik bulunur, manuel bağlamaya gerek yok!
6. **PausePanel** başlangıçta gizli olmalı → Inspector'da GameObject aktiflik checkbox: ❌ **TİKSIZ**

---

## 🎯 ADIM 7: Alarm UI (AlarmUI)

### Container Oluşturma
1. **MainCanvas** altında → Sağ tık → **Create Empty** → İsmi: **"AlarmUI"**
2. AlarmUI altında → **UI → Panel** → İsmi: **"AlarmContainer"**
3. **AlarmContainer** RectTransform ayarları:
   - **Anchor**: Stretch-Stretch (tüm ekranı kaplasın)
   - **Left, Right, Top, Bottom**: `0`
   - Bu sayede container tüm ekranı kaplar ve içerik ortada görünür
4. **AlarmContainer** başlangıçta gizli olmalı (Inspector'da Active: false)

### UI Elementleri

1. **AlarmTimerText** (Text - TextMeshPro):
   - AlarmContainer altında → **UI → Text - TextMeshPro**
   - İsmi: **"AlarmTimerText"**
   - **RectTransform** ayarları:
     - **Anchor**: Center, Top
     - **Position**: X = `0`, Y = `-80`
     - **Pivot**: `0.5, 1` (üstten orta)
   - **Font Size**: `48`
   - **Color**: Kırmızı
   - **Alignment**: Center, Top
   - **Text**: "ALARM: 5" (sayaç gösterimi)

2. **HealthIcons** (Image Array - Hollow Knight maskeleri gibi):

   **Yöntem 1: Horizontal Layout Group ile (Önerilen - Daha Kolay):**
   
   a. **HealthIconsContainer** oluştur:
   - AlarmContainer altında → Sağ tık → **Create Empty** → İsmi: **"HealthIconsContainer"**
   - **RectTransform** ayarları:
     - **Anchor**: Center (middle-center)
     - **Position**: X = `0`, Y = `50`
     - **Size**: `200 x 80` (başlangıç boyutu, Layout Group genişletecek)
     - **Pivot**: `0.5, 0.5` (ortada)
   
   b. **Horizontal Layout Group** ekle:
   - HealthIconsContainer'ı seç → Inspector'da **Add Component** → **Layout → Horizontal Layout Group**
   - **Horizontal Layout Group** ayarları:
     - **Spacing**: `15` (ikonlar arası boşluk - daha fazla görünürlük için)
     - **Child Alignment**: `Middle Center` (ortada hizala)
     - **Child Control Size**: ✅ **Width** işaretle, ✅ **Height** işaretle (ikonlar aynı boyutta olur)
     - **Child Force Expand**: ❌ **Width** tiksiz, ❌ **Height** tiksiz (otomatik genişlemesin)
     - **Padding**: Left = `0`, Right = `0`, Top = `0`, Bottom = `0`
   
   c. **Content Size Fitter** ekle (ÖNEMLİ - Container boyutunu otomatik ayarlar):
   - HealthIconsContainer'a **Add Component** → **Layout → Content Size Fitter**
   - **Horizontal Fit**: `Preferred Size` (içeriğe göre genişlik ayarla)
   - **Vertical Fit**: `Preferred Size` (içeriğe göre yükseklik ayarla)
   
   d. **3 Kalp İkonu** oluştur:
   - HealthIconsContainer altında → **UI → Image** → İsmi: **"HeartIcon1"**
   - HealthIconsContainer altında → **UI → Image** → İsmi: **"HeartIcon2"**
   - HealthIconsContainer altında → **UI → Image** → İsmi: **"HeartIcon3"**
   - Her ikonun **RectTransform** ayarları:
     - **Anchor**: Middle-Left (veya Middle-Center)
     - **Size**: `70 x 70` (daha büyük ve görünür)
     - **Pivot**: `0.5, 0.5`
   - Her ikona kalp sprite'ını ekleyin (Inspector → Image → Sprite)
   - **Image Type**: Simple
   - **Preserve Aspect**: ✅ İşaretle (orantı korunur)
   
   **Yöntem 2: Manuel Dizilim (Layout Group olmadan - Sadece referans için):**
   - AlarmContainer altında → 3 tane **UI → Image** oluştur
   - İsimler: **"HeartIcon1"**, **"HeartIcon2"**, **"HeartIcon3"**
   - **Position**: Yatay dizilim (örnek: X = -80, 0, 80, Y = 50)
   - **Size**: Her ikon `70 x 70`
   - Her ikona kalp sprite'ını ekleyin (Inspector'da)

### Script Bağlama
1. **AlarmUI** GameObject'ini seç → Inspector'da **Add Component** → **AlarmUI.cs** scriptini ekle
2. Script'te referansları ata:
   - **Alarm Container** → AlarmContainer GameObject (Panel)
   - **Alarm Timer Text** → AlarmTimerText GameObject (TextMeshPro)
   - **Health Icons** → Array'e 3 kalp ikonunu sürükle:
     - **Size**: `3` yap
     - **Element 0**: HeartIcon1 GameObject
     - **Element 1**: HeartIcon2 GameObject
     - **Element 2**: HeartIcon3 GameObject
   - **Full Heart Sprite** → Dolu kalp sprite'ı (Inspector'da seçin)
   - **Empty Heart Sprite** → Boş kalp sprite'ı (opsiyonel, boş bırakılabilir - renk değişimi kullanılır)
3. **UIManager** GameObject'ini seç → Inspector'da **Alarm UI** referansına AlarmUI GameObject'ini ata

### ⚠️ Önemli Notlar:
- **Max Health**: 3 kalp (Hollow Knight maskeleri gibi)
- Dolu kalpler: `fullHeartColor` (beyaz)
- Boş kalpler: `emptyHeartColor` (gri, yarı saydam)
- Sprite yoksa sadece renk değişimi kullanılır
- **Container boyut sorunları için**: Content Size Fitter kullanın ve RectTransform Size'ı yeterince büyük ayarlayın
- **Anchor ayarları**: Center anchor kullanarak içeriği ortada tutun

---

## 🎯 ADIM 8: AudioManager Oluşturma

1. **Hierarchy** → Sağ tık → **Create Empty** → İsmi: **"AudioManager"**
2. **AudioManager.cs** scriptini ekle
3. **AudioManager** altında → **Audio Source** ekle (Müzik için)
4. **AudioManager** altında → **Audio Source** ekle (SFX için)
5. Script'te **musicSource** ve **sfxSource** referanslarını ata
6. (Opsiyonel) Arka plan müziği ve alarm sesi clip'lerini ekle

---

## 🎯 ADIM 9: UIManager Referans Bağlama

**UIManager** scriptinde Inspector'da şu referansları bağla:

- ✅ **Main Menu Panel** → MainMenuPanel GameObject
- ✅ **Settings Panel** → SettingsPanel GameObject
- ✅ **Storyboard Panel** → StoryboardPanel GameObject
- ✅ **Pause Panel** → PausePanel GameObject
- ✅ **Alarm UI** → AlarmUI GameObject
- ✅ **Main Canvas** → MainCanvas

⚠️ **ÖNEMLİ**: Manager script referanslarını **manuel bağlamanıza gerek yok!** UIManager otomatik olarak panel GameObject'lerinden manager script'lerini bulur (`GetComponent`).

---

## 🎯 ADIM 10: İlk Ayarlar (ÖNEMLİ! Tüm paneller aynı anda açık görünüyorsa bunu yapın)

Unity'de her panel GameObject'ini seçip Inspector'da şu ayarları yapın:

1. **MainMenuPanel** → Inspector → GameObject aktiflik checkbox'ı: ✅ **TİKLI** (başlangıçta görünür)
2. **SettingsPanel** → Inspector → GameObject aktiflik checkbox'ı: ❌ **TİKSIZ**
3. **StoryboardPanel** → Inspector → GameObject aktiflik checkbox'ı: ❌ **TİKSIZ**
4. **PausePanel** → Inspector → GameObject aktiflik checkbox'ı: ❌ **TİKSIZ**
5. **AlarmContainer** (AlarmUI altında) → Inspector → GameObject aktiflik checkbox'ı: ❌ **TİKSIZ**

### ⚠️ Önemli Not:
- **GameManager** oyun başladığında otomatik olarak `ShowMainMenu()` çağırır ve diğer panelleri gizler
- Ama Unity'de GameObject'ler **başlangıçta aktif** ise, oyun çalışmadan önce hepsi görünür
- Bu yüzden **mutlaka ADIM 10'u yapın** - sadece MainMenuPanel aktif olmalı

---

## 🎮 Kullanım Örnekleri

### Oyun İçi Alarm Gösterme
```csharp
// Alarm başladığında
UIManager.Instance.ShowAlarmUI();
UIManager.Instance.UpdateAlarmTimer(5f); // 5 saniye geri sayım
UIManager.Instance.UpdateHealth(2, 2); // Can barını güncelle
```

### Alarm Bittiğinde
```csharp
UIManager.Instance.HideAlarmUI();
```

### Oyun İçi Duraklatma
```csharp
// ESC tuşu ile otomatik açılır (GameManager'da)
// Veya manuel:
GameManager.Instance.PauseGame();
```

---

## ✅ Kontrol Listesi

- [ ] Canvas oluşturuldu ve ayarlandı
- [ ] Tüm paneller oluşturuldu (4 panel)
- [ ] Tüm scriptler eklendi
- [ ] UIManager'da tüm referanslar bağlandı
- [ ] GameManager ve AudioManager oluşturuldu
- [ ] Başlangıç durumları ayarlandı (MainMenu görünür, diğerleri gizli)
- [ ] Butonlar çalışıyor
- [ ] TextMeshPro import edildi (ilk kullanımda otomatik)

---

## 🐛 Sorun Giderme

**TextMeshPro hatası alırsanız:**
- Window → TextMeshPro → Import TMP Essential Resources

**Butonlar çalışmıyorsa:**
- EventSystem var mı kontrol et (Canvas otomatik oluşturur)
- Buton referansları script'e bağlı mı kontrol et

**Paneller görünmüyorsa:**
- Inspector'da Active checkbox'ı işaretli mi?
- Canvas Render Mode doğru mu? (Screen Space - Overlay)

---

**Hazır! 🎉 Oyununuz çalışmaya hazır. Şimdi Player ve Enemy sistemlerini ekleyebilirsiniz!**

