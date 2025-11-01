#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;

namespace HalloweenJam.UI
{
    /// <summary>
    /// UITextStyleManager için Editor araçları
    /// Window menüsünden erişilebilir
    /// </summary>
    public class UITextStyleManagerEditor : EditorWindow
    {
        private TMP_FontAsset fontAsset;
        private Color textColor = Color.white;
        private Transform rootTransform;
        private bool includeInactive = false;

        [MenuItem("Window/UI Style Manager")]
        public static void ShowWindow()
        {
            GetWindow<UITextStyleManagerEditor>("UI Style Manager");
        }

        private void OnGUI()
        {
            GUILayout.Label("Tüm UI Text'lerine Font ve Renk Uygula", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Font seçimi
            fontAsset = EditorGUILayout.ObjectField("Font Asset", fontAsset, typeof(TMP_FontAsset), false) as TMP_FontAsset;

            // Renk seçimi
            textColor = EditorGUILayout.ColorField("Text Color", textColor);

            // Root transform (opsiyonel)
            rootTransform = EditorGUILayout.ObjectField("Root Transform (Opsiyonel)", rootTransform, typeof(Transform), true) as Transform;

            // Include inactive
            includeInactive = EditorGUILayout.Toggle("Include Inactive Objects", includeInactive);

            EditorGUILayout.Space();

            // Butonlar
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Font + Renk Uygula"))
            {
                ApplyStyles(true, true);
            }

            if (GUILayout.Button("Sadece Font"))
            {
                ApplyStyles(true, false);
            }

            if (GUILayout.Button("Sadece Renk"))
            {
                ApplyStyles(false, true);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "Font Asset: TextMeshPro'dan oluşturulmuş font asset'i olmalı.\n\n" +
                "Font Asset Oluşturma:\n" +
                "1. Font dosyanızı (TTF/OTF) Unity'ye import edin\n" +
                "2. Font dosyasına sağ tık → Create → TextMeshPro → Font Asset\n" +
                "3. Oluşturulan Font Asset'i yukarıya sürükleyin",
                MessageType.Info);
        }

        private void ApplyStyles(bool applyFont, bool applyColor)
        {
            if (applyFont && fontAsset == null)
            {
                EditorUtility.DisplayDialog("Hata", "Font Asset seçilmedi!", "Tamam");
                return;
            }

            Transform searchRoot = rootTransform != null ? rootTransform : FindObjectOfType<Canvas>()?.transform;
            
            if (searchRoot == null)
            {
                // Scene'deki tüm TextMeshProUGUI'leri bul
                TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>(includeInactive);
                
                int count = 0;
                foreach (TextMeshProUGUI text in allTexts)
                {
                    if (text == null) continue;

                    if (applyFont) text.font = fontAsset;
                    if (applyColor) text.color = textColor;

                    count++;
                    EditorUtility.SetDirty(text);
                }

                Debug.Log($"UI Style Manager: {count} text güncellendi.");
            }
            else
            {
                TextMeshProUGUI[] allTexts = searchRoot.GetComponentsInChildren<TextMeshProUGUI>(includeInactive);
                
                int count = 0;
                foreach (TextMeshProUGUI text in allTexts)
                {
                    if (text == null) continue;

                    if (applyFont) text.font = fontAsset;
                    if (applyColor) text.color = textColor;

                    count++;
                    EditorUtility.SetDirty(text);
                }

                Debug.Log($"UI Style Manager: {count} text güncellendi ({searchRoot.name} altında).");
            }

            AssetDatabase.SaveAssets();
        }
    }
}
#endif

