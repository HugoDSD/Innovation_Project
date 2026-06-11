using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Back_end_Innovation_Project.MODEL;
using Back_end_Innovation_Project.PERSIST;
using Back_end_Innovation_Project.COMMON;
using Back_end_Innovation_Project.LOGIC.Services;
using Back_end_Innovation_Project.LOGIC.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CONFIGURATION DES SERVICES (Le conteneur de dépendances)
// ==========================================

// Ajout des contrôleurs d'API
builder.Services.AddControllers();

// Configuration de la base de données PostgreSQL
builder.Services.AddDbContext<AppDb>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuration de la sécurité (Identity)
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDb>();

// ==========================================
// 2. CONSTRUCTION DE L'APPLICATION
// ==========================================
builder.Services.AddScoped<IAuthService, AuthService>(); // On dit à ASP.NET Core que chaque fois qu'on demande un IAuthService, il doit nous donner une instance de AuthService (qui est la classe concrète qui implémente notre interface IAuthService)
var app = builder.Build();

// ==========================================
// 3. CONFIGURATION DU PIPELINE HTTP (Les Middlewares)
// ==========================================


// app.UseMiddleware<ExceptionHandlingMiddleware>();  // Décommente cette ligne une fois qu'on auras ajouté la classe ExceptionHandlingMiddleware à ton projet

// Activation de la sécurité dans le pipeline
app.UseAuthentication();
app.UseAuthorization();

// ==========================================
// 4. CONFIGURATION DES ROUTES (Endpoints)
// ==========================================


/*
    En lisant cette ligne, notre API va, dans les coulisses, générer et exposer automatiquement tout un groupe de routes pré-codées pour gérer la sécurité.
    Ainsi on a déjà des routes prêtes à l'emploi pour :
    - S'inscrire (/register)
    - Se connecter (/login)
    - Se déconnecter (/logout)
    - Gérer les rôles (ajouter, supprimer, etc.)
    - Gérer les utilisateurs (voir la liste, supprimer, etc.)
    - Gérer les sessions (voir les sessions actives, les terminer, etc.)
    - Gérer les tokens (générer, révoquer, etc.)
    - Gérer les mots de passe (changer, réinitialiser, etc.)
*/



// Expose les futures routes de tes propres contrôleurs
app.MapControllers();

// Lancement du serveur
app.Run();