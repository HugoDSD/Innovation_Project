# Présentation du projet

## Le problème

En 2026, à mesure que les LLM deviennent de plus en plus performants et que
l'agentification de l'IA accélère son intégration dans le workflow des équipes,
l'IA s'invite dans des tâches toujours plus nombreuses — souvent par réflexe,
rarement par décision réfléchie.

Pourtant, utiliser l'IA générative dans un workflow n'est jamais gratuit. Chaque
requête consomme de l'électricité et de l'eau, coûte de l'argent et peut
comporter des risques juridiques ou de confidentialité des données. La plupart
des équipes adoptent l'IA sans jamais mesurer si la valeur qu'elle apporte
justifie réellement ces coûts — elles supposent simplement que c'est le cas.

## Ce qu'est ce projet

**SobrIA** — dont le nom vient de **« sobriété »** — porte une conviction
simple : l'IA doit être intégrée à un workflow **avec sobriété**, c'est-à-dire
de façon mesurée et seulement lorsqu'elle apporte une valeur qui justifie son
impact. Le nom mêle la *sobriété* à l'*IA* pour rappeler cette démarche à chaque
usage.

Concrètement, c'est une **plateforme d'évaluation de l'impact de l'IA sur un
workflow** : un outil web qui aide une entreprise à décider si l'utilisation de
l'IA pour un workflow donné en vaut vraiment la peine. L'utilisateur décrit
l'usage qu'il compte faire de l'IA, et l'outil renvoie un verdict clair et fondé
sur des données plutôt qu'une simple intuition.

## À qui il s'adresse

- **Les entreprises** (TPE, startups, grandes entreprises) qui envisagent une
  fonctionnalité ou un flux de travail basé sur l'IA — par exemple traduire les
  contenus de son site à la volée avec un LLM plutôt que de gérer des chaînes de
  traduction figées, générer automatiquement des descriptions de produits, ou
  trier et résumer les tickets du support client.
- **Les équipes informatiques et de direction** qui doivent justifier une
  décision liée à l'IA.
- **Les fonctions développement durable / RSE** responsables de l'empreinte
  environnementale des choix technologiques.

## Comment ça fonctionne, du point de vue de l'utilisateur

1. **Renseigner le workflow.** Via un formulaire, l'utilisateur décrit la tâche
   confiée à l'IA et ce qu'elle remplace, sa fréquence d'exécution, et choisit le
   modèle d'IA et le fournisseur cloud.
2. **Simuler une session d'agent.** À partir des informations du formulaire, un
   appel LLM **infère la consommation de tokens** d'une session type, visualisée
   par une jauge qui se remplit segment par segment — à la manière de la vue
   « context window » de Claude Code. L'utilisateur ne saisit aucun nombre : les
   tokens sont déduits de sa description.
3. **Obtenir l'évaluation.** À partir des tokens simulés et des informations du
   workflow, un LLM infère les variables des quatre critères ; des règles codées
   en dur en déduisent le verdict. La plateforme affiche les **quatre critères
   notés (1–5)** et le **résultat final**, avec les chiffres qui le justifient.

## Ce qu'il mesure

Pour chaque scénario, l'outil évalue **un bénéfice** mis en balance avec **trois
coûts** :

- **Efficacité** *(bénéfice)* — le temps de travail économisé par rapport à
  l'alternative sans IA, valorisé en euros (heures gagnées × coût horaire).
- **Environnemental** *(coût)* — énergie consommée (kWh), carbone émis (kg CO₂,
  sur la base du mix électrique français) et eau consommée (litres) par l'IA.
- **Économique** *(coût)* — le coût de l'utilisation de l'IA, en dollars.
- **Risque** *(coût)* — un score construit à partir de la sensibilité des
  données et du risque juridique.

À partir de ces critères, la plateforme rend un verdict sur une échelle à trois
niveaux qui indique, en somme, si l'usage de l'IA en vaut la peine :

- **Recommandé** — la valeur justifie l'impact ; l'IA est un bon choix pour ce
  workflow.
- **À optimiser** — l'usage vaut le coup, mais son empreinte est trop élevée ;
  il faut l'employer **avec sobriété** (modèle plus léger, moins d'appels,
  mise en cache).
- **Déconseillé** — l'impact dépasse la valeur apportée, ou le risque est trop
  élevé ; mieux vaut s'en passer.

La logique exacte qui relie ces critères au verdict est détaillée dans
[docs/MODELE-EVALUATION.md](docs/MODELE-EVALUATION.md). Chaque évaluation est
sauvegardée afin que l'utilisateur puisse consulter son historique.

## Documentation

- [docs/MODELE-EVALUATION.md](docs/MODELE-EVALUATION.md) — critères, notation et
  logique de décision.
- [docs/PLAN-IMPLEMENTATION.md](docs/PLAN-IMPLEMENTATION.md) — expérience
  utilisateur, pipeline et tâches d'implémentation.
- [docs/EVOLUTIONS.md](docs/EVOLUTIONS.md) — évolutions prévues.
