using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ValidationModalController : MonoBehaviour
{
    [Header("Modal Root")]
    public GameObject modalRoot;            // The validation popup panel (ValidationModal)
    public Button btnValidateTopBar;        // The Btn_Validate button in the TopBar
    public Button btnCloseModal;            // The Close button inside the popup

    [Header("UI Feedback")]
    public TMP_Text statusBannerText;       // Title banner: "SCENARIO IS VALID!" or "FOUND X ISSUES!"
    public Image statusBannerImage;         // Banner background color (green / red)
    public TMP_Text detailsText;            // Detailed errors and warnings text

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

            string details = "<color=#22C55E><b>All validation checks passed successfully!</b></color>\nThe scenario has a valid structure, unique IDs, and is ready for export.";
            if (result.Warnings.Count > 0)
            {
                details += "\n\n<color=#FBBF24><b>Warnings:</b></color>\n";
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

            string details = $"<color=#EF4444><b>{result.Errors.Count} critical error(s) found:</b></color>\n";
            foreach (var err in result.Errors)
            {
                details += $"• <color=#FCA5A5>{err}</color>\n";
            }

            if (result.Warnings.Count > 0)
            {
                details += "\n<color=#FBBF24><b>Warnings:</b></color>\n";
                foreach (var w in result.Warnings)
                {
                    details += $"• {w}\n";
                }
            }
            if (detailsText != null) detailsText.text = details;
        }
    }
}
