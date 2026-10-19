using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class ScenarioManager : MonoBehaviour
{
    public static ScenarioManager Instance { get; private set; }

    public event System.Action OnScenarioLoaded;

    // Το σενάριο που είναι φορτωμένο αυτή τη στιγμή στη μνήμη
    [Header("Current Loaded Scenario")]
    public ScenarioData currentScenario;

    // Προαιρετικό: σύρε εδώ το αρχείο SGTTX-20.json από το Unity για γρήγορο τεστ
    [Header("Testing")]
    public TextAsset sampleJsonFile;

    [Header("TopBar UI")]
    public UnityEngine.UI.Button btnSaveJson;

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

        // Μόλις πατήσουμε Play, αν έχουμε βάλει sample file, το φορτώνουμε αυτόματα
        if (sampleJsonFile != null)
        {
            LoadScenarioFromText(sampleJsonFile.text);
        }
    }

    public void OnSaveJsonButtonClicked()
    {
        string json = ExportScenarioToJson();
        if (string.IsNullOrEmpty(json)) return;

        string exportPath = Path.Combine(Application.dataPath, "SampleScenarios", "Exported_Scenario.json");
        try
        {
            File.WriteAllText(exportPath, json);
            Debug.Log($"<color=cyan><b>[ScenarioManager] Το σενάριο έγινε Export επιτυχώς!</b>\nΑποθηκεύτηκε στο: Assets/SampleScenarios/Exported_Scenario.json\nΜέγεθος: {json.Length} χαρακτήρες</color>");
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
            #endif
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ScenarioManager] Σφάλμα κατά την αποθήκευση του αρχείου JSON: {ex.Message}");
        }
    }

    /// <summary>
    /// Φορτώνει ένα σενάριο από κείμενο JSON (string)
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

            Debug.Log($"<color=green><b>[ScenarioManager] Το σενάριο φορτώθηκε επιτυχώς!</b></color>\n" +
                      $"Τίτλος: {currentScenario.title}\n" +
                      $"Έκδοση: {currentScenario.version}\n" +
                      $"Γύροι (Rounds): {currentScenario.step_rounds?.Count ?? 0}\n" +
                      $"Assets στο Store: {currentScenario.assets_store?.Count ?? 0}\n" +
                      $"Actions στο Store: {currentScenario.actions_store?.Count ?? 0}\n" +
                      $"Στόχοι (Goals): {currentScenario.goals?.Count ?? 0}\n" +
                      $"Στοιχεία WatchBoard: {currentScenario.watch_board?.Count ?? 0}");

            OnScenarioLoaded?.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ScenarioManager] Σφάλμα κατά τη φόρτωση του σεναρίου: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Εξάγει το τρέχον σενάριο σε JSON μορφή
    /// </summary>
    public string ExportScenarioToJson()
    {
        if (currentScenario == null)
        {
            Debug.LogWarning("[ScenarioManager] Δεν υπάρχει φορτωμένο σενάριο για export.");
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
/// Converter που διαχειρίζεται Enums ακόμα κι αν στο JSON υπάρχει κενό string "" ή απροσδόκητη τιμή
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