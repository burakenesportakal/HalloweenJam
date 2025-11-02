# Build Settings Kurulum Rehberi (Unity 6)

Bu rehber, oyununuzun sahnelerini Build Settings'e nasıl ekleyeceğinizi anlatır.

---

## 🎯 SAHNE SIRALAMASI

Sahneler şu sırayla olmalı:

1. **Index 0**: **Intro.unity** (Intro sahnesi)
2. **Index 1**: **UI.unity** (Main Menu sahnesi)
3. **Index 2**: **GameScene.unity** (Oyun sahnesi - isminiz farklıysa o)
4. **Index 3**: **Outro.unity** (Outro sahnesi)

---

## 📋 ADIM 1: Build Profiles Penceresini Açma (Unity 6)

**Yöntem 1: Kısayol**
- `Ctrl + Shift + B` tuşlarına basın

**Yöntem 2: Menü**
- **File** → **Build Profiles**

**Yöntem 3: Project Settings**
- **Edit** → **Project Settings** → **Editor** → **Build Settings**

---

## 📋 ADIM 2: Sahneleri Ekleme

### 2.1 Build Profiles Penceresi

1. Açılan **Build Profiles** penceresinde:
   - Sol üstte **"+"** butonuna tıklayarak yeni bir profile oluşturun
   - Veya mevcut bir profile'ı seçin (varsa)

2. **Scenes In Build** bölümünü bulun:
   - Genelde pencerenin sol tarafında veya üst kısmında yer alır

### 2.2 Sahne Dosyalarını Ekleme

**Yöntem A: Sürükle-Bırak (Kolay)**
1. **Project** panelinde sahne dosyalarınızı bulun:
   - `Assets/Scenes/Intro.unity`
   - `Assets/Scenes/UI.unity`
   - `Assets/Scenes/GameScene.unity` (veya oyun sahnenizin adı)
   - `Assets/Scenes/Outro.unity`

2. **Build Profiles** penceresindeki **Scenes In Build** listesine **sürükle-bırak**

3. Sıralamayı kontrol edin:
   - Intro: **Index 0**
   - UI: **Index 1**
   - GameScene: **Index 2**
   - Outro: **Index 3**

**Yöntem B: Add Open Scenes**
1. Her sahneyi sırayla açın:
   - Önce **Intro.unity**'yi açın
   - Build Profiles penceresinde **Add Open Scenes** butonuna tıklayın
   - Sonra **UI.unity**'yi açın → **Add Open Scenes**
   - Sonra **GameScene.unity**'yi açın → **Add Open Scenes**
   - Son olarak **Outro.unity**'yi açın → **Add Open Scenes**

### 2.3 Sıralama Düzenleme

Sahneleri yukarı-aşağı sürükleyerek sıralayabilirsiniz:
- Intro → **0** (en üstte)
- UI → **1**
- GameScene → **2**
- Outro → **3** (en altta)

---

## 📋 ADIM 3: GameManager Inspector Ayarları

### 3.1 GameManager GameObject'ini Seçin

**UI sahnesinde** veya **oyun sahnesinde** GameManager GameObject'ini bulun ve seçin.

### 3.2 Inspector Ayarları

Inspector'da **GameManager.cs** component'inde:

- ✅ **Intro Scene Index**: `0`
- ✅ **UI Scene Index**: `1`
- ✅ **Game Scene Index**: `2`
- ✅ **Outro Scene Index**: `3`

---

## 📋 ADIM 4: Kontrol ve Test

### 4.1 Build Settings Kontrolü

**Build Profiles** penceresinde kontrol edin:
- ✅ 4 sahne var mı?
- ✅ Sıralama doğru mu? (0, 1, 2, 3)
- ✅ Hepsinde tik var mı? (aktif olduklarını gösterir)

### 4.2 Test

1. **Play** butonuna basın
2. **File** → **Build And Run** (veya `Ctrl + B`) ile build yapın
3. Oyunun doğru sahne sırasında başladığını kontrol edin

---

## ⚠️ ÖNEMLİ NOTLAR

### ✅ Sahne İsimleri

Sahne dosyalarınızın isimleri farklıysa:
- Build Settings'te doğru sahneleri eklediğinizden emin olun
- GameManager Inspector'da index'ler doğru olmalı (0, 1, 2, 3)

### ✅ Sahne Yolları

Eğer sahneler farklı klasörlerdeyse:
- **Project** panelinde sahneleri bulun
- **Build Profiles** penceresine sürükle-bırak yapın
- Yol önemli değil, sadece index önemli

### ✅ Build Yapmadan Test

Editörde test ederken:
- Doğrudan bir sahneyi açıp Play edebilirsiniz
- Ama gerçek build'de sahne sırası önemli

---

## 🎯 HIZLI KONTROL LİSTESİ

- [ ] Build Profiles penceresi açık (`Ctrl + Shift + B`)
- [ ] Intro sahnesi eklendi (Index 0)
- [ ] UI sahnesi eklendi (Index 1)
- [ ] Game sahnesi eklendi (Index 2)
- [ ] Outro sahnesi eklendi (Index 3)
- [ ] Sıralama doğru (0, 1, 2, 3)
- [ ] GameManager Inspector'da index'ler doğru (0, 1, 2, 3)

---

**Hazır! 🎉 Artık Build Settings hazır ve oyununuz doğru sahne sırasında başlayacak!**

