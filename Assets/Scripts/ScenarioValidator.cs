using System;
using System.Collections.Generic;

public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors = new List<string>();
    public List<string> Warnings = new List<string>();
}

public static class ScenarioValidator
{
    public static ValidationResult Validate(ScenarioData scenario)
    {
        var result = new ValidationResult();

        if (scenario == null)
        {
            result.Errors.Add("Δεν υπάρχει φορτωμένο σενάριο στη μνήμη!");
            return result;
        }

        // 1. Έλεγχος Metadata Σεναρίου
        if (string.IsNullOrWhiteSpace(scenario.title))
        {
            result.Warnings.Add("[Metadata] Ο τίτλος του σεναρίου είναι κενός.");
        }
        if (string.IsNullOrWhiteSpace(scenario.version))
        {
            result.Warnings.Add("[Metadata] Η έκδοση του σεναρίου είναι κενή.");
        }

        // 2. Έλεγχος Asset Store
        if (scenario.assets_store == null || scenario.assets_store.Count == 0)
        {
            result.Errors.Add("[Assets] Το Asset Store είναι άδειο (δεν υπάρχουν πρότυπα assets).");
        }
        else
        {
            HashSet<string> seenAssetIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < scenario.assets_store.Count; i++)
            {
                var asset = scenario.assets_store[i];
                if (asset == null)
                {
                    result.Errors.Add($"[Assets] Το asset στη θέση #{i + 1} είναι null.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(asset.id))
                {
                    result.Errors.Add($"[Assets] Το asset '{asset.title}' στη θέση #{i + 1} έχει κενό ID!");
                }
                else if (seenAssetIds.Contains(asset.id))
                {
                    result.Errors.Add($"[Assets] Διπλότυπο ID: Το ID '{asset.id}' χρησιμοποιείται πάνω από μία φορά!");
                }
                else
                {
                    seenAssetIds.Add(asset.id);
                }

                if (string.IsNullOrWhiteSpace(asset.title))
                {
                    result.Warnings.Add($"[Assets] Το asset '{asset.id}' δεν έχει τίτλο.");
                }
            }
        }

        // 3. Έλεγχος Action Store
        if (scenario.actions_store == null || scenario.actions_store.Count == 0)
        {
            result.Errors.Add("[Actions] Το Action Store είναι άδειο (δεν υπάρχουν ενέργειες).");
        }
        else
        {
            HashSet<string> seenActionIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < scenario.actions_store.Count; i++)
            {
                var action = scenario.actions_store[i];
                if (action == null)
                {
                    result.Errors.Add($"[Actions] Το action στη θέση #{i + 1} είναι null.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(action.id))
                {
                    result.Errors.Add($"[Actions] Το action '{action.title}' στη θέση #{i + 1} έχει κενό ID!");
                }
                else if (seenActionIds.Contains(action.id))
                {
                    result.Errors.Add($"[Actions] Διπλότυπο ID: Το ID '{action.id}' χρησιμοποιείται πάνω από μία φορά!");
                }
                else
                {
                    seenActionIds.Add(action.id);
                }

                if (string.IsNullOrWhiteSpace(action.title))
                {
                    result.Warnings.Add($"[Actions] Το action '{action.id}' δεν έχει τίτλο.");
                }

                if (action.property == Property.UNDEFINED)
                {
                    result.Warnings.Add($"[Actions] Το action '{action.id}' έχει απροσδιόριστο Property (UNDEFINED).");
                }
            }
        }

        // 4. Έλεγχος Step Rounds
        if (scenario.step_rounds == null || scenario.step_rounds.Count == 0)
        {
            result.Errors.Add("[Rounds] Το σενάριο δεν περιέχει κανέναν Γύρο (Step Round)!");
        }
        else
        {
            HashSet<string> seenRoundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < scenario.step_rounds.Count; i++)
            {
                var round = scenario.step_rounds[i];
                if (round == null)
                {
                    result.Errors.Add($"[Rounds] Ο γύρος στη θέση #{i + 1} είναι null.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(round.id))
                {
                    result.Errors.Add($"[Rounds] Ο γύρος #{i + 1} έχει κενό ID!");
                }
                else if (seenRoundIds.Contains(round.id))
                {
                    result.Errors.Add($"[Rounds] Διπλότυπο ID: Ο γύρος '{round.id}' υπάρχει παραπάνω από μία φορά!");
                }
                else
                {
                    seenRoundIds.Add(round.id);
                }

                if (round.stepMetrics == null)
                {
                    result.Warnings.Add($"[Rounds] Ο γύρος '{round.id}' δεν έχει αρχικοποιημένα Metrics.");
                }
            }
        }

        // 5. Έλεγχος Goals
        if (scenario.goals == null || scenario.goals.Count == 0)
        {
            result.Warnings.Add("[Goals] Το σενάριο δεν περιέχει κανέναν Στόχο (Goal).");
        }
        else
        {
            HashSet<string> seenGoalIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < scenario.goals.Count; i++)
            {
                var goal = scenario.goals[i];
                if (goal == null)
                {
                    result.Errors.Add($"[Goals] Ο στόχος στη θέση #{i + 1} είναι null.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(goal.id))
                {
                    result.Errors.Add($"[Goals] Ο στόχος στη θέση #{i + 1} έχει κενό ID!");
                }
                else if (seenGoalIds.Contains(goal.id))
                {
                    result.Errors.Add($"[Goals] Διπλότυπο ID: Ο στόχος '{goal.id}' υπάρχει παραπάνω από μία φορά!");
                }
                else
                {
                    seenGoalIds.Add(goal.id);
                }

                if (goal.type == GoalType.UNDEFINED)
                {
                    result.Warnings.Add($"[Goals] Ο στόχος '{goal.id}' έχει απροσδιόριστο Goal Type.");
                }

                // Έλεγχος αν ο γύρος στον οποίο αναφέρεται ο στόχος υπάρχει
                if (goal.round >= 0 && scenario.step_rounds != null && goal.round >= scenario.step_rounds.Count)
                {
                    result.Errors.Add($"[Goals] Ο στόχος '{goal.id}' αναφέρεται στον Γύρο #{goal.round + 1}, αλλά το σενάριο έχει μόνο {scenario.step_rounds.Count} γύρους!");
                }
            }
        }

        return result;
    }
}
