using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalsController : MonoBehaviour
{
    [Header("List Column")]
    public Transform listContentContainer;          // GoalListColumn -> Scroll View -> Viewport -> Content
    public GameObject goalListItemPrefab;           // Assets/Prefabs/AssetListItem
    public Button btnNewGoal;                       // Btn_NewGoal

    [Header("Form Fields")]
    public TMP_Text headerFormText;                 // Header_Form
    public TMP_InputField inputGoalID;              // Input_GoalID
    public TMP_Dropdown dropdownGoalType;           // Dropdown_GoalType
    public TMP_InputField inputRound;               // Input_Round (-1 = Global, 0 = Round 1, etc.)
    public TMP_Dropdown dropdownProperty;           // Dropdown_Property
    public TMP_Dropdown dropdownObjectType;         // Dropdown_ObjectType
    public TMP_InputField inputTargetCount;         // Input_TargetCount (Threshold or Number of actions)

    [Header("Form Buttons")]
    public Button btnSaveGoal;                      // Btn_SaveGoal
    public Button btnDeleteGoal;                    // Btn_DeleteGoal

    private Goal currentSelectedGoal = null;
    private Dictionary<Goal, GameObject> goalListItems = new Dictionary<Goal, GameObject>();
    private GameObject currentSelectedButtonObj = null;

    private List<GoalType> goalTypeValues = new List<GoalType>();
    private List<Property> propertyValues = new List<Property>();
    private List<AssetType> objectTypeValues = new List<AssetType>();

    private void Start()
    {
        InitializeDropdowns();

        if (btnNewGoal != null) btnNewGoal.onClick.AddListener(OnCreateNewGoal);
        if (btnSaveGoal != null) btnSaveGoal.onClick.AddListener(OnSaveCurrentGoal);
        if (btnDeleteGoal != null) btnDeleteGoal.onClick.AddListener(OnDeleteCurrentGoal);

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

    private void InitializeDropdowns()
    {
        // 1. GoalType Dropdown
        if (dropdownGoalType != null)
        {
            dropdownGoalType.ClearOptions();
            goalTypeValues.Clear();
            List<string> typeNames = new List<string>();
            foreach (GoalType val in Enum.GetValues(typeof(GoalType)))
            {
                goalTypeValues.Add(val);
                typeNames.Add(val.ToString());
            }
            dropdownGoalType.AddOptions(typeNames);
        }

        // 2. Property Dropdown (INSPECT, ISOLATE, etc.)
        if (dropdownProperty != null)
        {
            dropdownProperty.ClearOptions();
            propertyValues.Clear();
            List<string> propNames = new List<string>();
            foreach (Property val in Enum.GetValues(typeof(Property)))
            {
                propertyValues.Add(val);
                propNames.Add(val.ToString());
            }
            dropdownProperty.AddOptions(propNames);
        }

        // 3. ObjectType Dropdown
        if (dropdownObjectType != null)
        {
            dropdownObjectType.ClearOptions();
            objectTypeValues.Clear();
            List<string> objNames = new List<string>();
            foreach (AssetType val in Enum.GetValues(typeof(AssetType)))
            {
                objectTypeValues.Add(val);
                objNames.Add(val.ToString());
            }
            dropdownObjectType.AddOptions(objNames);
        }
    }

    public void RefreshList()
    {
        if (listContentContainer == null) return;

        foreach (Transform child in listContentContainer)
        {
            Destroy(child.gameObject);
        }
        goalListItems.Clear();

        var scenario = ScenarioManager.Instance?.currentScenario;
        if (scenario == null || scenario.goals == null) return;

        Goal firstGoal = null;

        foreach (var goal in scenario.goals)
        {
            if (firstGoal == null) firstGoal = goal;
            CreateListItem(goal);
        }

        if (firstGoal != null)
        {
            SelectGoal(firstGoal);
        }
        else
        {
            ClearForm();
        }
    }

    private void CreateListItem(Goal goal)
    {
        if (goalListItemPrefab == null || listContentContainer == null) return;

        GameObject itemObj = Instantiate(goalListItemPrefab, listContentContainer);
        goalListItems[goal] = itemObj;

        UpdateListItemUI(goal, itemObj);

        Button btn = itemObj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => SelectGoal(goal));
        }
    }

    private void UpdateListItemUI(Goal goal, GameObject itemObj)
    {
        TMP_Text txt = itemObj.GetComponentInChildren<TMP_Text>();
        if (txt != null)
        {
            string roundLabel = goal.round >= 0 ? $"Round {goal.round + 1}" : "Global Goal";
            txt.text = $"<b>{goal.id}</b>\n<size=11><color=#9CA3AF>{goal.type} | {roundLabel}</color></size>";
        }
    }

    public void SelectGoal(Goal goal)
    {
        currentSelectedGoal = goal;

        // Χρωματισμός επιλεγμένου κουμπιού στη λίστα
        if (currentSelectedButtonObj != null)
        {
            var prevImg = currentSelectedButtonObj.GetComponent<Image>();
            if (prevImg != null) prevImg.color = new Color(0.16f, 0.16f, 0.19f); // Σκούρο #2A2A30
        }

        if (goalListItems.TryGetValue(goal, out GameObject btnObj))
        {
            currentSelectedButtonObj = btnObj;
            var currImg = currentSelectedButtonObj.GetComponent<Image>();
            if (currImg != null) currImg.color = new Color(0.15f, 0.39f, 0.92f); // Μπλε #2563EB
        }

        // Γέμισμα πεδίων φόρμας
        if (headerFormText != null) headerFormText.text = $"EDIT GOAL: <color=#60A5FA>{goal.id}</color>";
        if (inputGoalID != null) inputGoalID.text = goal.id ?? "";
        if (inputRound != null) inputRound.text = goal.round.ToString();

        // GoalType Dropdown
        int typeIndex = goalTypeValues.IndexOf(goal.type);
        if (dropdownGoalType != null) dropdownGoalType.value = typeIndex >= 0 ? typeIndex : 0;

        // Property Dropdown
        int propIndex = propertyValues.IndexOf(goal.property);
        if (dropdownProperty != null) dropdownProperty.value = propIndex >= 0 ? propIndex : 0;

        // ObjectType Dropdown
        AssetType primaryObjType = (goal.objectTypes != null && goal.objectTypes.Count > 0) ? goal.objectTypes[0] : AssetType.INFO;
        int objIndex = objectTypeValues.IndexOf(primaryObjType);
        if (dropdownObjectType != null) dropdownObjectType.value = objIndex >= 0 ? objIndex : 0;

        // Target Count / Threshold (από SEF ή default)
        int targetValue = 0;
        if (goal.sefs != null && goal.sefs.Count > 0)
        {
            var sef = goal.sefs[0];
            targetValue = sef.num_of_actions > 0 ? sef.num_of_actions : sef.threshold;
        }
        if (inputTargetCount != null) inputTargetCount.text = targetValue.ToString();
    }

    private void OnSaveCurrentGoal()
    {
        if (currentSelectedGoal == null) return;

        currentSelectedGoal.id = inputGoalID != null ? inputGoalID.text.Trim() : currentSelectedGoal.id;

        if (inputRound != null && int.TryParse(inputRound.text, out int rnd))
        {
            currentSelectedGoal.round = rnd;
        }

        if (dropdownGoalType != null && dropdownGoalType.value < goalTypeValues.Count)
        {
            currentSelectedGoal.type = goalTypeValues[dropdownGoalType.value];
        }

        if (dropdownProperty != null && dropdownProperty.value < propertyValues.Count)
        {
            currentSelectedGoal.property = propertyValues[dropdownProperty.value];
        }

        if (dropdownObjectType != null && dropdownObjectType.value < objectTypeValues.Count)
        {
            if (currentSelectedGoal.objectTypes == null) currentSelectedGoal.objectTypes = new List<AssetType>();
            currentSelectedGoal.objectTypes.Clear();
            currentSelectedGoal.objectTypes.Add(objectTypeValues[dropdownObjectType.value]);
        }

        // Save Target Count into SEF
        if (inputTargetCount != null && int.TryParse(inputTargetCount.text, out int targetVal))
        {
            if (currentSelectedGoal.sefs == null) currentSelectedGoal.sefs = new List<SEF>();
            if (currentSelectedGoal.sefs.Count == 0)
            {
                currentSelectedGoal.sefs.Add(new SEF { relatedGoalType = currentSelectedGoal.type });
            }
            var sef = currentSelectedGoal.sefs[0];
            sef.relatedGoalType = currentSelectedGoal.type;
            sef.num_of_actions = targetVal;
            sef.threshold = targetVal;
            sef.property = currentSelectedGoal.property;
        }

        // Ενημέρωση του στοιχείου στη λίστα
        if (goalListItems.TryGetValue(currentSelectedGoal, out GameObject itemObj))
        {
            UpdateListItemUI(currentSelectedGoal, itemObj);
        }

        if (headerFormText != null) headerFormText.text = $"EDIT GOAL: <color=#60A5FA>{currentSelectedGoal.id}</color>";

        Debug.Log($"<color=green>[Goals] Αποθηκεύτηκαν οι αλλαγές για τον στόχο: {currentSelectedGoal.id}</color>");
    }

    private void OnCreateNewGoal()
    {
        var scenario = ScenarioManager.Instance?.currentScenario;
        if (scenario == null) return;

        if (scenario.goals == null) scenario.goals = new List<Goal>();

        int newIndex = scenario.goals.Count + 1;
        Goal newGoal = new Goal
        {
            id = $"base-goal-{newIndex}",
            type = GoalType.ACTIONS_SPECIFIC_TYPE_COUNT,
            round = -1, // Global
            subjectType = AssetType.LEARNER,
            property = Property.INSPECT,
            objectTypes = new List<AssetType> { AssetType.WORKSTATION },
            sefs = new List<SEF>
            {
                new SEF
                {
                    relatedGoalType = GoalType.ACTIONS_SPECIFIC_TYPE_COUNT,
                    property = Property.INSPECT,
                    num_of_actions = 3,
                    threshold = 3
                }
            },
            achieved = false
        };

        scenario.goals.Add(newGoal);
        CreateListItem(newGoal);
        SelectGoal(newGoal);

        Debug.Log($"<color=cyan>[Goals] Δημιουργήθηκε νέος στόχος: {newGoal.id}</color>");
    }

    private void OnDeleteCurrentGoal()
    {
        if (currentSelectedGoal == null) return;

        var scenario = ScenarioManager.Instance?.currentScenario;
        if (scenario == null || scenario.goals == null) return;

        string deletedId = currentSelectedGoal.id;
        scenario.goals.Remove(currentSelectedGoal);

        if (goalListItems.TryGetValue(currentSelectedGoal, out GameObject itemObj))
        {
            Destroy(itemObj);
            goalListItems.Remove(currentSelectedGoal);
        }

        currentSelectedGoal = null;

        if (scenario.goals.Count > 0)
        {
            SelectGoal(scenario.goals[0]);
        }
        else
        {
            ClearForm();
        }

        Debug.Log($"<color=yellow>[Goals] Διαγράφηκε ο στόχος: {deletedId}</color>");
    }

    private void ClearForm()
    {
        if (headerFormText != null) headerFormText.text = "EDIT GOAL";
        if (inputGoalID != null) inputGoalID.text = "";
        if (inputRound != null) inputRound.text = "-1";
        if (inputTargetCount != null) inputTargetCount.text = "0";
    }
}
