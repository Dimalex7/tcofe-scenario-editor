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
            result.Errors.Add("No scenario is currently loaded in memory.");
            return result;
        }

        // 1. Scenario metadata checks
        if (string.IsNullOrWhiteSpace(scenario.title))
        {
            result.Warnings.Add("[Metadata] Scenario title is empty.");
        }
        if (string.IsNullOrWhiteSpace(scenario.version))
        {
            result.Warnings.Add("[Metadata] Scenario version is empty.");
        }

        // 2. Asset Store checks
        if (scenario.assets_store == null || scenario.assets_store.Count == 0)
        {
            result.Errors.Add("[Assets] The Asset Store is empty — no base asset templates found.");
        }
        else
        {
            HashSet<string> seenAssetIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < scenario.assets_store.Count; i++)
            {
                var asset = scenario.assets_store[i];
                if (asset == null)
                {
                    result.Errors.Add($"[Assets] Asset at position #{i + 1} is null.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(asset.id))
                {
                    result.Errors.Add($"[Assets] Asset '{asset.title}' at position #{i + 1} has an empty ID.");
                }
                else if (seenAssetIds.Contains(asset.id))
                {
                    result.Errors.Add($"[Assets] Duplicate ID: '{asset.id}' is used more than once.");
                }
                else
                {
                    seenAssetIds.Add(asset.id);
                }

                if (string.IsNullOrWhiteSpace(asset.title))
                {
                    result.Warnings.Add($"[Assets] Asset '{asset.id}' has no title.");
                }
            }
        }

        // 3. Action Store checks
        if (scenario.actions_store == null || scenario.actions_store.Count == 0)
        {
            result.Errors.Add("[Actions] The Action Store is empty — no base action templates found.");
        }
        else
        {
            HashSet<string> seenActionIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < scenario.actions_store.Count; i++)
            {
                var action = scenario.actions_store[i];
                if (action == null)
                {
                    result.Errors.Add($"[Actions] Action at position #{i + 1} is null.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(action.id))
                {
                    result.Errors.Add($"[Actions] Action '{action.title}' at position #{i + 1} has an empty ID.");
                }
                else if (seenActionIds.Contains(action.id))
                {
                    result.Errors.Add($"[Actions] Duplicate ID: '{action.id}' is used more than once.");
                }
                else
                {
                    seenActionIds.Add(action.id);
                }

                if (string.IsNullOrWhiteSpace(action.title))
                {
                    result.Warnings.Add($"[Actions] Action '{action.id}' has no title.");
                }

                if (action.property == Property.UNDEFINED)
                {
                    result.Warnings.Add($"[Actions] Action '{action.id}' has an undefined Property (UNDEFINED).");
                }
            }
        }

        // 4. Step Rounds checks
        if (scenario.step_rounds == null || scenario.step_rounds.Count == 0)
        {
            result.Errors.Add("[Rounds] The scenario contains no Step Rounds.");
        }
        else
        {
            HashSet<string> seenRoundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < scenario.step_rounds.Count; i++)
            {
                var round = scenario.step_rounds[i];
                if (round == null)
                {
                    result.Errors.Add($"[Rounds] Round at position #{i + 1} is null.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(round.id))
                {
                    result.Errors.Add($"[Rounds] Round #{i + 1} has an empty ID.");
                }
                else if (seenRoundIds.Contains(round.id))
                {
                    result.Errors.Add($"[Rounds] Duplicate ID: Round '{round.id}' appears more than once.");
                }
                else
                {
                    seenRoundIds.Add(round.id);
                }

                if (round.stepMetrics == null)
                {
                    result.Warnings.Add($"[Rounds] Round '{round.id}' has no Metrics initialized.");
                }
            }
        }

        // 5. Goals checks
        if (scenario.goals == null || scenario.goals.Count == 0)
        {
            result.Warnings.Add("[Goals] The scenario contains no Goals.");
        }
        else
        {
            HashSet<string> seenGoalIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < scenario.goals.Count; i++)
            {
                var goal = scenario.goals[i];
                if (goal == null)
                {
                    result.Errors.Add($"[Goals] Goal at position #{i + 1} is null.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(goal.id))
                {
                    result.Errors.Add($"[Goals] Goal at position #{i + 1} has an empty ID.");
                }
                else if (seenGoalIds.Contains(goal.id))
                {
                    result.Errors.Add($"[Goals] Duplicate ID: Goal '{goal.id}' appears more than once.");
                }
                else
                {
                    seenGoalIds.Add(goal.id);
                }

                if (goal.type == GoalType.UNDEFINED)
                {
                    result.Warnings.Add($"[Goals] Goal '{goal.id}' has an undefined Goal Type.");
                }

                // Validate that the referenced round index exists
                if (goal.round >= 0 && scenario.step_rounds != null && goal.round >= scenario.step_rounds.Count)
                {
                    result.Errors.Add($"[Goals] Goal '{goal.id}' references Round #{goal.round + 1}, but the scenario only has {scenario.step_rounds.Count} round(s).");
                }
            }
        }

        return result;
    }
}
