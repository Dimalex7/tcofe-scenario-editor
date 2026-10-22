using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionStoreController : MonoBehaviour
{
    [Header("List Column")]
    public Transform listContentContainer;          // ActionScrollView -> Viewport -> Content
    public GameObject actionListItemPrefab;         // Shared prefab from Assets/Prefabs/
    public Button btnNewAction;                     // Btn_NewAction

    [Header("Form Fields")]
    public TMP_Text headerFormText;                 // Header_Form
    public TMP_InputField inputID;                  // Input_ActionID
    public TMP_InputField inputTitle;               // Input_ActionTitle
    public TMP_Dropdown dropdownProperty;           // Dropdown_Property
    public TMP_Dropdown dropdownSubjectType;        // Dropdown_SubjectType
    public TMP_Dropdown dropdownObjectType;         // Dropdown_ObjectType
    public TMP_InputField inputAPCost;              // Input_APCost
    public TMP_InputField inputBudgetCost;          // Input_BudgetCost
    public TMP_Dropdown dropdownRepeatMode;         // Dropdown_RepeatMode

    [Header("Form Buttons")]
    public Button btnSaveAction;                    // Btn_SaveAction
    public Button btnDeleteAction;                  // Btn_DeleteAction

    private Action currentSelectedAction = null;
    private Dictionary<Action, GameObject> actionListItems = new Dictionary<Action, GameObject>();
    private GameObject currentSelectedButtonObj = null;

    private List<Property> propertyValues = new List<Property>();
    private List<AssetType> subjectTypeValues = new List<AssetType>();
    private List<AssetType> objectTypeValues = new List<AssetType>();
    private List<RepeatMode> repeatModeValues = new List<RepeatMode>();

    private void Start()
    {
        InitializeDropdowns();

        if (btnNewAction != null) btnNewAction.onClick.AddListener(OnCreateNewAction);
        if (btnSaveAction != null) btnSaveAction.onClick.AddListener(OnSaveCurrentAction);
        if (btnDeleteAction != null) btnDeleteAction.onClick.AddListener(OnDeleteCurrentAction);

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
        // 1. Property dropdown (verb: Inspect, Isolate, etc.)
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

        // 2. Subject Type dropdown (who performs the action: Learner, etc.)
        if (dropdownSubjectType != null)
        {
            dropdownSubjectType.ClearOptions();
            subjectTypeValues.Clear();
            List<string> subNames = new List<string>();
            foreach (AssetType val in Enum.GetValues(typeof(AssetType)))
            {
                subjectTypeValues.Add(val);
                subNames.Add(val.ToString());
            }
            dropdownSubjectType.AddOptions(subNames);
        }

        // 3. Object Type dropdown (target of the action: Router, Server, etc.)
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

        // 4. Repeat Mode dropdown (REPEAT, NO_REPEAT, etc.)
        if (dropdownRepeatMode != null)
        {
            dropdownRepeatMode.ClearOptions();
            repeatModeValues.Clear();
            List<string> repNames = new List<string>();
            foreach (RepeatMode val in Enum.GetValues(typeof(RepeatMode)))
            {
                repeatModeValues.Add(val);
                repNames.Add(val.ToString());
            }
            dropdownRepeatMode.AddOptions(repNames);
        }
    }

    public void RefreshList()
    {
        if (listContentContainer == null) return;

        foreach (Transform child in listContentContainer)
        {
            Destroy(child.gameObject);
        }
        actionListItems.Clear();

        var scenario = ScenarioManager.Instance?.currentScenario;
        if (scenario == null || scenario.actions_store == null) return;

        Action firstAction = null;

        foreach (var action in scenario.actions_store)
        {
            if (firstAction == null) firstAction = action;
            CreateListItem(action);
        }

        if (firstAction != null)
        {
            SelectAction(firstAction);
        }
        else
        {
            ClearForm();
        }
    }

    private void CreateListItem(Action action)
    {
        if (actionListItemPrefab == null || listContentContainer == null) return;

        GameObject itemObj = Instantiate(actionListItemPrefab, listContentContainer);
        actionListItems[action] = itemObj;

        UpdateListItemUI(action, itemObj);

        Button btn = itemObj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => SelectAction(action));
        }
    }

    private void UpdateListItemUI(Action action, GameObject itemObj)
    {
        TMP_Text txt = itemObj.GetComponentInChildren<TMP_Text>();
        if (txt != null)
        {
            string displayTitle = string.IsNullOrWhiteSpace(action.title) ? "Unnamed Action" : action.title;
            txt.text = $"<b>{displayTitle}</b>\n<size=11><color=#9CA3AF>{action.id}</color></size>";
        }
    }

    public void SelectAction(Action action)
    {
        currentSelectedAction = action;

        // Reset previously selected list button color
        if (currentSelectedButtonObj != null)
        {
            var prevImg = currentSelectedButtonObj.GetComponent<Image>();
            if (prevImg != null) prevImg.color = new Color(0.16f, 0.16f, 0.19f); // Dark #2A2A30
        }

        // Highlight newly selected list button
        if (actionListItems.TryGetValue(action, out GameObject btnObj))
        {
            currentSelectedButtonObj = btnObj;
            var currImg = currentSelectedButtonObj.GetComponent<Image>();
            if (currImg != null) currImg.color = new Color(0.15f, 0.39f, 0.92f); // Blue #2563EB
        }

        // Populate form fields with selected action data
        if (headerFormText != null) headerFormText.text = $"EDIT BASE ACTION: <color=#60A5FA>{action.id}</color>";
        if (inputID != null) inputID.text = action.id ?? "";
        if (inputTitle != null) inputTitle.text = action.title ?? "";
        if (inputAPCost != null) inputAPCost.text = action.apCost.ToString();
        if (inputBudgetCost != null) inputBudgetCost.text = action.budgetCost.ToString();

        // Property dropdown
        int propIndex = propertyValues.IndexOf(action.property);
        if (dropdownProperty != null) dropdownProperty.value = propIndex >= 0 ? propIndex : 0;

        // SubjectType dropdown
        int subIndex = subjectTypeValues.IndexOf(action.subjectType);
        if (dropdownSubjectType != null) dropdownSubjectType.value = subIndex >= 0 ? subIndex : 0;

        // ObjectType dropdown
        AssetType primaryObjType = (action.objectTypes != null && action.objectTypes.Count > 0) ? action.objectTypes[0] : AssetType.INFO;
        int objIndex = objectTypeValues.IndexOf(primaryObjType);
        if (dropdownObjectType != null) dropdownObjectType.value = objIndex >= 0 ? objIndex : 0;

        // RepeatMode dropdown
        int repIndex = repeatModeValues.IndexOf(action.actionRepeatMode);
        if (dropdownRepeatMode != null) dropdownRepeatMode.value = repIndex >= 0 ? repIndex : 0;
    }

    private void OnSaveCurrentAction()
    {
        if (currentSelectedAction == null) return;

        currentSelectedAction.id = inputID != null ? inputID.text.Trim() : currentSelectedAction.id;
        currentSelectedAction.title = inputTitle != null ? inputTitle.text.Trim() : currentSelectedAction.title;

        // Save costs
        if (inputAPCost != null && int.TryParse(inputAPCost.text, out int ap))
        {
            currentSelectedAction.apCost = ap;
        }
        if (inputBudgetCost != null && int.TryParse(inputBudgetCost.text, out int budget))
        {
            currentSelectedAction.budgetCost = budget;
        }

        // Save Property (verb)
        if (dropdownProperty != null && dropdownProperty.value < propertyValues.Count)
        {
            currentSelectedAction.property = propertyValues[dropdownProperty.value];
        }

        // Save Subject Type
        if (dropdownSubjectType != null && dropdownSubjectType.value < subjectTypeValues.Count)
        {
            currentSelectedAction.subjectType = subjectTypeValues[dropdownSubjectType.value];
        }

        // Save Object Type
        if (dropdownObjectType != null && dropdownObjectType.value < objectTypeValues.Count)
        {
            if (currentSelectedAction.objectTypes == null) currentSelectedAction.objectTypes = new List<AssetType>();
            currentSelectedAction.objectTypes.Clear();
            currentSelectedAction.objectTypes.Add(objectTypeValues[dropdownObjectType.value]);
        }

        // Save Repeat Mode
        if (dropdownRepeatMode != null && dropdownRepeatMode.value < repeatModeValues.Count)
        {
            currentSelectedAction.actionRepeatMode = repeatModeValues[dropdownRepeatMode.value];
        }

        // Refresh list button label
        if (actionListItems.TryGetValue(currentSelectedAction, out GameObject itemObj))
        {
            UpdateListItemUI(currentSelectedAction, itemObj);
        }

        if (headerFormText != null) headerFormText.text = $"EDIT BASE ACTION: <color=#60A5FA>{currentSelectedAction.id}</color>";

        Debug.Log($"<color=green>[ActionStore] Changes saved for action: {currentSelectedAction.id} ({currentSelectedAction.title})</color>");
    }

    private void OnCreateNewAction()
    {
        var scenario = ScenarioManager.Instance?.currentScenario;
        if (scenario == null) return;

        if (scenario.actions_store == null) scenario.actions_store = new List<Action>();

        int newIndex = scenario.actions_store.Count + 1;
        Action newAction = new Action
        {
            id = $"base-action-{newIndex}",
            title = $"New Action {newIndex}",
            ontologyType = CofeletOntologyType.TRIPLE,
            subjectType = AssetType.LEARNER,
            property = Property.INSPECT,
            objectTypes = new List<AssetType> { AssetType.WORKSTATION },
            apCost = 1,
            budgetCost = 0,
            actionRepeatMode = RepeatMode.NO_REPEAT,
            visible = true
        };

        scenario.actions_store.Add(newAction);
        CreateListItem(newAction);
        SelectAction(newAction);

        Debug.Log($"<color=cyan>[ActionStore] New base action created: {newAction.id}</color>");
    }

    private void OnDeleteCurrentAction()
    {
        if (currentSelectedAction == null) return;

        var scenario = ScenarioManager.Instance?.currentScenario;
        if (scenario == null || scenario.actions_store == null) return;

        string deletedId = currentSelectedAction.id;
        scenario.actions_store.Remove(currentSelectedAction);

        if (actionListItems.TryGetValue(currentSelectedAction, out GameObject itemObj))
        {
            Destroy(itemObj);
            actionListItems.Remove(currentSelectedAction);
        }

        currentSelectedAction = null;

        if (scenario.actions_store.Count > 0)
        {
            SelectAction(scenario.actions_store[0]);
        }
        else
        {
            ClearForm();
        }

        Debug.Log($"<color=yellow>[ActionStore] Action deleted: {deletedId}</color>");
    }

    private void ClearForm()
    {
        if (headerFormText != null) headerFormText.text = "EDIT BASE ACTION";
        if (inputID != null) inputID.text = "";
        if (inputTitle != null) inputTitle.text = "";
        if (inputAPCost != null) inputAPCost.text = "0";
        if (inputBudgetCost != null) inputBudgetCost.text = "0";
    }
}
