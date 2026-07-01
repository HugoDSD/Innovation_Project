# Modèle d'évaluation

Ce document décrit comment SobrIA passe de la description d'un workflow au
**verdict** final (`Recommandé` / `À optimiser` / `Déconseillé`).

## Principe général

SobrIA compare l'usage de l'IA à son **alternative sans IA**, mais la comparaison
ne porte que sur **un seul axe** : l'efficacité (le temps de travail économisé).
Les autres critères mesurent le **prix à payer** pour emprunter la voie de l'IA.

Le modèle se résume à une question : **le gain d'efficacité justifie-t-il les
coûts environnemental, économique et de risque ?**

## Architecture du calcul (4 étapes)

### 1. Simulation des tokens

À partir des informations du formulaire, un appel LLM **infère une session
d'agent type** sous forme de **timeline d'événements multi-tours** (prompts,
lectures de fichiers, réponses, sorties d'outils, hooks, sous-agents,
compaction). Ces événements s'accumulent dans une fenêtre de contexte unique,
visualisée **exactement comme le simulateur « context window » de Claude Code**.
Les tokens d'entrée et de sortie en sont dérivés (**occupation finale** de la
fenêtre — choix de facturation simple). Le contrat de données du simulateur est
détaillé dans [PLAN-IMPLEMENTATION.md](PLAN-IMPLEMENTATION.md). Ces tokens ne
sont **ni saisis par l'utilisateur ni des données fictives** — ils sont déduits
de la description.

### 2. Dérivation des variables de décision

- **Variables floues → appel LLM.** L'intégralité des entrées du formulaire
  (description, `employee_count`, `hours_per_run`, etc.) est transmise au LLM,
  qui estime les variables qui ne se calculent pas par formule : fraction de temps
  économisée par l'IA, sensibilité des données, exposition juridique.
- **Variables physiques → formules déterministes.** L'empreinte environnementale
  et le coût économique ne sont **pas** devinés par le LLM : à partir des tokens
  simulés (étape 1) et de `modèle × fournisseur`, ils se calculent par formule
  (facteurs d'émission, prix catalogue) → kWh, kg CO₂, litres, dollars.

### 3. Notation de 1 à 5

Des **règles codées en dur** mappent les variables de décision vers une **note
de 1 à 5** pour chacun des quatre critères.

**Convention de polarité (uniforme) : 5 = le plus favorable à l'usage de l'IA,
1 = le moins favorable.** Ainsi une note élevée est toujours « bonne » pour l'IA,
quel que soit le critère (un graphe radar plus grand = cas plus favorable).

### 4. Verdict

Une **cascade ordinale** sur les quatre notes produit le verdict (voir plus bas).

## Les critères

L'évaluation repose sur **un bénéfice** mis en balance avec **trois coûts**,
chacun noté de 1 à 5.

| Critère | Type | Note 5 signifie | Note 1 signifie | Variable notée |
| --- | --- | --- | --- | --- |
| **Efficacité** | Bénéfice | Temps économisé considérable | Gain négligeable | `value_saved` (€) |
| **Environnemental** | Coût | Empreinte très faible | Empreinte très élevée | `co2_kg` + `water_l` (50/50) |
| **Économique** | Coût | Très bon rapport coût/valeur | Dépense disproportionnée | `cost_usd_total / value_saved` |
| **Risque** | Coût | Aucun risque | Risque critique | `max(sensitivity_idx, legal_idx)` |

L'**Efficacité** reste le seul critère conceptuellement comparatif : le « temps
économisé » est un écart (`heures sans IA − heures avec IA`). Le critère
**Environnemental** combine CO₂ et eau à parts égales — les deux ne sont jamais
monétisés. Le critère **Économique** est un ratio coût/valeur : il exprime ce que
l'on dépense en dollars pour chaque euro de valeur créée. Le **Risque** est un
coût à part entière, jamais compensable par du temps gagné.

## Variables de décision (détail)

Liste de référence des variables produites par le pipeline.
`Formulaire` = saisie utilisateur ; `LLM` = inféré par appel LLM ;
`Formule` = calcul déterministe.

### Entrées (formulaire)

| Variable | Source | Description |
| --- | --- | --- |
| `workflow_description` | Formulaire | La tâche confiée à l'IA et ce qu'elle remplace (texte libre). |
| `run_frequency` | Formulaire | Nombre d'exécutions sur une période (ex. par mois). |
| `employee_count` | Formulaire | Nombre d'employés mobilisés sur ce workflow par exécution. |
| `hours_per_run` | Formulaire | Heures passées par chaque employé sur une exécution (avant IA). |
| `experience_level` | Formulaire | Niveau d'expérience : junior / confirmé / senior / expert. Détermine le taux horaire via une table de correspondance — n'est jamais demandé directement en euros. |
| `ai_model` | Formulaire | Modèle d'IA retenu. |
| `cloud_provider` | Formulaire | Fournisseur / région → facteurs carbone, eau, prix. |

> L'intégralité de ces champs est transmise au LLM en Phase 3 pour ancrer
> les estimations floues sur les données réelles du workflow.

### Étape 1 — Session de tokens

| Variable | Source | Description |
| --- | --- | --- |
| `session_timeline` | LLM | Liste ordonnée d'événements (prompt, lecture de fichier, réponse, sortie d'outil, hook, sous-agent, compaction), chacun avec sa catégorie, son coût en tokens, son rôle (entrée/sortie) et sa visibilité. Donnée qui alimente le simulateur. |
| `input_tokens` | Formule | Tokens de rôle « entrée » présents dans la fenêtre en fin de session (occupation finale). |
| `output_tokens` | Formule | Tokens de rôle « sortie » présents dans la fenêtre en fin de session. |
| `turns` | Formule | Nombre de tours de la session, dérivé de `session_timeline`. |

### Étape 2 — Variables par critère

**Efficacité** (Formulaire + LLM + Formule)

| Variable | Source | Unité | Détail |
| --- | --- | --- | --- |
| `hourly_rate` | Formule (table) | €/h | Dérivé de `experience_level` : junior 30 € · confirmé 50 € · senior 75 € · expert 110 €. |
| `ai_savings_fraction` | LLM | 0–1 | Fraction du temps total (`employee_count × hours_per_run`) que l'IA économise. Inférée à partir de l'ensemble des entrées du formulaire. |
| `hours_saved_per_run` | Formule | heures | `ai_savings_fraction × employee_count × hours_per_run` |
| `value_saved` | Formule | € | `hours_saved_per_run × hourly_rate × run_frequency` |

**Environnemental** (formule, depuis les tokens)

| Variable | Source | Unité |
| --- | --- | --- |
| `energy_kwh` | Formule | kWh (par exécution et total) |
| `co2_kg` | Formule | kg CO₂ = `energy_kwh × intensité_carbone(provider)` |
| `water_l` | Formule | litres = `energy_kwh × facteur_eau(provider)` |

**Économique** (formule)

| Variable | Source | Unité |
| --- | --- | --- |
| `cost_usd_per_run` | Formule | $ = `input_tokens × prix_in + output_tokens × prix_out` |
| `cost_usd_total` | Formule | $ = `cost_usd_per_run × run_frequency` |

**Risque** (LLM)

| Variable | Source | Échelle |
| --- | --- | --- |
| `data_sensitivity` | LLM | public / interne / confidentiel / réglementé |
| `legal_risk` | LLM | faible / modéré / élevé / critique (RGPD, secteur régulé) |

> Les seuils exacts qui transforment chaque variable en note 1–5 sont à définir
> et calibrer dans l'implémentation (voir
> [PLAN-IMPLEMENTATION.md](PLAN-IMPLEMENTATION.md)).

## Notation des critères

### Efficacité

Note `rateHigherBetter(value_saved, seuils)` — plus la valeur économisée est
élevée, meilleure est la note.

### Environnemental

La note combine CO₂ et eau à **parts égales (50/50)** :

```
co2_rating   = rateLowerBetter(co2_kg, seuils_co2)
water_rating = rateLowerBetter(water_l, seuils_eau)
note = round(0.5 × co2_rating + 0.5 × water_rating)
```

CO₂ et eau ne sont jamais monétisés ni agrégés en un score unique — ils
restent deux dimensions distinctes de l'empreinte, combinées à poids égal.

### Économique

La note porte sur le **ratio coût/valeur** (dollars dépensés par euro créé),
pas sur le coût absolu :

```
ratio = cost_usd_total / value_saved
note  = rateLowerBetter(ratio, seuils_ratio)
```

Un ratio de 0,01 signifie que l'on dépense 1 ¢ par euro économisé (excellent) ;
0,50 signifie que l'on dépense 50 ¢ par euro — là où la note passe à 1.
Le garde-fou d'efficacité (Gate 2) empêche toute division par zéro : si
`value_saved` est nul, la cascade s'arrête avant d'atteindre ce calcul.

### Risque

La note repose sur une **table de dominance** : on prend le pire des deux
indices ordinaux (`data_sensitivity` et `legal_risk`), puis on lit la note
directement :

```
worst = max(sensitivity_idx, legal_idx)   # 0 = le plus sûr, 3 = le plus risqué
```

| `worst` | Correspondance | Note |
| --- | --- | --- |
| 0 | public / faible | 5 |
| 1 | interne / modéré | 4 |
| 2 | confidentiel / élevé | 2 |
| 3 | réglementé / critique | 1 (veto) |

La note 3 est délibérément absente : le palier intermédiaire « confidentiel ou
élevé » tire déjà vers `Déconseillé` (note ≤ 2). Aucun score composite ni
pondération — la règle est lisible en une ligne.

## La logique de décision : une cascade de garde-fous

Le verdict n'est **pas** une moyenne pondérée des notes. C'est une **cascade de
garde-fous** (`gates`) évalués dans l'ordre ; la première condition vérifiée
détermine le verdict.

```
Garde-fou 1  — Risque ≤ 2 ?                              → DÉCONSEILLÉ   (veto absolu)
Garde-fou 2  — Efficacité ≤ 2 ?                          → DÉCONSEILLÉ   (bénéfice trop faible)
Garde-fou 3a — Environnemental ≤ 2 ET Économique ≤ 2 ?  → DÉCONSEILLÉ   (double dépassement)
Garde-fou 3b — Environnemental ≤ 2 OU Économique ≤ 2 ?  → À OPTIMISER
Sinon                                                    → RECOMMANDÉ
```

### Pourquoi une cascade plutôt qu'une moyenne pondérée

Une moyenne pondérée laisserait une bonne note sur un axe **masquer** un défaut
rédhibitoire sur un autre. Or trois situations sont **éliminatoires** :

1. **Le risque est un veto.** Aucun gain de temps ne justifie de faire transiter
   des données réglementées par un modèle. Un risque élevé impose `Déconseillé`
   à lui seul — et il n'est **pas** optimisable (contrairement au coût ou à
   l'empreinte, on ne le réduit pas avec un modèle plus léger).
2. **Sans bénéfice, rien à arbitrer.** Si l'IA n'économise quasiment pas de
   temps (Efficacité ≤ 2), le cas d'usage échoue d'emblée → `Déconseillé`.
3. **Un double dépassement n'est plus optimisable.** Si l'empreinte ET le coût
   sont tous deux inacceptables (Garde-fou 3a), « employer avec sobriété » n'est
   plus un conseil actionnable — il faut reconsidérer l'usage → `Déconseillé`.
4. **Coût ou empreinte seul sont les leviers d'optimisation** (Garde-fou 3b).
   Une fois l'usage jugé utile *et* sûr, si un seul axe dépasse, c'est
   précisément ce que la **sobriété** permet de réduire (modèle plus léger,
   mise en cache, moins d'appels) → `À optimiser`.

### Avantage : un verdict explicable

Chaque verdict pointe vers le garde-fou qui s'est déclenché ; l'interface affiche
**pourquoi** plutôt qu'un score opaque :

- *« Déconseillé : données réglementées (Risque 1/5) »*
- *« Déconseillé : gain de temps négligeable (Efficacité 2/5) »*
- *« Déconseillé : empreinte et coût tous deux trop élevés »*
- *« À optimiser : empreinte environnementale élevée (Environnemental 2/5) »*

## Deux choix de conception assumés

1. **Le carbone et l'eau ne sont jamais monétisés.** L'environnement reste un axe
   à part : une électricité bon marché ne doit pas pouvoir « racheter » des
   émissions. Essentiel à la thèse de sobriété.
2. **Le risque est un garde-fou strict**, pas un terme pondéré. Plus exigeant à
   calibrer qu'un score unique, mais cela empêche un défaut rédhibitoire de se
   cacher derrière une bonne moyenne.

## Reproductibilité

Les règles de notation et les formules physiques sont **déterministes** : à
variables de décision identiques, le verdict est toujours le même. La seule
source de variabilité est l'**estimation LLM** des variables floues ; elle doit
être stabilisée (température basse, sortie structurée) pour que deux évaluations
d'un même workflow restent cohérentes.

## Seuils

Les seuils de notation (ce qui fait passer un critère de 2 à 3, etc.) et les
bornes de la cascade sont des **paramètres à calibrer**. Ils doivent rester
explicites : une même saisie produit toujours le même verdict, et les chiffres
qui le justifient sont affichés à l'utilisateur.
