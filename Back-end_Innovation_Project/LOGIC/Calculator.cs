using Back_end_Innovation_Project.APP.DTOs;

namespace Back_end_Innovation_Project.LOGIC.Calculators;

public class ImpactCalculator
{
    private const double MixElectriqueFrance = 0.0801;

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
        { "DeepSeek V3.1", (1.4e-07, 2.8e-07) }
                                                // A voir: DeepSeek R1 n'avait pas de prix dans le CSV fourni
    };

    // --- LE MOTEUR DE CALCUL ---
    public EvaluationResultDto EvaluateProject(EvaluationRequestDto request)
    {
        var result = new EvaluationResultDto();

        // Correspond au "Kill Switch" dans le cas ou le critère Juridique et du Sensibilité est atteint
        if (request.DataSensitivity == 5 && request.LegalRisk == 5)
        {
            result.IsApproved = false;
            result.Message = "REFUSÉ : Risque critique. L'utilisation de l'IA est interdite pour ce projet.";
            return result;
        }

        // Vérification de sécurité (si le modèle envoyé par le front n'existe pas)
        if (!_energyPerToken.ContainsKey(request.ModelName) || !_wueProvider.ContainsKey(request.Provider))
        {
            result.IsApproved = false;
            result.Message = "ERREUR : Modèle IA ou Fournisseur inconnu.";
            return result;
        }

        // ÉTAPE B : L'impact Environnemental
        double energyPerToken = _energyPerToken[request.ModelName];
        double totalTokens = request.InputTokens + request.OutputTokens;
        
        result.TotalEnergyKwh = totalTokens * energyPerToken;
        result.TotalCarbonKg = result.TotalEnergyKwh * MixElectriqueFrance;
        result.TotalWaterLiters = result.TotalEnergyKwh * _wueProvider[request.Provider];

        // ÉTAPE C : L'impact Économique
        if (_costPerToken.TryGetValue(request.ModelName, out var costs))
        {
            result.TotalCostUsd = (request.InputTokens * costs.Input) + (request.OutputTokens * costs.Output);
        }

        // ÉTAPE D : L'impact Social
        result.TotalHoursSaved = request.HoursSavedReports + request.HoursSavedImages + request.HoursSavedPresentations;
        
        // Formule basique pour le risque (moyenne sur 5)
        result.RiskScore = (request.DataSensitivity + request.LegalRisk) / 2.0;

        // ÉTAPE E : L'équilibre final (Logique d'approbation)
        // C'est ici que tu définis ta règle : qu'est-ce qui valide un projet ?
        // Exemple simple : on valide si le gain de temps est supérieur à 1h et le risque moyen inférieur à 4
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