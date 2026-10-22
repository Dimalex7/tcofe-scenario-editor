using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class ScenarioManager : MonoBehaviour
{
    public static ScenarioManager Instance { get; private set; }

    public event System.Action OnScenarioLoaded;

    // The scenario currently loaded in memory
    [Header("Current Loaded Scenario")]
    public ScenarioData currentScenario;

    // Optional: drag the SGTTX-20.json file here from the Unity Project for a quick in-editor test
    [Header("Testing")]
    public TextAsset sampleJsonFile;

    [Header("TopBar UI")]
    public UnityEngine.UI.Button btnSaveJson;

#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void DownloadFileFromBrowser(string filename, string content);
#endif

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (btnSaveJson != null)
        {
            btnSaveJson.onClick.AddListener(OnSaveJsonButtonClicked);
        }

        // On Play, automatically load the sample file if one is assigned
        if (sampleJsonFile != null)
        {
            LoadScenarioFromText(sampleJsonFile.text);
        }
    }

    public void OnSaveJsonButtonClicked()
    {
        string json = ExportScenarioToJson();
        if (string.IsNullOrEmpty(json)) return;

#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            DownloadFileFromBrowser("Exported_Scenario.json", json);
            Debug.Log("[ScenarioManager] Started browser download for Exported_Scenario.json");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ScenarioManager] Failed to trigger browser file download: {ex.Message}");
        }
#else
        string exportPath = Path.Combine(Application.dataPath, "SampleScenarios", "Exported_Scenario.json");
        try
        {
            File.WriteAllText(exportPath, json);
            Debug.Log($"<color=cyan><b>[ScenarioManager] Scenario exported successfully!</b>\nSaved to: Assets/SampleScenarios/Exported_Scenario.json\nSize: {json.Length} characters</color>");
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
            #endif
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ScenarioManager] Failed to save JSON file: {ex.Message}");
        }
#endif
    }

    /// <summary>
    /// Loads a scenario from a JSON string into memory.
    /// </summary>
    public bool LoadScenarioFromText(string jsonContent)
    {
        try
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            settings.Converters.Add(new TolerantEnumConverter());

            currentScenario = JsonConvert.DeserializeObject<ScenarioData>(jsonContent, settings);

            Debug.Log($"<color=green><b>[ScenarioManager] Scenario loaded successfully!</b></color>\n" +
                      $"Title: {currentScenario.title}\n" +
                      $"Version: {currentScenario.version}\n" +
                      $"Rounds: {currentScenario.step_rounds?.Count ?? 0}\n" +
                      $"Assets in Store: {currentScenario.assets_store?.Count ?? 0}\n" +
                      $"Actions in Store: {currentScenario.actions_store?.Count ?? 0}\n" +
                      $"Goals: {currentScenario.goals?.Count ?? 0}\n" +
                      $"Watch Board items: {currentScenario.watch_board?.Count ?? 0}");

            OnScenarioLoaded?.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ScenarioManager] Failed to load scenario: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Serializes the currently loaded scenario to an indented JSON string.
    /// </summary>
    public string ExportScenarioToJson()
    {
        if (currentScenario == null)
        {
            Debug.LogWarning("[ScenarioManager] No scenario is currently loaded — nothing to export.");
            return string.Empty;
        }

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };
        settings.Converters.Add(new TolerantEnumConverter());

        return JsonConvert.SerializeObject(currentScenario, settings);
    }
}

/// <summary>
/// A tolerant JSON converter for enums that gracefully handles empty strings (""),
/// unknown values, or integer representations without throwing exceptions.
/// </summary>
public class TolerantEnumConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsEnum || (Nullable.GetUnderlyingType(objectType)?.IsEnum ?? false);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        Type enumType = Nullable.GetUnderlyingType(objectType) ?? objectType;

        if (reader.TokenType == JsonToken.String)
        {
            string enumText = reader.Value?.ToString();
            if (string.IsNullOrWhiteSpace(enumText))
            {
                if (Enum.IsDefined(enumType, -1))
                    return Enum.ToObject(enumType, -1);
                return Activator.CreateInstance(enumType);
            }
            if (int.TryParse(enumText, out int intVal))
            {
                return Enum.ToObject(enumType, intVal);
            }
            try
            {
                return Enum.Parse(enumType, enumText, true);
            }
            catch
            {
                if (Enum.IsDefined(enumType, -1))
                    return Enum.ToObject(enumType, -1);
                return Activator.CreateInstance(enumType);
            }
        }
        else if (reader.TokenType == JsonToken.Integer)
        {
            return Enum.ToObject(enumType, Convert.ToInt32(reader.Value));
        }

        return Activator.CreateInstance(enumType);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }
        writer.WriteValue(Convert.ToInt32(value));
    }
}
