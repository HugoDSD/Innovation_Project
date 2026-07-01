using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Back_end_Innovation_Project.APP.DTOs;

namespace Back_end_Innovation_Project.LOGIC.Services;

public interface IModelsDevService
{
    Task<(double InputCostPerToken, double OutputCostPerToken)> GetModelPricingAsync(string modelName);
}

public class ModelsDevService : IModelsDevService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "ModelsDevCatalog";
    // pour nos deux model manquant dans models.dev
    private readonly Dictionary<string, (double In, double Out)> _staticFallbackPricing = new()
    {
        { "GPT OSS 20B", (1.5e-07, 6.0e-07) },
        { "DeepSeek V3.1", (1.4e-07, 2.8e-07) }
    };

    public ModelsDevService(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }

    public async Task<(double InputCostPerToken, double OutputCostPerToken)> GetModelPricingAsync(string modelName)
    {
        // On appelle maintenant models.json pour avoir les prix
        if (!_cache.TryGetValue("ModelsDevPricing", out Dictionary<string, ModelDevInfo>? models))
        {
            var response = await _httpClient.GetAsync("https://models.dev/models.json");
            response.EnsureSuccessStatusCode();
            
            var jsonString = await response.Content.ReadAsStringAsync();
            models = JsonSerializer.Deserialize<Dictionary<string, ModelDevInfo>>(jsonString);
    
            if (models != null)
            {
                _cache.Set("ModelsDevPricing", models, TimeSpan.FromHours(24));
            }
        }

        // 2. Recherche du prix
        if (models != null && models.TryGetValue(modelName, out var model) && model.Pricing != null)
        {
            return (model.Pricing.Prompt / 1_000_000.0, model.Pricing.Completion / 1_000_000.0);
        }
        // On utilise le dictionnaire statique pour 20B / V3.1 
        if (_staticFallbackPricing.TryGetValue(modelName, out var fallback))
        {
            return (fallback.In, fallback.Out);
        }

        throw new ArgumentException($"Modèle '{modelName}' non trouvé (ni dans API, ni en statique).");
    }
}