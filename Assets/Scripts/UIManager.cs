using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject panelAssetStore;
    public GameObject panelActionStore;
    public GameObject panelRounds;
    public GameObject panelGoals;

    [Header("Sidebar Buttons")]
    public Button btnAssetStore;
    public Button btnActionStore;
    public Button btnRounds;
    public Button btnGoals;

    [Header("Tab Colors")]
    public Color activeTabColor = new Color(0.15f, 0.39f, 0.92f); // Μπλε (#2563EB)
    public Color inactiveTabColor = new Color(0.15f, 0.15f, 0.17f); // Σκούρο (#27272A)

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Σύνδεση των OnClick listeners στα κουμπιά της Sidebar
        if (btnAssetStore != null) btnAssetStore.onClick.AddListener(ShowAssetStore);
        if (btnActionStore != null) btnActionStore.onClick.AddListener(ShowActionStore);
        if (btnRounds != null) btnRounds.onClick.AddListener(ShowRounds);
        if (btnGoals != null) btnGoals.onClick.AddListener(ShowGoals);

        // Προεπιλογή: Άνοιγμα του Asset Store στην αρχή
        ShowAssetStore();
    }

    public void ShowAssetStore()
    {
        SetActivePanel(panelAssetStore, btnAssetStore);
    }

    public void ShowActionStore()
    {
        SetActivePanel(panelActionStore, btnActionStore);
    }

    public void ShowRounds()
    {
        SetActivePanel(panelRounds, btnRounds);
    }

    public void ShowGoals()
    {
        SetActivePanel(panelGoals, btnGoals);
    }

    private void SetActivePanel(GameObject activePanel, Button activeButton)
    {
        // Εμφάνιση μόνο του ενεργού panel
        if (panelAssetStore) panelAssetStore.SetActive(panelAssetStore == activePanel);
        if (panelActionStore) panelActionStore.SetActive(panelActionStore == activePanel);
        if (panelRounds) panelRounds.SetActive(panelRounds == activePanel);
        if (panelGoals) panelGoals.SetActive(panelGoals == activePanel);

        // Αλλαγή χρώματος στα κουμπιά για να φαίνεται ποιο tab είναι επιλεγμένο
        SetButtonColor(btnAssetStore, btnAssetStore == activeButton);
        SetButtonColor(btnActionStore, btnActionStore == activeButton);
        SetButtonColor(btnRounds, btnRounds == activeButton);
        SetButtonColor(btnGoals, btnGoals == activeButton);
    }

    private void SetButtonColor(Button btn, bool isActive)
    {
        if (btn == null) return;
        Image img = btn.GetComponent<Image>();
        if (img != null)
        {
            img.color = isActive ? activeTabColor : inactiveTabColor;
        }
    }
}