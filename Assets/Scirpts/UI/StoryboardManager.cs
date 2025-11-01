using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace HalloweenJam.UI
{
    /// <summary>
    /// Storyboard ekranı yönetimi - Oyun hikayesini metinlerle anlatır
    /// </summary>
    public class StoryboardManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI storyText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipButton;

        [Header("Story Settings")]
        [SerializeField] private List<string> storyPages = new List<string>();
        [SerializeField] private float textDisplaySpeed = 0.05f; // Her karakter için süre

        private int currentPageIndex = 0;
        private bool isDisplayingText = false;
        private Coroutine displayTextCoroutine;

        private void Awake()
        {
            // Varsayılan hikaye metinleri (sonradan değiştirilebilir)
            if (storyPages.Count == 0)
            {
                storyPages.Add("Bir laboratuvarda hapsolmuş bir simbiyotsun...");
                storyPages.Add("En üst kattan kaçmak için aşağıya inmelisin.");
                storyPages.Add("Ancak dikkatli ol - renkli bölgelerdeki askerler seni fark ederse alarm çalar!");
                storyPages.Add("Askerlerin içine girerek onları ele geçirebilirsin. Ama doğru renk bölgesinde olmalısın...");
                storyPages.Add("Hazır mısın? Kaçış başlıyor!");
            }
        }

        private void Start()
        {
            SetupButtons();
        }

        private void SetupButtons()
        {
            if (nextButton != null)
                nextButton.onClick.AddListener(OnNextButtonClicked);

            if (skipButton != null)
                skipButton.onClick.AddListener(OnSkipButtonClicked);
        }

        /// <summary>
        /// Storyboard'u başlatır
        /// </summary>
        public void StartStoryboard()
        {
            currentPageIndex = 0;
            ShowPage(0);
        }

        private void ShowPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= storyPages.Count)
            {
                // Hikaye bitti, oyunu başlat
                StartGame();
                return;
            }

            currentPageIndex = pageIndex;
            
            // Next butonunu gizle (metin gösterilirken)
            if (nextButton != null)
                nextButton.gameObject.SetActive(false);

            // Metni göster
            if (displayTextCoroutine != null)
                StopCoroutine(displayTextCoroutine);
            
            displayTextCoroutine = StartCoroutine(DisplayTextCoroutine(storyPages[pageIndex]));
        }

        private IEnumerator DisplayTextCoroutine(string text)
        {
            isDisplayingText = true;
            
            if (storyText != null)
            {
                storyText.text = "";
                
                foreach (char c in text)
                {
                    storyText.text += c;
                    yield return new WaitForSeconds(textDisplaySpeed);
                }
            }

            isDisplayingText = false;
            
            // Metin gösterildi, Next butonunu göster
            if (nextButton != null)
                nextButton.gameObject.SetActive(true);
        }

        private void OnNextButtonClicked()
        {
            if (isDisplayingText)
            {
                // Metin gösteriliyorsa, hemen tamamla
                if (displayTextCoroutine != null)
                {
                    StopCoroutine(displayTextCoroutine);
                    isDisplayingText = false;
                    
                    if (storyText != null && currentPageIndex < storyPages.Count)
                        storyText.text = storyPages[currentPageIndex];
                    
                    if (nextButton != null)
                        nextButton.gameObject.SetActive(true);
                }
                return;
            }

            // Sonraki sayfaya geç
            ShowPage(currentPageIndex + 1);
        }

        private void OnSkipButtonClicked()
        {
            // Hikayeyi atla, oyunu başlat
            StartGame();
        }

        private void StartGame()
        {
            // Oyunu başlat
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
            }
            else
            {
                Debug.LogError("StoryboardManager: GameManager bulunamadı! Oyun başlatılamıyor.");
            }
        }

        private void OnDestroy()
        {
            if (nextButton != null)
                nextButton.onClick.RemoveAllListeners();
            if (skipButton != null)
                skipButton.onClick.RemoveAllListeners();
        }
    }
}

