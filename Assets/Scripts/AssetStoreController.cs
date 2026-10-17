using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AssetStoreController : MonoBehaviour
{
    [Header("List Column")]
    public Transform listContentContainer;      // AssetScrollView -> Viewport -> Content
    public GameObject assetListItemPrefab;       // Το Prefab από το Assets/Prefabs/
    public Button btnNewAsset;                  // Btn_NewAsset

    [Header("Form Fields")]
    public TMP_Text headerFormText;             // Header_Form
    public TMP_InputField inputID;              // Input_ID
    public TMP_InputField inputTitle;           // Input_Title
    public TMP_InputField inputCaption;         // Input_Caption
    public TMP_InputField inputDescription;     // Input_Description
    public TMP_Dropdown dropdownType;           // Dropdown_Type
    public TMP_Dropdown dropdownOpState;        // Dropdown_OpState
    public TMP_Dropdown dropdownSecState;       // Dropdown_SecState

    [Header("Form Buttons")]
    public Button btnSaveAsset;                 // Btn_SaveAsset
    public Button btnDeleteAsset;               // Btn_DeleteAsset

    private Asset currentSelectedAsset = null;
    private Dictionary<Asset, GameObject> assetListItems = new Dictionary<Asset, GameObject>();
    private GameObject currentSelectedButtonObj = null;

    private List<AssetType> assetTypeValues = new List<AssetType>();
    private List<OperationalState> opStateValues = new List<OperationalState>();
    private List<SecurityState> secStateValues = new List<SecurityState>();

    private void Start()
    {
        InitializeDropdowns();

        if (btnNewAsset != null) btnNewAsset.onClick.AddListener(OnCreateNewAsset);
        if (btnSaveAsset != null) btnSaveAsset.onClick.AddListener(OnSaveCurrentAsset);
        if (btnDeleteAsset != null) btnDeleteAsset.onClick.AddListener(OnDeleteCurrentAsset);

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
        if (dropdownType != null)
        {
            dropdownType.ClearOptions();
            assetTypeValues.Clear();
            List<string> typeNames = new List<string>();
            foreach (AssetType val in Enum.GetValues(typeof(AssetType)))
            {
                assetTypeValues.Add(val);
                typeNames.Add(val.ToString());
            }
            dropdownType.AddOptions(typeNames);
        }

        if (dropdownOpState != null)
        {
            dropdownOpState.ClearOptions();
            opStateValues.Clear();
            List<string> opNames = new List<string>();
            foreach (OperationalState val in Enum.GetValues(typeof(OperationalState)))
            {
                opStateValues.Add(val);
                opNames.Add(val.ToString());
            }
            dropdownOpState.AddOptions(opNames);
        }

        if (dropdownSecState != null)
        {
            dropdownSecState.ClearOptions();
            secStateValues.Clear();
            List<string> secNames = new List<string>();
            foreach (SecurityState val in Enum.GetValues(typeof(SecurityState)))
            {
                secStateValues.Add(val);
                secNames.Add(val.ToString());
            }
            dropdownSecState.AddOptions(secNames);
        }
    }

    public void RefreshList()
    {
        if (listContentContainer == null) return;

        // Καθαρισμός προηγούμενων στοιχείων
        foreach (Transform child in listContentContainer)
        {
            Destroy(child.gameObject);
        }
        assetListItems.Clear();

        var scenario = ScenarioManager.Instance?.currentScenario;
        if (scenario == null || scenario.assets_store == null) return;

        Asset firstAsset = null;

        foreach (var asset in scenario.assets_store)
        {
            if (firstAsset == null) firstAsset = asset;
            CreateListItem(asset);
        }

        if (firstAsset != null)
        {
            SelectAsset(firstAsset);
        }
        else
        {
            ClearForm();
        }
    }

    private void CreateListItem(Asset asset)
    {
        if (assetListItemPrefab == null || listContentContainer == null) return;

        GameObject itemObj = Instantiate(assetListItemPrefab, listContentContainer);
        assetListItems[asset] = itemObj;

        UpdateListItemUI(asset, itemObj);

        Button btn = itemObj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => SelectAsset(asset));
        }
    }

    private void UpdateListItemUI(Asset asset, GameObject itemObj)
    {
        TMP_Text txt = itemObj.GetComponentInChildren<TMP_Text>();
        if (txt != null)
        {
            string displayTitle = string.IsNullOrWhiteSpace(asset.title) ? "Unnamed Asset" : asset.title;
            txt.text = $"<b>{displayTitle}</b>\n<size=11><color=#9CA3AF>{asset.id}</color></size>";
        }
    }

    public void SelectAsset(Asset asset)
    {
        currentSelectedAsset = asset;

        // Χρωματισμός επιλεγμένου κουμπιού στη λίστα
        if (currentSelectedButtonObj != null)
        {
            var prevImg = currentSelectedButtonObj.GetComponent<Image>();
            if (prevImg != null) prevImg.color = new Color(0.16f, 0.16f, 0.19f); // Σκούρο #2A2A30
        }

        if (assetListItems.TryGetValue(asset, out GameObject btnObj))
        {
            currentSelectedButtonObj = btnObj;
            var currImg = currentSelectedButtonObj.GetComponent<Image>();
            if (currImg != null) currImg.color = new Color(0.15f, 0.39f, 0.92f); // Μπλε #2563EB
        }

        // Γέμισμα των πεδίων της φόρμας
        if (headerFormText != null) headerFormText.text = $"EDIT BASE ASSET: <color=#60A5FA>{asset.id}</color>";
        if (inputID != null) inputID.text = asset.id ?? "";
        if (inputTitle != null) inputTitle.text = asset.title ?? "";
        if (inputCaption != null) inputCaption.text = asset.caption ?? "";
        if (inputDescription != null) inputDescription.text = asset.description ?? "";

        // Επιλογή στο Dropdown Type
        AssetType primaryType = (asset.types != null && asset.types.Count > 0) ? asset.types[0] : AssetType.INFO;
        int typeIndex = assetTypeValues.IndexOf(primaryType);
        if (dropdownType != null) dropdownType.value = typeIndex >= 0 ? typeIndex : 0;

        // Επιλογή στο Dropdown OpState
        int opIndex = opStateValues.IndexOf(asset.operationalState);
        if (dropdownOpState != null) dropdownOpState.value = opIndex >= 0 ? opIndex : 0;

        // Επιλογή στο Dropdown SecState
        int secIndex = secStateValues.IndexOf(asset.securityState);
        if (dropdownSecState != null) dropdownSecState.value = secIndex >= 0 ? secIndex : 0;
    }

    private void OnSaveCurrentAsset()
    {
        if (currentSelectedAsset == null) return;

        currentSelectedAsset.id = inputID != null ? inputID.text.Trim() : currentSelectedAsset.id;
        currentSelectedAsset.title = inputTitle != null ? inputTitle.text.Trim() : currentSelectedAsset.title;
        currentSelectedAsset.caption = inputCaption != null ? inputCaption.text.Trim() : currentSelectedAsset.caption;
        currentSelectedAsset.description = inputDescription != null ? inputDescription.text.Trim() : currentSelectedAsset.description;

        // Αποθήκευση Τύπου
        if (dropdownType != null && dropdownType.value < assetTypeValues.Count)
        {
            if (currentSelectedAsset.types == null) currentSelectedAsset.types = new List<AssetType>();
            currentSelectedAsset.types.Clear();
            currentSelectedAsset.types.Add(assetTypeValues[dropdownType.value]);
        }

        // Αποθήκευση Operational State
        if (dropdownOpState != null && dropdownOpState.value < opStateValues.Count)
        {
            currentSelectedAsset.operationalState = opStateValues[dropdownOpState.value];
        }

        // Αποθήκευση Security State
        if (dropdownSecState != null && dropdownSecState.value < secStateValues.Count)
        {
            currentSelectedAsset.securityState = secStateValues[dropdownSecState.value];
        }

        // Ενημέρωση του κουμπιού στη λίστα
        if (assetListItems.TryGetValue(currentSelectedAsset, out GameObject itemObj))
        {
            UpdateListItemUI(currentSelectedAsset, itemObj);
        }

        if (headerFormText != null) headerFormText.text = $"EDIT BASE ASSET: <color=#60A5FA>{currentSelectedAsset.id}</color>";

        Debug.Log($"<color=green>[AssetStore] Αποθηκεύτηκαν οι αλλαγές για το asset: {currentSelectedAsset.id} ({currentSelectedAsset.title})</color>");
    }

    private void OnCreateNewAsset()
    {
        var scenario = ScenarioManager.Instance?.currentScenario;
        if (scenario == null) return;

        if (scenario.assets_store == null) scenario.assets_store = new List<Asset>();

        int newIndex = scenario.assets_store.Count + 1;
        Asset newAsset = new Asset
        {
            id = $"base-asset-{newIndex}",
            title = $"New Asset {newIndex}",
            caption = "Asset caption",
            description = "Asset description...",
            types = new List<AssetType> { AssetType.WORKSTATION },
            operationalState = OperationalState.ACTIVE,
            securityState = SecurityState.SECURED
        };

        scenario.assets_store.Add(newAsset);
        CreateListItem(newAsset);
        SelectAsset(newAsset);

        Debug.Log($"<color=cyan>[AssetStore] Δημιουργήθηκε νέο base asset: {newAsset.id}</color>");
    }

    private void OnDeleteCurrentAsset()
    {
        if (currentSelectedAsset == null) return;

        var scenario = ScenarioManager.Instance?.currentScenario;
        if (scenario == null || scenario.assets_store == null) return;

        string deletedId = currentSelectedAsset.id;
        scenario.assets_store.Remove(currentSelectedAsset);

        if (assetListItems.TryGetValue(currentSelectedAsset, out GameObject itemObj))
        {
            Destroy(itemObj);
            assetListItems.Remove(currentSelectedAsset);
        }

        currentSelectedAsset = null;

        if (scenario.assets_store.Count > 0)
        {
            SelectAsset(scenario.assets_store[0]);
        }
        else
        {
            ClearForm();
        }

        Debug.Log($"<color=yellow>[AssetStore] Διαγράφηκε το asset: {deletedId}</color>");
    }

    private void ClearForm()
    {
        if (headerFormText != null) headerFormText.text = "EDIT BASE ASSET";
        if (inputID != null) inputID.text = "";
        if (inputTitle != null) inputTitle.text = "";
        if (inputCaption != null) inputCaption.text = "";
        if (inputDescription != null) inputDescription.text = "";
    }
}