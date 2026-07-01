# Évolutions prévues

Ce document recense les évolutions volontairement reportées après le prototype,
avec la raison de leur report.

## Notation en unités réelles (modèle « A »)

**Aujourd'hui (modèle « B »).** Le verdict est calculé à partir des quatre notes
ordinales de 1 à 5, via une cascade de garde-fous (voir
[MODELE-EVALUATION.md](MODELE-EVALUATION.md)).

**Évolution visée (modèle « A »).** Faire reposer le verdict sur les **variables
de décision en unités réelles** (euros, dollars, kg CO₂, litres) plutôt que sur
les notes. Les notes 1–5 ne disparaissent pas : elles deviennent une **couche
d'affichage** (radar, badges) dérivée des mêmes variables, tandis que le calcul
du verdict s'appuie sur l'arithmétique réelle.

**Pourquoi c'est mieux.** Le modèle B perd la comparaison directe
euros-contre-dollars : « en vaut la peine » devient un seuil sur la note
d'Efficacité seule, et non un véritable *bénéfice moins coût*. Le modèle A
restaure ce calcul :

```
Garde-fou « valeur nette » :  € économisés  −  coût IA $  ≤ 0   → DÉCONSEILLÉ
Garde-fou « optimisation »  :  kg CO₂ (ou litres) par heure gagnée trop élevé
                                                                  → À OPTIMISER
```

On retrouve une vraie balance bénéfice/coût, des seuils plus fins, et un verdict
qui colle mieux à une décision d'entreprise réelle.

**Pourquoi c'est reporté.** Le modèle A exige une **estimation fiable en unités
réelles** (heures économisées et coût horaire crédibles), donc une calibration
plus lourde et une plus grande sensibilité au bruit des estimations LLM. Le
modèle B, ordinal, est plus robuste à ce bruit et suffit à valider le concept.

## Autres évolutions

- **Facturation cumulée des tokens** — comptabiliser le ré-envoi du contexte à
  chaque tour de la session (au lieu de la seule occupation finale, choix « A »),
  pour un coût et une empreinte plus réalistes d'une session agentique.
- **Modélisation explicite de l'alternative sans IA** — noter l'empreinte et le
  coût de la voie *sans IA* (aujourd'hui supposés négligeables) pour une
  comparaison réellement symétrique sur les trois coûts, et pas seulement sur
  l'efficacité.
- **Simulation d'un vrai prompt** au sein de l'outil.
- **Comparaison de plusieurs modèles côte à côte** pour recommander le plus
  adapté.
- **Hébergement cloud de la base de données** (aujourd'hui locale par
  développeur).
