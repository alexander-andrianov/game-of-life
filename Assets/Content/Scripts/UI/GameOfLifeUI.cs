using UnityEngine;
using UnityEngine.UI;
using Content.Scripts.ECS;
using UnityEngine.AddressableAssets;

namespace Content.Scripts.UI
{
    public class GameOfLifeUI : MonoBehaviour
    {
        private GameSettings gameSettings;
        private Button speedUpBtn, slowDownBtn, restartBtn, pauseBtn;
        private Text speedText, pauseBtnText;

        private bool paused = false;
        private float speed = 1f;

        private async void Start()
        {
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                var canvasGO = new GameObject("Canvas", typeof(Canvas));

                canvas = canvasGO.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
            }

            gameSettings = await Addressables.LoadAssetAsync<GameSettings>("game-settings-config").Task.ConfigureAwait(true);
            speed = gameSettings.SimulationSpeed;

            CreateButton(canvas.transform, "SpeedUp", "Faster", new Vector2(120, -40), OnSpeedUp);
            CreateButton(canvas.transform, "SlowDown", "Slower", new Vector2(120, -80), OnSlowDown);
            CreateButton(canvas.transform, "Restart", "Restart", new Vector2(120, -120), OnRestart);

            pauseBtn = CreateButton(canvas.transform, "Pause", "Pause", new Vector2(120, -160), OnPause);
            pauseBtnText = pauseBtn.GetComponentInChildren<Text>();
            speedText = CreateText(canvas.transform, "SpeedText", $"Speed: {speed}x", new Vector2(120, -200));
        }

        private void OnSpeedUp() {
            speed = Mathf.Min(speed * 2f, 64f);
            UpdateSpeed();
        }

        private void OnSlowDown() {
            speed = Mathf.Max(speed * 0.5f, 0.5f);
            UpdateSpeed();
        }

        private void OnRestart() { GameOfLifeSystem.RequestRestart(); }
        private void OnPause() {
            paused = !paused;
            Time.timeScale = paused ? 0f : 1f;
            
            if (pauseBtnText) 
                pauseBtnText.text = paused ? "Play" : "Pause";
        }

        private void UpdateSpeed()
        {
            GameOfLifeSystem.SetSimulationSpeed(speed);
            if (speedText) speedText.text = $"Speed: {speed:0.##}x";
        }

        private Button CreateButton(Transform parent, string name, string text, Vector2 anchoredPos, UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Button), typeof(Image));
            go.transform.SetParent(parent, false);
            
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(180, 32);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = anchoredPos;

            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(onClick);

            var img = go.GetComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            var txtGO = new GameObject("Text", typeof(RectTransform), typeof(Text));
            txtGO.transform.SetParent(go.transform, false);

            var txt = txtGO.GetComponent<Text>();
            txt.text = text;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.rectTransform.anchorMin = Vector2.zero;
            txt.rectTransform.anchorMax = Vector2.one;
            txt.rectTransform.offsetMin = Vector2.zero;
            txt.rectTransform.offsetMax = Vector2.zero;

            return btn;
        }

        private Text CreateText(Transform parent, string name, string text, Vector2 anchoredPos)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(180, 32);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = anchoredPos;

            var txt = go.GetComponent<Text>();
            txt.text = text;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;

            return txt;
        }
    }
} 