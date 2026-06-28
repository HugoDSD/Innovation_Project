using InnovationProject.Dtos;

namespace InnovationProject.Services;

public class ImpactCalculator
{
    private const double FranceElectricityMixKgPerKwh = 0.0801;

    private readonly Dictionary<string, double> _energyPerToken = new()
    {
        { "GPT OSS 20B", 7.888889e-09 },
        { "GPT OSS 120B", 1.102778e-08 },
        { "DeepSeek V3.1", 3.708611e-07 },
        { "DeepSeek R1", 6.600556e-07 }
    };

    private readonly Dictionary<string, double> _wueProvider = new()
    {
        { "Microsoft", 0.03 },
        { "Amazon", 0.12 },
        { "Référence", 0.84 }
    };

    private readonly Dictionary<string, (double Input, double Output)> _costPerToken = new()
    {
        { "GPT OSS 20B", (1.5e-07, 6.0e-07) },
        { "GPT OSS 120B", (3.0e-06, 1.5e-05) },
        { "DeepSeek V3.1", (1.4e-07, 2.8e-07) },
        { "DeepSeek R1", (5.5e-07, 2.19e-06) }                                      
    };

    // --- THE CALCULATION ENGINE ---
    public EvaluationResultDto EvaluateProject(EvaluationRequestDto request)
    {
        var result = new EvaluationResultDto();


        // Validate the inputs
        if (request.DataSensitivity == 5 && request.LegalRisk == 5)
        {
            result.IsApproved = false;
            result.Message = "REFUSÉ : Risque critique. L'utilisation de l'IA est interdite pour ce projet.";
            return result;
        }

        // Compute the energy, carbon and water impact
        if (!_energyPerToken.ContainsKey(request.ModelName) || !_wueProvider.ContainsKey(request.Provider))
        {
            result.IsApproved = false;
            result.Message = "ERREUR : Modèle IA ou Fournisseur inconnu.";
            return result;
        }

        double energyPerToken = _energyPerToken[request.ModelName];
        double totalTokens = request.InputTokens + request.OutputTokens;
        
        result.TotalEnergyKwh = totalTokens * energyPerToken;
        result.TotalCarbonKg = result.TotalEnergyKwh * FranceElectricityMixKgPerKwh;
        result.TotalWaterLiters = result.TotalEnergyKwh * _wueProvider[request.Provider];

        // Compute the economic impact
        if (_costPerToken.TryGetValue(request.ModelName, out var costs))
        {
            result.TotalCostUsd = (request.InputTokens * costs.Input) + (request.OutputTokens * costs.Output);
        }

        // Compute the social impact
        result.TotalHoursSaved = request.HoursSavedReports + request.HoursSavedImages + request.HoursSavedPresentations;


        result.RiskScore = (request.DataSensitivity + request.LegalRisk) / 2.0;

        // Approval logic

        if (result.TotalHoursSaved > 1.0 && result.RiskScore < 4.0)
        {
            result.IsApproved = true;
            result.Message = "APPROUVÉ : Le gain social compense l'impact environnemental et les risques sont maîtrisés.";
        }
        else
        {
            result.IsApproved = false;
            result.Message = "REJETÉ : Les bénéfices (gain de temps) ne compensent pas les risques ou l'impact.";
        }

        return result;
    }
}