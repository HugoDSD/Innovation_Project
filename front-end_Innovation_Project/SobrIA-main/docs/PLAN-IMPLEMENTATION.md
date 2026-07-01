# Plan d'implémentation

Ce document synthétise l'expérience utilisateur, le pipeline de traitement et les
tâches d'implémentation de SobrIA. Il sert de référence aux agents qui
implémentent le produit. Le **modèle de décision** (critères, notation, cascade,
variables) est défini dans [MODELE-EVALUATION.md](MODELE-EVALUATION.md) ; les
évolutions reportées dans [EVOLUTIONS.md](EVOLUTIONS.md).

## Expérience utilisateur (3 écrans)

1. **Formulaire workflow.** L'utilisateur renseigne les informations du
   workflow : description (texte libre) de la tâche et de ce qu'elle remplace ;
   le contexte humain actuel (nombre d'employés, heures par exécution, niveau
   d'expérience) ; fréquence d'exécution, modèle d'IA, fournisseur cloud.
2. **Simulation de session d'agent.** Un appel LLM infère la consommation de
   tokens d'une session type **à partir des informations du formulaire** (les
   tokens d'entrée ne sont ni saisis ni fictifs). Une jauge animée visualise le
   remplissage de la fenêtre de contexte, à la manière de la vue
   [« context window » de Claude Code](https://code.claude.com/docs/en/context-window).
3. **Résultat.** À partir des tokens simulés et des informations du workflow, le
   système infère les variables de décision, calcule les notes et le verdict, et
   affiche les **quatre critères notés (1–5)** + le **résultat final** + les
   chiffres qui le justifient.

## Simulateur de session d'agent

L'écran 2 reproduit **à l'identique** le simulateur
[« context window » de Claude Code](https://code.claude.com/docs/en/context-window).
Une **session d'agent** y est une séquence **multi-tours** : prompts, lectures de
fichiers, réponses, sorties d'outils, règles, hooks, sous-agents, compaction.

### Modèle

- **Fenêtre unique** de taille `MAX` = fenêtre de contexte du modèle choisi
  (via models.dev). La jauge affiche l'occupation `total / MAX`, colorée en
  vert / ambre / rouge à 50 % / 75 %.
- **Timeline d'événements** : chaque événement porte
  `{ ordre, kind, catégorie, label, tokens, rôle (entrée/sortie), visibilité }`.
  Catégories (légende) : Système, CLAUDE.md, Mémoire, Compétences, MCP, Règles,
  Utilisateur, Fichiers, Sortie, Claude, Hooks.
- **Les tours empilent, ils ne multiplient pas.** Chaque tour ajoute ses
  événements ; `total = Σ tokens des événements présents dans la fenêtre`.
- **Sous-agents** : fenêtre séparée ; leurs lectures (`subTokens`) ne comptent
  **pas** dans le total principal.
- **/compact** : remplace la conversation par un résumé (~12 % des tokens
  résumés) ; le contenu de démarrage se recharge.

### Origine de la timeline

- **Phase 1** : `session_timeline` est un **stub aléatoire** — le simulateur
  tourne sans LLM.
- **Phase 3** : `session_timeline` est **inférée par appel LLM** depuis le
  formulaire. Même rendu dans les deux cas.

### Facturation (choix « A » — logique simple)

`input_tokens` et `output_tokens` sont l'**occupation finale** de la fenêtre,
ventilée par rôle — exactement le nombre affiché par la jauge. Les formules les
utilisent tels quels :

```
cost_usd_per_run = input_tokens × prix_in + output_tokens × prix_out
```

Choix **volontairement simple** : il ne facture pas le ré-envoi du contexte à
chaque tour (facturation cumulée), donc il sous-estime le coût réel d'une session
agentique. La variante cumulée est une évolution possible (voir
[EVOLUTIONS.md](EVOLUTIONS.md)).

## Pipeline de traitement

Le pipeline suit les 4 étapes de [MODELE-EVALUATION.md](MODELE-EVALUATION.md) :

1. **Simulation des tokens** — appel LLM : `formulaire` → `input_tokens`,
   `output_tokens`, `turns`.
2. **Dérivation des variables de décision**
   - Appel LLM (variables floues) : `ai_savings_fraction`, `data_sensitivity`,
     `legal_risk`. L'intégralité des entrées du formulaire est transmise au LLM
     pour ancrer les estimations sur le contexte réel.
   - Formules déterministes : `hourly_rate` (table `experience_level`) ;
     `hours_saved_per_run`, `value_saved` (depuis `ai_savings_fraction`) ;
     `energy_kwh`, `co2_kg`, `water_l`, `cost_usd_per_run`, `cost_usd_total`
     (depuis les tokens, le modèle et le fournisseur).
3. **Notation 1–5** — règles codées en dur (polarité uniforme : 5 = le plus
   favorable à l'IA) :
   - Efficacité : `rateHigherBetter(value_saved)`
   - Environnemental : `round(0.5 × rateCO₂ + 0.5 × rateEau)`
   - Économique : `rateLowerBetter(cost_usd_total / value_saved)`
   - Risque : table de dominance sur `max(sensitivity_idx, legal_idx)` → 5/4/2/1
4. **Verdict** — cascade de garde-fous sur les 4 notes :
   `Risque ≤ 2 → Déconseillé` · `Efficacité ≤ 2 → Déconseillé` ·
   `Env ≤ 2 ET Éco ≤ 2 → Déconseillé` · `Env ≤ 2 OU Éco ≤ 2 → À optimiser` ·
   sinon `Recommandé`.

## Phases d'implémentation

Chaque variable de décision a une **source cible** (`Formulaire` / `LLM` /
`Formule`, voir MODELE-EVALUATION), mise en place progressivement. Tant qu'une
variable n'est pas branchée sur sa source réelle, elle est fournie par un
**stub**.

- **Phase 1 — Frontend & flux (stubs aléatoires).** **Toutes** les variables de
  décision sont **générées aléatoirement**. Objectif : construire et tester
  l'interface de bout en bout (formulaire → simulation → notes → verdict →
  historique) sans dépendre du LLM ni des formules. La notation 1–5 et la
  cascade de verdict sont, elles, déjà réelles — elles tournent sur les valeurs
  aléatoires.
- **Phase 2 — Variables calculées.** Remplacer les stubs des **variables
  physiques** par les **formules déterministes** : `cost_usd_per_run` /
  `cost_usd_total` à partir des tokens et du prix par token du modèle ;
  `energy_kwh` / `co2_kg` / `water_l` à partir des facteurs par fournisseur.
- **Phase 3 — Variables inférées.** Remplacer les stubs des **variables floues**
  par les **appels LLM** : tokens (`input_tokens`, `output_tokens`, `turns`),
  puis Efficacité (`ai_savings_fraction`) et Risque (`data_sensitivity`,
  `legal_risk`). Le prompt LLM reçoit l'intégralité des entrées du formulaire.

Les phases 2 et 3 peuvent être déployées **variable par variable** : chaque stub
est remplacé indépendamment, sans casser le flux.

## Sources de données

- **Métadonnées des modèles → API [models.dev](https://models.dev/).** Prix par
  token (`prix_in`, `prix_out`), fenêtre de contexte et autres caractéristiques
  par modèle proviennent de cette API. C'est la source des données nécessaires au
  calcul du critère **Économique** (et aux bornes du simulateur de tokens).

## Composants à construire

- **Formulaire workflow** — capture des entrées (voir « Entrées » dans
  MODELE-EVALUATION).
- **Simulateur de tokens** — appel LLM d'inférence + jauge visuelle de la fenêtre
  de contexte.
- **Inférence des variables floues** — appel LLM (Efficacité, Risque) à sortie
  structurée.
- **Moteur de formules** — calcul environnemental + économique depuis les tokens
  (facteurs d'émission/eau par fournisseur, table de prix par modèle).
- **Règles de notation** — mapping codé en dur variable → note 1–5 par critère.
- **Moteur de verdict** — cascade de garde-fous + libellé explicatif du
  garde-fou déclenché.
- **Écran de résultat** — 4 critères notés (radar/barres) + verdict + chiffres.
- **Persistance** — sauvegarde de chaque évaluation (historique utilisateur).

## Variables de décision

La liste complète des variables (source, unité, formules) est la **section de
référence** « Variables de décision (détail) » de
[MODELE-EVALUATION.md](MODELE-EVALUATION.md). Elle est à implémenter telle quelle.

## À définir / calibrer

Ces paramètres sont délibérément laissés ouverts et doivent être fixés à
l'implémentation :

- **Prompt d'inférence des tokens** (étape 1) et **prompt d'inférence des
  variables floues** (étape 2) — à sortie structurée, température basse pour la
  reproductibilité.
- **Facteurs par fournisseur** — intensité carbone, facteur eau, énergie par
  token (ou par modèle).
- **Prix par token** par modèle (`prix_in`, `prix_out`) — récupérés via l'API
  [models.dev](https://models.dev/) (voir « Sources de données »).
- **Seuils de notation 1–5** par critère (ce qui fait passer une variable d'une
  note à l'autre).
- **Seuil de veto Risque** et bornes de la cascade.
- **Seuils de la table de dominance Risque** (actuellement : worst 0→5, 1→4, 2→2, 3→1).
- **Poids CO₂ / eau** pour la note Environnemental (actuellement 50/50).
- **Seuils du ratio coût/valeur** pour la note Économique.

## Hors périmètre (voir EVOLUTIONS.md)

Notation en unités réelles (modèle « A »), modélisation explicite de
l'alternative sans IA, simulation d'un vrai prompt, comparaison de modèles,
hébergement cloud de la base.
