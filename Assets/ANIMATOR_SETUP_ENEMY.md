# Enemy Animator Controller Kurulum Rehberi

## Adım 1: Animator Controller Oluşturma
1. Project penceresinde sağ tık → **Create → Animator Controller**
2. İsmini `Enemy_Animator` yap
3. Enemy GameObject'ine bağla (Animator component'inde Controller olarak seç)

## Adım 2: Parametreler (Parameters Tab)
1. **isMoving** → Bool tipinde
2. **attack** → Trigger tipinde
3. **death** → Trigger tipinde

## Adım 3: State'ler Oluşturma
1. **Idle** state oluştur (idle animasyonunu sürükle-bırak)
2. **Move** state oluştur (move animasyonunu sürükle-bırak)
3. **Fire** state oluştur (fire animasyonunu sürükle-bırak)
4. **Death** state oluştur (death animasyonunu sürükle-bırak)

## Adım 4: Default State
- **Idle** state'ini seç → sağ tık → **Set as Layer Default State**

## Adım 5: Transition'lar

### Idle → Move
- **Conditions**: `isMoving == true`

### Move → Idle
- **Conditions**: `isMoving == false`

### ANY STATE → Fire
- **Conditions**: `attack` (trigger)
- **Settings**:
  - Has Exit Time: ❌ (kapalı)
  - Transition Duration: 0.1s
  - Interruption Source: None

### Fire → Idle
- **Conditions**: (boş - otomatik geçiş)
- **Settings**:
  - Has Exit Time: ✅ (açık)
  - Exit Time: 1.0 (animasyon bitince)
  - Transition Duration: 0.1s

### Fire → Move
- **Conditions**: `isMoving == true`
- **Settings**:
  - Has Exit Time: ✅ (açık)
  - Exit Time: 1.0 (animasyon bitince)
  - Transition Duration: 0.1s

### ANY STATE → Death
- **Conditions**: `death` (trigger)
- **Settings**:
  - Has Exit Time: ❌ (kapalı)
  - Transition Duration: 0.1s
  - Interruption Source: None

### Death → (Hiçbir yere geçiş yok - ölüm animasyonu son state)

## Önemli Notlar:
1. **ANY STATE → Death**: Bu transition tüm state'lerden ölüm animasyonuna geçişi sağlar
2. **ANY STATE → Fire**: Bu transition tüm state'lerden ateş animasyonuna geçişi sağlar
3. **Fire animasyonu**: Ateş etme animasyonu bittikten sonra otomatik olarak Idle veya Move'a döner (isMoving durumuna göre)
4. **Death animasyonu**: Ölüm animasyonu son state, başka bir yere geçiş yok

## Transition Sırası (Öncelik):
1. **Death** her zaman en yüksek öncelikte (ANY STATE → Death)
2. **Fire** ikinci öncelikte (ANY STATE → Fire)
3. **Move/Idle** arasındaki geçişler normal flow

