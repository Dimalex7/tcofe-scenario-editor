using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ValidationModalController : MonoBehaviour
{
    [Header("Modal Root")]
    public GameObject modalRoot;            // Το Panel του Popup (ValidationModal)
    public Button btnValidateTopBar;        // Το κουμπί Btn_Validate στην TopBar
    public Button btnCloseModal;            // Το κουμπί Close μέσα στο Popup

    [Header("UI Feedback")]
    public TMP_Text statusBannerText;       // Τίτλος: "SCENARIO IS VALID!" ή "FOUND X ISSUES"
    public Image statusBannerImage;         // Χρώμα μπάρας τίτλου (Πράσινο / Κόκκινο)
    public TMP_Text detailsText;            // Αναλυτικό κείμενο σφαλμάτων και προειδοποιήσεων

    [Header("Colors")]
    public Color validColor = new Color(0.09f, 0.64f, 0.29f);   // Green (#16A34A)
    public Color invalidColor = new Color(0.86f, 0.15f, 0.15f); // Red (#DC2626)

    private void Start()
    {
        if (modalRoot != null) modalRoot.SetActive(false);

        if (btnValidateTopBar != null)
        {
            btnValidateTopBar.onClick.AddListener(RunValidationAndShowModal);
        }

        if (btnCloseModal != null)
        {
            btnCloseModal.onClick.AddListener(CloseModal);
        }
    }

    public void CloseModal()
    {
        if (modalRoot != null) modalRoot.SetActive(false);
    }

    public void RunValidationAndShowModal()
    {
        var scenario = ScenarioManager.Instance?.currentScenario;
        var result = ScenarioValidator.Validate(scenario);

        if (modalRoot != null) modalRoot.SetActive(true);

        if (result.IsValid)
        {
            if (statusBannerText != null) statusBannerText.text = "SCENARIO IS VALID!";
            if (statusBannerImage != null) statusBannerImage.color = validColor;

            string details = "<color=#22C55E><b>Όλοι οι έλεγχοι εγκυρότητας πέρασαν επιτυχώς!</b></color>\nΤο σενάριο έχει σωστή δομή, μοναδικά IDs και είναι έτοιμο για εξαγωγή.";
            if (result.Warnings.Count > 0)
            {
                details += "\n\n<color=#FBBF24><b>Προειδοποιήσεις (Warnings):</b></color>\n";
                foreach (var w in result.Warnings)
                {
                    details += $"• {w}\n";
                }
            }
            if (detailsText != null) detailsText.text = details;
        }
        else
        {
            if (statusBannerText != null) statusBannerText.text = $"FOUND {result.Errors.Count} ISSUES!";
            if (statusBannerImage != null) statusBannerImage.color = invalidColor;

            string details = $"<color=#EF4444><b>Εντοπίστηκαν {result.Errors.Count} κρίσιμα σφάλματα:</b></color>\n";
            foreach (var err in result.Errors)
            {
                details += $"• <color=#FCA5A5>{err}</color>\n";
            }

            if (result.Warnings.Count > 0)
            {
                details += "\n<color=#FBBF24><b>Προειδοποιήσεις (Warnings):</b></color>\n";
                foreach (var w in result.Warnings)
                {
                    details += $"• {w}\n";
                }
            }
            if (detailsText != null) detailsText.text = details;
        }
    }
}
