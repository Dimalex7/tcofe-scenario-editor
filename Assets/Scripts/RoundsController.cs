using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundsController : MonoBehaviour
{
    [Header("List Column")]
    public Transform listContentContainer;          // RoundListColumn -> Scroll View -> Viewport -> Content
    public GameObject roundListItemPrefab;          // Το Prefab Assets/Prefabs/AssetListItem
    public Button btnNewRound;                      // Btn_NewRound

    [Header("Form Fields - Info")]
    public TMP_Text headerFormText;                 // Header_Form
    public TMP_InputField inputRoundID;             // Input_RoundID
    public TMP_InputField inputRoundLabel;          // Input_RoundLabel

    [Header("Form Fields - Metrics")]
    public TMP_InputField inputAP;                  // Input_AP
    public TMP_InputField inputBudget;              // Input_Budget
    public TMP_InputField inputCS;                  // Input_CS
    public TMP_InputField inputBI;                  // Input_BI
    public TMP_InputField inputTrust;               // Input_Trust
    public TMP_InputField inputDeadline;            // Input_Deadline

    [Header("Form Buttons")]
    public Button btnSaveRound;                     // Btn_SaveRound
    public Button btnDeleteRound;                   // Btn_DeleteRound

    private StepRound currentSelectedRound = null;
    private Dictionary<StepRound, GameObject> roundListItems = new Dictionary<StepRound, GameObject>();
    private GameObject currentSelectedButtonObj = null;

    private void Start()
    {
        if (btnNewRound != null) btnNewRound.onClick.AddListener(OnCreateNewRound);
        if (btnSaveRound != null) btnSaveRound.onClick.AddListener(OnSaveCurrentRound);
        if (btnDeleteRound != null) btnDeleteRound.onClick.AddListener(OnDeleteCurrentRound);

        if (ScenarioManager.Instance != null)
        {
            ScenarioManager.Instance.OnScenarioLoaded += RefreshList;
            if (ScenarioManager.Instance.currentScenario != null)
            {
                RefreshList();
            }
        }
    }

    private void OnDestroy()
    {
        if (ScenarioManager.Instance != null)
        {
            ScenarioManager.Instance.OnScenarioLoaded -= RefreshList;
        }
    }

    public void RefreshList()
    {
        if (listContentContainer == null) return;

        foreach (Transform child in listContentContainer)
        {
            Destroy(child.gameObject);
        }
        roundListItems.Clear();

        var scenario = ScenarioManager.Instance?.currentScenario;
        if (scenario == null || scenario.step_rounds == null) return;

        StepRound firstRound = null;

        foreach (var round in scenario.step_rounds)
        {
            if (firstRound == null) firstRound = round;
            CreateListItem(round);
        }

        if (firstRound != null)
        {
            SelectRound(firstRound);
        }
        else
        {
            ClearForm();
        }
    }

    private void CreateListItem(StepRound round)
    {
        if (roundListItemPrefab == null || listContentContainer == null) return;

        GameObject itemObj = Instantiate(roundListItemPrefab, listContentContainer);
        roundListItems[round] = itemObj;

        UpdateListItemUI(round, itemObj);

        Button btn = itemObj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => SelectRound(round));
        }
    }

    private void UpdateListItemUI(StepRound round, GameObject itemObj)
    {
        TMP_Text txt = itemObj.GetComponentInChildren<TMP_Text>();
        if (txt != null)
        {
            string displayLabel = string.IsNullOrWhiteSpace(round.label) ? "Unnamed Round" : round.label;
            int ap = round.stepMetrics != null ? round.stepMetrics.ActionPoints : 0;
            int cs = round.stepMetrics != null ? round.stepMetrics.CS : 0;
            txt.text = $"<b>{displayLabel}</b>\n<size=11><color=#9CA3AF>{round.id} | AP: {ap} | CS: {cs}</color></size>";
        }
    }

    public void SelectRound(StepRound round)
    {
        currentSelectedRound = round;

        // Χρωματισμός επιλεγμένου κουμπιού στη λίστα
        if (currentSelectedButtonObj != null)
        {
            var prevImg = currentSelectedButtonObj.GetComponent<Image>();
            if (prevImg != null) prevImg.color = new Color(0.16f, 0.16f, 0.19f); // Σκούρο #2A2A30
        }

        if (roundListItems.TryGetValue(round, out GameObject btnObj))
        {
            currentSelectedButtonObj = btnObj;
            var currImg = currentSelectedButtonObj.GetComponent<Image>();
            if (currImg != null) currImg.color = new Color(0.15f, 0.39f, 0.92f); // Μπλε #2563EB
        }

        if (round.stepMetrics == null)
        {
            round.stepMetrics = new Metrics();
        }

        // Γέμισμα των πεδίων της φόρμας
        if (headerFormText != null) headerFormText.text = $"EDIT STEP ROUND: <color=#60A5FA>{round.id}</color>";
        if (inputRoundID != null) inputRoundID.text = round.id ?? "";
        if (inputRoundLabel != null) inputRoundLabel.text = round.label ?? "";

        // Γέμισμα των Metrics
        if (inputAP != null) inputAP.text = round.stepMetrics.ActionPoints.ToString();
        if (inputBudget != null) inputBudget.text = round.stepMetrics.budget.ToString();
        if (inputCS != null) inputCS.text = round.stepMetrics.CS.ToString();
        if (inputBI != null) inputBI.text = round.stepMetrics.BI.ToString();
        if (inputTrust != null) inputTrust.text = round.stepMetrics.trust.ToString();
        if (inputDeadline != null) inputDeadline.text = round.stepMetrics.deadline.ToString();
    }

    private void OnSaveCurrentRound()
    {
        if (currentSelectedRound == null) return;

        currentSelectedRound.id = inputRoundID != null ? inputRoundID.text.Trim() : currentSelectedRound.id;
        currentSelectedRound.label = inputRoundLabel != null ? inputRoundLabel.text.Trim() : currentSelectedRound.label;

        if (currentSelectedRound.stepMetrics == null)
        {
            currentSelectedRound.stepMetrics = new Metrics();
        }

        if (inputAP != null && int.TryParse(inputAP.text, out int ap)) currentSelectedRound.stepMetrics.ActionPoints = ap;
        if (inputBudget != null && int.TryParse(inputBudget.text, out int budget)) currentSelectedRound.stepMetrics.budget = budget;
        if (inputCS != null && int.TryParse(inputCS.text, out int cs)) currentSelectedRound.stepMetrics.CS = cs;
        if (inputBI != null && int.TryParse(inputBI.text, out int bi)) currentSelectedRound.stepMetrics.BI = bi;
        if (inputTrust != null && int.TryParse(inputTrust.text, out int trust)) currentSelectedRound.stepMetrics.trust = trust;
        if (inputDeadline != null && int.TryParse(inputDeadline.text, out int deadline)) currentSelectedRound.stepMetrics.deadline = deadline;

        // Ενημέρωση του στοιχείου στη λίστα
        if (roundListItems.TryGetValue(currentSelectedRound, out GameObject itemObj))
        {
            UpdateListItemUI(currentSelectedRound, itemObj);
        }

        if (headerFormText != null) headerFormText.text = $"EDIT STEP ROUND: <color=#60A5FA>{currentSelectedRound.id}</color>";

        Debug.Log($"<color=green>[Rounds] Αποθηκεύτηκαν οι αλλαγές για τον γύρο: {currentSelectedRound.id} ({currentSelectedRound.label})</color>");
    }

    private void OnCreateNewRound()
    {
        var scenario = ScenarioManager.Instance?.currentScenario;
        if (scenario == null) return;

        if (scenario.step_rounds == null) scenario.step_rounds = new List<StepRound>();

        int newIndex = scenario.step_rounds.Count + 1;
        StepRound newRound = new StepRound
        {
            id = $"round-{newIndex}",
            label = $"Round {newIndex}",
            stepMetrics = new Metrics
            {
                ActionPoints = 10,
                budget = 5000,
                CS = 50,
                BI = 80,
                trust = 70,
                deadline = 0
            },
            assets = new List<Asset>(),
            goals = new List<Goal>()
        };

        scenario.step_rounds.Add(newRound);
        CreateListItem(newRound);
        SelectRound(newRound);

        Debug.Log($"<color=cyan>[Rounds] Δημιουργήθηκε νέος γύρος: {newRound.id}</color>");
    }

    private void OnDeleteCurrentRound()
    {
        if (currentSelectedRound == null) return;

        var scenario = ScenarioManager.Instance?.currentScenario;
        if (scenario == null || scenario.step_rounds == null) return;

        string deletedId = currentSelectedRound.id;
        scenario.step_rounds.Remove(currentSelectedRound);

        if (roundListItems.TryGetValue(currentSelectedRound, out GameObject itemObj))
        {
            Destroy(itemObj);
            roundListItems.Remove(currentSelectedRound);
        }

        currentSelectedRound = null;

        if (scenario.step_rounds.Count > 0)
        {
            SelectRound(scenario.step_rounds[0]);
        }
        else
        {
            ClearForm();
        }

        Debug.Log($"<color=yellow>[Rounds] Διαγράφηκε ο γύρος: {deletedId}</color>");
    }

    private void ClearForm()
    {
        if (headerFormText != null) headerFormText.text = "EDIT STEP ROUND";
        if (inputRoundID != null) inputRoundID.text = "";
        if (inputRoundLabel != null) inputRoundLabel.text = "";
        if (inputAP != null) inputAP.text = "0";
        if (inputBudget != null) inputBudget.text = "0";
        if (inputCS != null) inputCS.text = "0";
        if (inputBI != null) inputBI.text = "0";
        if (inputTrust != null) inputTrust.text = "0";
        if (inputDeadline != null) inputDeadline.text = "0";
    }
}
