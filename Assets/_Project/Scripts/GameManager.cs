using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Economy")]
    public int startGold = 100;
    public int Gold { get; private set; }

    [Header("Game State")]
    public int wavesToWin = 5;
    public bool createRuntimeHud = true;
    public string controlsMessage = "WASD Move\nSpace Shoot\nClick pads to build";

    [Header("Demo Camera")]
    public bool configureCameraOnStart = true;
    public Vector3 cameraPosition = new Vector3(3f, 8.5f, -7.5f);
    public Vector3 cameraLookAt = new Vector3(3f, 0f, 4f);
    public float cameraFieldOfView = 55f;

    [Header("UI")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI coreHealthText;
    public TextMeshProUGUI enemiesText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI controlsText;
    public TextMeshProUGUI statusText;
    public Button restartButton;

    public bool IsGameOver { get; private set; }
    public int CurrentWave { get; private set; } = 1;
    public int EnemiesAlive { get; private set; }
    public int EnemiesRemainingInWave { get; private set; }
    public int Kills { get; private set; }
    public int Score => Kills * 100 + Gold;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Time.timeScale = 1f;
        Gold = startGold;
        if (createRuntimeHud)
            EnsureRuntimeHud();

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);

        UpdateUI();
    }

    private void Start()
    {
        if (configureCameraOnStart)
            ConfigureDemoCamera();

        CoreHealth core = FindFirstObjectByType<CoreHealth>();
        if (core != null)
            UpdateCoreHealth(core.GetHp(), core.maxHp);
    }

    public bool TrySpend(int amount)
    {
        if (IsGameOver) return false;
        if (Gold < amount) return false;
        Gold -= amount;
        UpdateUI();
        return true;
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        UpdateUI();
    }

    public void AddKill()
    {
        Kills++;
        UpdateUI();
    }

    public void SetWave(int wave, int totalEnemies)
    {
        CurrentWave = wave;
        EnemiesRemainingInWave = totalEnemies;
        UpdateUI();
    }

    public void RegisterEnemySpawned()
    {
        EnemiesAlive++;
        UpdateUI();
    }

    public void RegisterEnemyRemoved()
    {
        EnemiesAlive = Mathf.Max(0, EnemiesAlive - 1);
        EnemiesRemainingInWave = Mathf.Max(0, EnemiesRemainingInWave - 1);
        UpdateUI();
    }

    public void UpdateCoreHealth(int hp, int maxHp)
    {
        if (coreHealthText != null)
            coreHealthText.text = $"Core: {Mathf.Max(0, hp)} / {maxHp}";
    }

    public void ShowWaveBreak(float seconds)
    {
        if (statusText != null && !IsGameOver)
            statusText.text = $"Next wave in {Mathf.CeilToInt(seconds)}s";
    }

    public void ShowWaveBreak(string message)
    {
        if (statusText != null && !IsGameOver)
            statusText.text = message;
    }

    public void ClearStatus()
    {
        if (statusText != null && !IsGameOver)
            statusText.text = "";
    }

    public void WinGame()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Time.timeScale = 0f;

        if (statusText != null)
            statusText.text = "Victory! Your core survived.";
        if (restartButton != null)
            restartButton.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Time.timeScale = 0f;

        if (statusText != null)
            statusText.text = "Game Over";
        if (restartButton != null)
            restartButton.gameObject.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ConfigureDemoCamera()
    {
        Camera camera = Camera.main;
        if (camera == null) return;

        camera.transform.position = cameraPosition;
        camera.transform.LookAt(cameraLookAt);
        camera.fieldOfView = cameraFieldOfView;
    }

    public void UpdateUI()
    {
        if (goldText != null)
            goldText.text = $"Gold: {Gold}";
        if (waveText != null)
            waveText.text = $"Wave: {CurrentWave} / {wavesToWin}";
        if (enemiesText != null)
            enemiesText.text = $"Enemies: {EnemiesAlive} alive, {EnemiesRemainingInWave} left";
        if (killsText != null)
            killsText.text = $"Score: {Score}   Kills: {Kills}";
        if (controlsText != null)
            controlsText.text = controlsMessage;
    }

    private void EnsureRuntimeHud()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Runtime HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
        }
        else if (canvas.GetComponent<GraphicRaycaster>() == null)
        {
            canvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        EnsureEventSystem();

        FontStyles style = FontStyles.Bold;
        if (goldText == null)
            goldText = CreateHudText(canvas.transform, "Gold Text", new Vector2(24f, -24f), style);
        if (waveText == null)
            waveText = CreateHudText(canvas.transform, "Wave Text", new Vector2(24f, -64f), style);
        if (coreHealthText == null)
            coreHealthText = CreateHudText(canvas.transform, "Core Health Text", new Vector2(24f, -104f), style);
        if (enemiesText == null)
            enemiesText = CreateHudText(canvas.transform, "Enemies Text", new Vector2(24f, -144f), style);
        if (killsText == null)
            killsText = CreateHudText(canvas.transform, "Kills Text", new Vector2(24f, -184f), style);
        if (controlsText == null)
            controlsText = CreateHudText(canvas.transform, "Controls Text", new Vector2(24f, -224f), style);
        if (statusText == null)
            statusText = CreateHudText(canvas.transform, "Status Text", new Vector2(0f, -30f), style, TextAlignmentOptions.Top);
        ConfigureStatusText();
        ConfigureHudText(goldText, new Vector2(24f, -24f));
        ConfigureHudText(waveText, new Vector2(24f, -52f));
        ConfigureHudText(coreHealthText, new Vector2(24f, -80f));
        ConfigureHudText(enemiesText, new Vector2(24f, -108f));
        ConfigureHudText(killsText, new Vector2(24f, -136f), 20f, new Vector2(380f, 28f));
        ConfigureTopRightText(controlsText, new Vector2(-24f, -24f));

        if (restartButton == null)
            restartButton = CreateRestartButton(canvas.transform);
        restartButton.gameObject.SetActive(false);
    }

    private TextMeshProUGUI CreateHudText(Transform parent, string objectName, Vector2 anchoredPosition, FontStyles style, TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = alignment == TextAlignmentOptions.Top ? new Vector2(0.5f, 1f) : new Vector2(0f, 1f);
        rect.anchorMax = rect.anchorMin;
        rect.pivot = alignment == TextAlignmentOptions.Top ? new Vector2(0.5f, 1f) : new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = alignment == TextAlignmentOptions.Top ? new Vector2(900f, 70f) : new Vector2(420f, 36f);

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.fontSize = alignment == TextAlignmentOptions.Top ? 36f : 28f;
        text.fontStyle = style;
        text.alignment = alignment;
        text.color = Color.white;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        return text;
    }

    private void ConfigureHudText(TextMeshProUGUI text, Vector2 anchoredPosition, float fontSize = 22f, Vector2? size = null)
    {
        if (text == null) return;

        RectTransform rect = text.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = rect.anchorMin;
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size ?? new Vector2(420f, 28f);

        text.fontSize = fontSize;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.color = Color.white;
        text.textWrappingMode = TextWrappingModes.NoWrap;
    }

    private void ConfigureTopRightText(TextMeshProUGUI text, Vector2 anchoredPosition)
    {
        if (text == null) return;

        RectTransform rect = text.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = rect.anchorMin;
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(300f, 76f);

        text.fontSize = 16f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.TopRight;
        text.color = new Color(0.9f, 0.96f, 1f);
        text.textWrappingMode = TextWrappingModes.Normal;
    }

    private void ConfigureStatusText()
    {
        if (statusText == null) return;

        RectTransform rect = statusText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = rect.anchorMin;
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -14f);
        rect.sizeDelta = new Vector2(460f, 34f);

        statusText.text = "";
        statusText.fontSize = 18f;
        statusText.fontStyle = FontStyles.Bold;
        statusText.alignment = TextAlignmentOptions.Top;
        statusText.color = new Color(1f, 0.92f, 0.35f);
        statusText.textWrappingMode = TextWrappingModes.Normal;
    }

    private Button CreateRestartButton(Transform parent)
    {
        GameObject buttonObject = new GameObject("Restart Button", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, -80f);
        rect.sizeDelta = new Vector2(240f, 70f);

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.12f, 0.9f);

        Button button = buttonObject.GetComponent<Button>();

        TextMeshProUGUI label = CreateHudText(buttonObject.transform, "Label", Vector2.zero, FontStyles.Bold, TextAlignmentOptions.Center);
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.pivot = new Vector2(0.5f, 0.5f);
        labelRect.anchoredPosition = Vector2.zero;
        labelRect.sizeDelta = Vector2.zero;
        label.fontSize = 30f;
        label.text = "Restart";

        return button;
    }

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
            return;

        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
    }
}
