# EndGame Boss Sistemi - Unity Entegrasyon Rehberi

Bu rehber, EndGame Boss sistemini Unity'de nasıl kuracağınızı adım adım anlatır.

---

## 📋 Oluşturulan Scriptler

### Ana Sistemler:
1. **BossController.cs** - Ana koordinatör (tüm sistemleri yönetir)
2. **BossAttackController.cs** - Saldırı sistemi (burst mermi)
3. **BossMovementController.cs** - Hareket sistemi (ileri-geri patrolling)
4. **BossFatigueSystem.cs** - Yorulma sistemi (4 saldırı → 5 saniye yorulma)
5. **BossTakeoverSystem.cs** - Ele geçirme sistemi (3 deneme)
6. **EndGameDoor.cs** - Kapı sistemi (HP, kırılma)
7. **BossWinHandler.cs** - Win condition yönetimi
8. **BossBullet.cs** - Boss mermisi

### UI Sistemleri:
9. **BossFatigueIndicator.cs** - Yorulma göstergesi
10. **BossTakeoverPrompt.cs** - Ele geçirme prompt (X tuşu)
11. **BossAttackCounter.cs** - Saldırı sayacı (opsiyonel)
12. **DoorHealthBar.cs** - Kapı can barı

---

## 🎯 ADIM 1: Boss GameObject Oluşturma

### 1.1 Boss GameObject
1. **Hierarchy** → Sağ tık → **Create Empty** → İsmi: **"EndGameBoss"**
2. **Transform** ayarları:
   - **Position**: Arena'nın merkezi
   - **Scale**: `1, 1, 1`

### 1.2 Boss Sprite/Model
1. **EndGameBoss** altına **Sprite** veya **Model** ekleyin
2. Boss görselini atayın (kaslı asker)
3. **SpriteRenderer** veya **SpriteRenderer2D** component'i ekleyin

### 1.3 Boss Collider
1. **EndGameBoss** seçiliyken → **Add Component** → **2D → Box Collider 2D**
2. Collider ayarları:
   - **Is Trigger**: ✅ **TİKLİ** (başlangıçta simbiyot içinden geçebilir)
   - **Size**: Boss sprite'ının boyutuna göre ayarlayın

### 1.4 Boss Rigidbody (Hareket için)
1. **Add Component** → **2D → Rigidbody 2D**
2. Rigidbody ayarları:
   - **Body Type**: `Kinematic` (fizik etkileşimi yok, sadece script ile hareket)
   - **Collision Detection**: `Continuous`
   - **Sleeping Mode**: `Never Sleep`

### 1.5 Boss Tag ve Layer
1. **Tag**: Yeni tag oluşturun: **"Boss"**
2. **Layer**: Yeni layer oluşturun: **"Boss"** (opsiyonel)

---

## 🎯 ADIM 2: Boss Sub-Systems Eklemek

### 2.1 Ana BossController
1. **EndGameBoss** seçiliyken → **Add Component** → **BossController.cs**
2. Inspector'da şu referansları bağla:
   - **Movement Controller**: (henüz eklenmedi, sonra bağlayacağız)
   - **Attack Controller**: (henüz eklenmedi)
   - **Fatigue System**: (henüz eklenmedi)
   - **Takeover System**: (henüz eklenmedi)
   - **Player Target**: Player GameObject'i (Tag: "Player" olan)
   - **Boss Collider**: EndGameBoss'un Collider component'i

### 2.2 BossMovementController
1. **EndGameBoss** → **Add Component** → **BossMovementController.cs**
2. Inspector ayarları:
   - **Move Speed**: `3`
   - **Patrol Distance**: `5`
   - **Change Direction Time**: `2`
   - **Use Custom Bounds**: ❌ (otomatik bounds kullanılacak)
3. **BossController**'da **Movement Controller** referansına bağla

### 2.3 BossAttackController
1. **EndGameBoss** → **Add Component** → **BossAttackController.cs**
2. **FirePoint** oluştur:
   - EndGameBoss altında → **Create Empty** → İsmi: **"FirePoint"**
   - **Position**: Boss'un silah ucunun pozisyonu (mermilerin çıktığı yer)
   - Örnek: X = `1`, Y = `0.5` (silah sağa bakar)
   - **⚠️ ÖNEMLİ**: Bu, mermilerin fiziksel olarak çıktığı nokta!
3. Inspector ayarları:
   - **Bullets Per Burst**: `6`
   - **Time Between Bullets**: `0.1`
   - **Attack Cooldown**: `1.5`
   - **Attack Range**: `15`
   - **Fire Point**: FirePoint GameObject'i (mermilerin çıktığı yer - ZORUNLU!)
   - **Use Aim Point**: ❌ **TİKSIZ** (silah karakterle bütün olduğu için)
   - **Aim Point**: (Boş bırakın - kullanılmıyor)
   - **Aim Smoothing**: `5` (ne kadar yumuşak dönecek)
   - **Spread Angle**: `30` (üçgen alan açısı - mermiler bu açı içinde rastgele gider)
   - **Use Random Spread**: ✅ **TİKLİ** (rastgele yayılım)
4. **BossController**'da **Attack Controller** referansına bağla

### ⚠️ FirePoint Açıklaması:
- **FirePoint**: Mermilerin fiziksel olarak çıktığı nokta (silah ucundan)
- **AimPoint**: Kullanılmıyor (silah karakterle bütün, boss'un kendisi player'a döner)
- Boss otomatik olarak player'a doğru döner (sprite flip veya rotation ile)

### 2.4 BossFatigueSystem
1. **EndGameBoss** → **Add Component** → **BossFatigueSystem.cs**
2. Inspector ayarları:
   - **Attacks Until Fatigue**: `4`
   - **Fatigue Duration**: `5`
   - **Fatigue Indicator**: (UI'ı sonra ekleyeceğiz)
3. **BossController**'da **Fatigue System** referansına bağla

### 2.5 BossTakeoverSystem
1. **EndGameBoss** → **Add Component** → **BossTakeoverSystem.cs**
2. Inspector ayarları:
   - **Takeover Range**: `1.5`
   - **Takeover Key**: `X`
   - **Failed Attempts Needed**: `3`
   - **Takeover Prompt**: (UI'ı sonra ekleyeceğiz)
3. **BossController**'da **Takeover System** referansına bağla

---

## 🎯 ADIM 3: Boss Mermisi (Bullet) Oluşturma

### 3.1 Bullet Prefab
1. **Hierarchy** → Sağ tık → **Create Empty** → İsmi: **"BossBullet"**
2. **BossBullet** altında → **UI → Image** veya **SpriteRenderer2D** ekleyin
3. Mermi sprite'ını atayın
4. **Rigidbody2D** ekleyin:
   - **Body Type**: `Dynamic`
   - **Gravity Scale**: `0` (yer çekimi yok)
   - **Collision Detection**: `Continuous`
5. **Circle Collider 2D** veya **Box Collider 2D** ekleyin:
   - **Is Trigger**: ✅ **TİKLİ**
6. **BossBullet.cs** scriptini ekleyin
7. Inspector ayarları:
   - **Damage**: `10`
   - **Lifetime**: `3`
   - **Damage Layers**: Player, Door layer'larını ekleyin
8. **Prefabs** klasörüne sürükle → **Prefab** yapın
9. **BossAttackController** → **Bullet Prefab** referansına bu prefab'ı ata

---

## 🎯 ADIM 4: EndGame Door (Kapı) Oluşturma

### 4.1 Door GameObject
1. **Hierarchy** → Sağ tık → **Create Empty** → İsmi: **"EndGameDoor"**
2. **Position**: Arena'nın sağında (çıkış kapısı)

### 4.2 Door Sprite
1. **EndGameDoor** altına **SpriteRenderer2D** veya **Image** ekleyin
2. Kapı sprite'ını atayın
3. **Damage States** (opsiyonel): Hasar durumlarına göre sprite'lar ekleyin (array)

### 4.3 Door Collider
1. **EndGameDoor** → **Add Component** → **2D → Box Collider 2D**
2. **Is Trigger**: ❌ **TİKSIZ** (fiziksel engel)
3. **Size**: Kapı boyutuna göre ayarlayın

### 4.4 Door Script
1. **EndGameDoor** → **Add Component** → **EndGameDoor.cs**
2. Inspector ayarları:
   - **Max Health**: `100` (arka planda çalışır, görsel olarak gösterilmez)
   - **Door Sprite**: Door SpriteRenderer component'i
   - **Destroyed Effect**: (opsiyonel) Kırılma efekti prefab'ı
   - **⚠️ Hasar Göstergesi YOK**: Kapı hasar almaz gibi görünür, sadece kırıldığında yok olur
   - **⚠️ Alternatif Son**: Oyuncu kapının kırılabileceğini keşfetmeli (tutorial yok)

---

## 🎯 ADIM 5: BossWinHandler Oluşturma

### 5.1 WinHandler GameObject
1. **Hierarchy** → Sağ tık → **Create Empty** → İsmi: **"BossWinHandler"**
2. **BossWinHandler.cs** scriptini ekle
3. Inspector ayarları:
   - **Win Delay**: `2`
   - **Next Scene Name**: Win sonrası sahne adı (örn: "Credits" veya "MainMenu")

---

## 🎯 ADIM 6: UI Sistemleri - Canvas Hazırlama (Minimal UI)

### ⚠️ Minimal UI Yaklaşımı:
Oyunun atmosferik yapısı için UI minimal tutulacak. **Health bar'lar YOK!** Sadece:
- Boss yorulma göstergesi (yorulduğunda)
- Ele geçirme prompt'u (X tuşuna bas)

### 6.1 Boss UI Canvas
1. **Hierarchy** → Sağ tık → **UI → Canvas** → İsmi: **"BossUICanvas"**
2. Canvas ayarları:
   - **Render Mode**: `Screen Space - Overlay`
   - **Canvas Scaler**: `Scale With Screen Size`
   - **Reference Resolution**: `1920 x 1080`

---

## 🎯 ADIM 7: Boss Fatigue Indicator UI

### 7.1 Fatigue Panel
1. **BossUICanvas** altında → **UI → Panel** → İsmi: **"BossFatiguePanel"**
2. Panel **RectTransform**:
   - **Anchor**: Top, Center
   - **Position**: Y = `-100`
   - **Size**: `400 x 100`
3. Panel **Image** → **Color**: Kırmızı, Alpha: `0.8` (yarı saydam arka plan)

### 7.2 Fatigue Text
1. **BossFatiguePanel** altında → **UI → Text - TextMeshPro**
2. İsmi: **"FatigueTimerText"**
3. **RectTransform**: Stretch-Stretch (panel'i kaplasın)
4. **Font Size**: `36`
5. **Color**: Beyaz
6. **Alignment**: Center, Middle
7. **Text**: "Boss Yorgun! 5s"

### 7.3 Script Bağlama
1. **BossFatiguePanel** → **Add Component** → **BossFatigueIndicator.cs**
2. Inspector'da:
   - **Fatigue Panel**: BossFatiguePanel GameObject
   - **Fatigue Timer Text**: FatigueTimerText GameObject
3. **BossFatigueSystem** → **Fatigue Indicator** referansına bağla
4. Panel başlangıçta gizli olmalı → **Active: false**

---

## 🎯 ADIM 8: Boss Takeover Prompt UI

### 8.1 Takeover Prompt Panel
1. **BossUICanvas** altında → **UI → Panel** → İsmi: **"TakeoverPromptPanel"**
2. Panel **RectTransform**:
   - **Anchor**: Center, Bottom
   - **Position**: Y = `150`
   - **Size**: `400 x 80`
3. Panel **Image** → **Color**: Sarı/Turuncu, Alpha: `0.9`

### 8.2 Prompt Text
1. **TakeoverPromptPanel** altında → **UI → Text - TextMeshPro**
2. İsmi: **"PromptText"**
3. **RectTransform**: Stretch-Stretch
4. **Font Size**: `32`
5. **Color**: Siyah
6. **Alignment**: Center, Middle
7. **Text**: "X - ELE GEÇİR"

### 8.3 Script Bağlama
1. **TakeoverPromptPanel** → **Add Component** → **BossTakeoverPrompt.cs**
2. Inspector'da:
   - **Prompt Panel**: TakeoverPromptPanel GameObject
   - **Prompt Text**: PromptText GameObject
3. **BossTakeoverSystem** → **Takeover Prompt** referansına bağla
4. Panel başlangıçta gizli → **Active: false**

---

## ⚠️ ADIM 9: Door Health Bar UI - KALDIRILDI

**Health bar'lar oyunun atmosferik yapısı için kaldırıldı. Kapı hasarı görsel olarak sprite değişimi ile gösterilecek (damageStates).**

---

## ⚠️ ADIM 9: Boss Attack Counter UI - KALDIRILDI

**Minimal UI yaklaşımı için saldırı sayacı kaldırıldı.**

---

## 🎯 ADIM 10: Player Referansı ve Tag Ayarları

### 10.1 Player Tag
1. Unity Editor → **Edit → Project Settings → Tags and Layers**
2. **Tags** bölümüne **"Player"** tag'i ekleyin (yoksa)
3. Player GameObject'ine **"Player"** tag'ini atayın

### 10.2 BossController Player Referansı
1. **EndGameBoss** → **BossController** component'i
2. **Player Target** alanına Player GameObject'ini sürükle

---

## 🎯 ADIM 11: Layer Ayarları

### 11.1 Layer Oluşturma
1. **Edit → Project Settings → Tags and Layers**
2. Yeni layer'lar ekleyin (opsiyonel):
   - **"Boss"**
   - **"BossProjectiles"**
   - **"Door"**

### 11.2 GameObject Layer Atamaları
1. **EndGameBoss** → Layer: **"Boss"**
2. **BossBullet** prefab → Layer: **"BossProjectiles"**
3. **EndGameDoor** → Layer: **"Door"**

---

## 🎯 ADIM 12: Boss Bullet - Kapıya Hasar Entegrasyonu

### 12.1 BossBullet.cs Güncellemesi
BossBullet script'i zaten kapıya hasar veriyor, sadece kontrol edin:
1. **BossBullet** prefab'ı seçin
2. Inspector'da **BossBullet.cs** component'i
3. **Damage Layers** → **Door** layer'ını ekleyin
4. **Damage** değerini ayarlayın (örn: `10`)

### 12.2 EndGameDoor Layer Ayarı
1. **EndGameDoor** → Layer: **"Door"**
2. **EndGameDoor** → **EndGameDoor.cs** → **Max Health**: `100`

---

## 🎯 ADIM 13: Boss Ele Geçirildikten Sonra Kapıyı Kırma

### 13.1 Boss Ele Geçirildiğinde
Boss ele geçirildiğinde (BossController.OnBossTakenOver), boss'un saldırı sistemi artık kapıyı hedef almalı.

**Not:** Bu entegrasyon için BossController'a ek bir metod eklenebilir veya BossAttackController ele geçirildikten sonra kapıyı hedef alacak şekilde güncellenebilir.

### 13.2 Ele Geçirme Sonrası Kontrol
1. Boss ele geçirildiğinde **BossWinHandler** tetikleniyor
2. Boss artık player kontrolünde (bu kısım arkadaşınızın player sistemi ile entegre edilecek)

---

## 🎯 ADIM 14: Final Kontroller ve Test

### 14.1 Kontrol Listesi

#### Boss GameObject:
- [ ] **EndGameBoss** oluşturuldu
- [ ] **BossController.cs** eklendi ve referanslar bağlandı
- [ ] **BossMovementController.cs** eklendi
- [ ] **BossAttackController.cs** eklendi, **FirePoint** oluşturuldu
- [ ] **BossFatigueSystem.cs** eklendi
- [ ] **BossTakeoverSystem.cs** eklendi
- [ ] **Collider2D** eklendi (Is Trigger: true)
- [ ] **Rigidbody2D** eklendi (Kinematic)
- [ ] **Player Target** referansı bağlandı

#### Boss Bullet:
- [ ] **BossBullet** prefab oluşturuldu
- [ ] **BossBullet.cs** eklendi
- [ ] **BossAttackController** → **Bullet Prefab** referansı bağlandı

#### EndGame Door:
- [ ] **EndGameDoor** oluşturuldu
- [ ] **EndGameDoor.cs** eklendi
- [ ] **Collider2D** eklendi (Is Trigger: false)
- [ ] **Max Health** ayarlandı
- [ ] **Health Bar** YOK (minimal UI - sadece sprite değişimi ile hasar gösterilir)

#### BossWinHandler:
- [ ] **BossWinHandler** GameObject oluşturuldu
- [ ] **BossWinHandler.cs** eklendi
- [ ] **Next Scene Name** ayarlandı

#### UI Sistemleri (Minimal):
- [ ] **BossUICanvas** oluşturuldu
- [ ] **BossFatigueIndicator** UI oluşturuldu ve bağlandı (yorulma göstergesi)
- [ ] **BossTakeoverPrompt** UI oluşturuldu ve bağlandı (X tuşu prompt)
- [ ] **DoorHealthBar** YOK (minimal UI)
- [ ] **BossAttackCounter** YOK (minimal UI)

#### Tag ve Layer:
- [ ] **"Player"** tag'i oluşturuldu ve Player'a atandı
- [ ] **"Boss"** layer'ı oluşturuldu (opsiyonel)
- [ ] **"Door"** layer'ı oluşturuldu (opsiyonel)

---

### 14.2 Test Senaryoları

### Test 1: Boss Hareketi
1. Play'e basın
2. Boss'un ileri-geri gittiğini kontrol edin

### Test 2: Boss Saldırısı
1. Player'ı boss'a yaklaştırın
2. Boss'un durup ateş ettiğini kontrol edin
3. 6 mermi atıp durduğunu kontrol edin

### Test 3: Boss Yorulma
1. Boss'un 4 saldırı yaptığını kontrol edin
2. 4. saldırıdan sonra yorulduğunu (UI gösterimi) kontrol edin
3. Collider'ın trigger olmaktan çıktığını kontrol edin

### Test 4: Ele Geçirme
1. Boss yorgunken player'ı yaklaştırın
2. "X - ELE GEÇİR" prompt'unun göründüğünü kontrol edin
3. X tuşuna basın → Başarısız deneme
4. 3 kez tekrarlayın → Başarılı ele geçirme

### Test 5: Kapı Hasarı
1. Boss'un mermilerini kapıya doğru ateş edin
2. Kapı can barının azaldığını kontrol edin
3. Kapı kırıldığında win condition'ın tetiklendiğini kontrol edin

---

## ⚠️ Önemli Notlar

1. **Boss Ele Geçirme**: Boss ele geçirildikten sonra player kontrolüne geçer. Bu kısım arkadaşınızın player sistemi ile entegre edilecek.

2. **Player Damage**: BossBullet.cs'de player hasar kısmı comment'lenmiş. Player health sisteminiz hazır olduğunda entegre edin.

3. **Win Condition**: İki win condition var:
   - Boss ele geçirilirse → Kapıyı kır → Win
   - Kapı kırılırsa → Kaçış → Win

4. **Boss Collider Toggle**: 
   - Normal: `isTrigger = true` (simbiyot içinden geçer)
   - Yorulunca: `isTrigger = false` (ele geçirme teması)

5. **FirePoint Pozisyonu**: Boss'un silah/el pozisyonuna göre ayarlayın.

---

## 🔧 Sorun Giderme

**Boss hareket etmiyor:**
- Rigidbody2D eklendi mi? (Kinematic olmalı)
- MovementController referansı bağlandı mı?
- Player Target atandı mı?

**Boss saldırmıyor:**
- AttackController referansı bağlandı mı?
- FirePoint oluşturuldu mu?
- Bullet prefab atandı mı?
- Player tag'i doğru mu?

**Ele geçirme çalışmıyor:**
- TakeoverSystem referansı bağlandı mı?
- Collider isTrigger ayarı doğru mu? (yorulunca false olmalı)
- Takeover key (X) doğru mu?

**Kapıya hasar verilmiyor:**
- EndGameDoor.cs eklendi mi?
- BossBullet damage layer'ları doğru mu?
- Door layer ayarı doğru mu?

---

**Hazır! 🎉 Boss sistemi Unity'de kuruldu. Test edip entegrasyonu tamamlayabilirsiniz!**

