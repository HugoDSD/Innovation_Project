<script setup>
import { useRouter } from 'vue-router'
import apiService from '../services/apiService'

const router = useRouter()

const goBack = () => {
  if (window.history.length > 1) router.back()
  else router.push('/')
}

const handleLogout = () => {
  apiService.logout()
  router.push('/login')
}
</script>

<template>
  <div class="app-wrapper">

    <header class="header">
      <div class="header-content">
        <h1>SobrIA</h1>

        <div class="header-actions">
          <button class="history-btn" @click="goBack">← Retour</button>
          <button class="logout-btn" @click="handleLogout">Déconnexion</button>
        </div>
      </div>
    </header>

    <div class="doc-body">
      <div class="doc-card">

        <h2>Comprendre le calcul complet du système</h2>

        <p class="intro">
          Cette page explique comment SobrIA transforme une requête IA en un verdict
          (Recommandé / À optimiser / Déconseillé), en croisant impact environnemental,
          coût économique et risque lié aux données.
        </p>

        <!-- ============================================================ -->
        <!-- 1. PIPELINE + FORMULE CENTRALE -->
        <!-- ============================================================ -->
        <div class="doc-section">
          <h3>1. Le pipeline de calcul, de la requête au verdict</h3>

          <p>
            Toute évaluation suit exactement la même chaîne de calcul, qu'il s'agisse
            de GPT, Claude ou DeepSeek. Voici la formule complète :
          </p>

          <div class="master-formula">
            <div class="mf-line"><span class="mf-label">Tokens totaux</span> = Tokens input + Tokens output</div>
            <div class="mf-arrow">↓</div>
            <div class="mf-line"><span class="mf-label">Énergie (kWh)</span> = Tokens totaux × Énergie/token (modèle) × Fréquence d'usage</div>
            <div class="mf-arrow">↓</div>
            <div class="mf-split">
              <div class="mf-line small"><span class="mf-label">CO₂ (kg)</span> = Énergie × 0.0801 <em>(mix électrique français)</em></div>
              <div class="mf-line small"><span class="mf-label">Eau (L)</span> = Énergie × Eau/kWh (modèle)</div>
            </div>
            <div class="mf-arrow">↓</div>
            <div class="mf-line"><span class="mf-label">Coût (€)</span> = (Tokens input × Coût input + Tokens output × Coût output) × Fréquence</div>
            <div class="mf-arrow">↓</div>
            <div class="mf-line"><span class="mf-label">Valeur générée (€)</span> = Fraction de gain IA × Effectif × Heures/exécution × Taux horaire × Fréquence</div>
            <div class="mf-arrow">↓</div>
            <div class="mf-line highlight">Chaque résultat est converti en note de 1 à 5</div>
            <div class="mf-arrow">↓</div>
            <div class="mf-line final">Verdict final = application des règles de blocage</div>
          </div>

          <h4 class="sub-heading">Les 3 critères qui déterminent le verdict</h4>

          <div class="columns">

            <div class="col col-env">
              <h2>Environnemental</h2>
              <ul>
                <li>CO₂ émis (kg)</li>
                <li>Eau consommée (L)</li>
                <li>Énergie consommée (kWh)</li>
              </ul>
              <div class="verdict-pill pill-good">Note 5 = impact minimal</div>
            </div>

            <div class="col col-eco">
              <h2>Économique</h2>
              <ul>
                <li>Coût total API (€)</li>
                <li>Ratio coût / valeur générée</li>
              </ul>
              <div class="verdict-pill pill-mid">Note 5 = très rentable</div>
            </div>

            <div class="col col-risk">
              <h2>Social &amp; Risque</h2>
              <ul>
                <li>Sensibilité des données</li>
                <li>Risque légal</li>
                <li>Impact métier</li>
              </ul>
              <div class="verdict-pill pill-veto">Note ≤ 2 = blocage</div>
            </div>

          </div>
        </div>

        <!-- ============================================================ -->
        <!-- 2. TRIANGLE DES 3 IA -->
        <!-- ============================================================ -->
        <div class="doc-section">
          <h3>2. Les trois intelligences artificielles</h3>
          <p>
            Le système ne dit pas "un modèle est meilleur qu'un autre" : chaque IA a un
            profil et des cas d'usage où elle excelle. C'est ce profil qui sert de base
            au calcul, quel que soit le modèle réellement demandé. Quand vous indiquez votre cas d'usage
            (ex. "code dev"), le système sait quel modèle est théoriquement le plus
            adapté qualitativement, même si le calcul d'impact reste fait sur le modèle
            réellement sélectionné. C'est ce qui permet de proposer une alternative
            "meilleur compromis qualité" en plus des alternatives environnementale et économique.
          </p>

          <div class="triangle">
            <div class="triangle-top">
              <div class="ai-card ai-claude">
                <div class="ai-badge">C</div>
                <h4>Claude</h4>
                <ul class="ai-specialties">
                  <li>Code dev</li>
                  <li>Analyse de document</li>
                  <li>Rédaction de rapport</li>
                </ul>
              </div>
            </div>

            <div class="triangle-bottom">
              <div class="ai-card ai-gpt">
                <div class="ai-badge">G</div>
                <h4>GPT</h4>
                <ul class="ai-specialties">
                  <li>Rédaction business</li>
                  <li>Code du quotidien (SQL, etc.)</li>
                  <li>Assistant quotidien</li>
                </ul>
              </div>

              <div class="ai-card ai-deepseek">
                <div class="ai-badge">D</div>
                <h4>DeepSeek</h4>
                <ul class="ai-specialties">
                  <li>Décisions logiques</li>
                  <li>Code technique (debug, algorithme)</li>
                  <li>Raisonnement complexe</li>
                </ul>
              </div>
            </div>
          </div>
        </div>

        <!-- ============================================================ -->
        <!-- 3. LOGIQUE DE RISQUE -->
        <!-- ============================================================ -->
        <div class="doc-section">
          <h3>3. Comprendre la logique de risque</h3>

          <p>
            Le risque est évalué sur deux échelles indépendantes, chacune à 4 niveaux.
            Le système ne fait jamais de moyenne entre les deux : il retient toujours
            le plus défavorable.
          </p>

          <div class="risk-scales">
            <div class="risk-scale">
              <p class="risk-scale-title">Sensibilité des données</p>
              <div class="scale-track">
                <div class="scale-step">Public</div>
                <div class="scale-step">Interne</div>
                <div class="scale-step">Confidentiel</div>
                <div class="scale-step scale-critical">Réglementé</div>
              </div>
            </div>

            <div class="risk-scale">
              <p class="risk-scale-title">Risque légal</p>
              <div class="scale-track">
                <div class="scale-step">Faible</div>
                <div class="scale-step">Modéré</div>
                <div class="scale-step">Élevé</div>
                <div class="scale-step scale-critical">Critique</div>
              </div>
            </div>
          </div>

          <div class="formula centered-formula">
            Risque final = MAX( indice sensibilité , indice risque légal )
          </div>

          <p>
            Cet indice (de 0 à 3) est ensuite converti en note de risque via une
            table volontairement punitive : dès qu'on atteint le niveau "élevé/confidentiel",
            la note chute brutalement pour déclencher le blocage.
          </p>

          <div class="risk-table">
            <div class="risk-table-row risk-table-head">
              <div>Indice le plus élevé retenu</div>
              <div>Note de risque</div>
            </div>
            <div class="risk-table-row">
              <div>0 — Public / Faible</div>
              <div><span class="score-badge score-5">5</span></div>
            </div>
            <div class="risk-table-row">
              <div>1 — Interne / Modéré</div>
              <div><span class="score-badge score-4">4</span></div>
            </div>
            <div class="risk-table-row">
              <div>2 — Confidentiel / Élevé</div>
              <div><span class="score-badge score-2">2</span></div>
            </div>
            <div class="risk-table-row">
              <div>3 — Réglementé / Critique</div>
              <div><span class="score-badge score-1">1</span></div>
            </div>
          </div>
        </div>

        <!-- ============================================================ -->
        <!-- 4. GATES / REGLES DE DECISION -->
        <!-- ============================================================ -->
        <div class="doc-section">
          <h3>4. Les règles de décision</h3>
          <p>
            Une fois les 4 notes calculées (Efficacité, Environnemental, Économique, Risque),
            le verdict final applique ces règles, dans cet ordre :
          </p>

          <div class="gates-table">
            <div class="gate-row gate-veto">
              <div class="gate-condition">Risque ≤ 2</div>
              <div class="gate-verdict">🔴 Déconseillé</div>
              <div class="gate-reason">Un risque élevé n'est jamais compensable par du temps gagné.</div>
            </div>
            <div class="gate-row gate-veto">
              <div class="gate-condition">Efficacité ≤ 2</div>
              <div class="gate-verdict">🔴 Déconseillé</div>
              <div class="gate-reason">Gain de temps négligeable : sans bénéfice réel, rien à arbitrer.</div>
            </div>
            <div class="gate-row gate-veto">
              <div class="gate-condition">Environnemental ≤ 2 <strong>ET</strong> Économique ≤ 2</div>
              <div class="gate-verdict">🔴 Déconseillé</div>
              <div class="gate-reason">Empreinte et coût sont tous les deux mauvais : optimiser ne suffit plus.</div>
            </div>
            <div class="gate-row gate-warning">
              <div class="gate-condition">Environnemental ≤ 2 <strong>OU</strong> Économique ≤ 2</div>
              <div class="gate-verdict">🟠 À optimiser</div>
              <div class="gate-reason">Usage utile et sûr, mais un point noir à corriger : à employer avec sobriété.</div>
            </div>
            <div class="gate-row gate-ok">
              <div class="gate-condition">Aucune condition ci-dessus déclenchée</div>
              <div class="gate-verdict">🟢 Recommandé</div>
              <div class="gate-reason">La valeur générée justifie l'impact sur les quatre critères.</div>
            </div>
          </div>
        </div>


      </div>
    </div>

  </div>
</template>

<style scoped>
.app-wrapper {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background: #194a3e;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

ul {
  list-style: none;          /* enlève les puces par défaut */
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 0.6rem;
}

li {
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.08);
  padding: 0.8rem 1rem;
  border-radius: 10px;
  color: #eaf6f2;
  line-height: 1.5;
  transition: all 0.2s ease;
}

.header {
  background: #407a69;
  color: white;
  padding: 1.2rem 0;
  position: sticky;
  top: 0;
}

.header-content {
  max-width: 1400px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  padding: 0 1.5rem;
}

h1 { margin: 0; }

.header-actions {
  display: flex;
  gap: 0.8rem;
}

.history-btn, .logout-btn {
  padding: 0.6rem 1rem;
  border-radius: 10px;
  color: white;
  cursor: pointer;
}

.logout-btn {
  background: linear-gradient(135deg, #3fbf8f, #2f9d74);
  border: none;
}

.history-btn {
  background: rgba(255,255,255,0.08);
  border: 1px solid rgba(255,255,255,0.2);
}

.doc-body {
  flex: 1;
  display: flex;
  justify-content: center;
  padding: 3rem 1rem;
}

.doc-card {
  width: 100%;
  max-width: 1050px;
  background: #407a69;
  color: white;
  border-radius: 18px;
  padding: 2rem;
}

.intro {
  color: rgba(255,255,255,0.85);
  font-size: 1.05rem;
  padding-top: 1rem;
}

.doc-section {
  margin-top: 2.5rem;
  border-top: 1px solid rgba(255,255,255,0.2);
  padding-top: 1.2rem;
}

.doc-section p{
  padding-top: 0.5rem;
  padding-bottom: 1rem;
}

.sub-heading {
  margin-top: 2rem;
  margin-bottom: 1rem;
  text-align: center;
}

/* ---------- Formule maîtresse ---------- */
.master-formula {
  background: rgba(0,0,0,0.22);
  border-radius: 14px;
  padding: 1.6rem;
  font-family: monospace;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.4rem;
  text-align: center;
}

.mf-line {
  font-size: 0.95rem;
  line-height: 1.5;
}

.mf-line.small { font-size: 0.88rem; }

.mf-label {
  color: #a8f0d4;
  font-weight: bold;
}

.mf-arrow {
  color: rgba(255,255,255,0.5);
  font-size: 1rem;
}

.mf-split {
  display: flex;
  gap: 2rem;
  flex-wrap: wrap;
  justify-content: center;
}

.mf-line.highlight {
  color: #ffd27a;
  font-weight: bold;
  margin-top: 0.3rem;
}

.mf-line.final {
  color: #3fbf8f;
  font-weight: bold;
  font-size: 1.05rem;
  margin-top: 0.3rem;
}

.formula {
  background: rgba(0,0,0,0.2);
  padding: 1rem;
  border-radius: 10px;
  font-family: monospace;
}

.centered-formula {
  text-align: center;
  font-size: 1rem;
  margin: 1.5rem 0;
}

/* ---------- 3 colonnes critères ---------- */
.columns {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}

.col {
  flex: 1;
  min-width: 220px;
  background: rgba(0,0,0,0.15);
  padding: 1.2rem;
  border-radius: 12px;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.col-icon { font-size: 1.8rem; }

.col-desc {
  font-size: 0.88rem;
  color: rgba(255,255,255,0.8);
}

.verdict-pill {
  margin-top: auto;
  align-self: flex-start;
  padding: 0.35rem 0.8rem;
  border-radius: 999px;
  font-size: 0.8rem;
  font-weight: bold;
}

.pill-good { background: rgba(63,191,143,0.25); color: #a8f0d4; }
.pill-mid { background: rgba(255,210,122,0.2); color: #ffd27a; }
.pill-veto { background: rgba(224,90,78,0.25); color: #ff9c8f; }

/* ---------- Triangle des IA ---------- */
.triangle {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1.5rem;
  margin: 1.5rem 0;
}

.triangle-bottom {
  display: flex;
  gap: 1.5rem;
  flex-wrap: wrap;
  justify-content: center;
}

.ai-card {
  background: rgba(0,0,0,0.15);
  border-radius: 14px;
  padding: 1.4rem;
  width: 240px;
  text-align: center;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
}

.ai-badge {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 1.3rem;
  color: #194a3e;
}

.ai-gpt .ai-badge { background: #74d3a0; }
.ai-claude .ai-badge { background: #ffb37a; }
.ai-deepseek .ai-badge { background: #7ac3ff; }

.ai-tagline {
  font-size: 0.85rem;
  color: rgba(255,255,255,0.75);
}

.ai-specialties {
  list-style: none;
  padding: 0;
  margin: 0.4rem 0 0;
  font-size: 0.85rem;
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
}

.ai-specialties li {
  background: rgba(255,255,255,0.08);
  border-radius: 6px;
  padding: 0.3rem 0.5rem;
}

.mapping-note {
  margin-top: 1.5rem;
  font-size: 0.9rem;
  color: rgba(255,255,255,0.8);
  background: rgba(0,0,0,0.12);
  padding: 1rem;
  border-radius: 10px;
}

/* ---------- Echelles de risque ---------- */
.risk-scales {
  display: flex;
  flex-direction: column;
  gap: 1.2rem;
  margin: 1.2rem 0;
}

.risk-scale-title {
  margin: 0 0 0.5rem;
  font-weight: bold;
  color: rgba(255,255,255,0.85);
}

.scale-track {
  display: flex;
  border-radius: 10px;
  overflow: hidden;
}

.scale-step {
  flex: 1;
  padding: 0.6rem 0.5rem;
  text-align: center;
  font-size: 0.85rem;
  background: rgba(255,255,255,0.1);
  border-right: 1px solid rgba(255,255,255,0.15);
}

.scale-step:last-child { border-right: none; }

.scale-step.scale-critical {
  background: rgba(224,90,78,0.35);
  font-weight: bold;
}

.risk-table {
  margin-top: 1.2rem;
  border-radius: 10px;
  overflow: hidden;
  background: rgba(0,0,0,0.15);
}

.risk-table-row {
  display: grid;
  grid-template-columns: 2fr 1fr;
  padding: 0.7rem 1rem;
  align-items: center;
  border-bottom: 1px solid rgba(255,255,255,0.08);
}

.risk-table-row:last-child { border-bottom: none; }

.risk-table-head {
  font-weight: bold;
  color: rgba(255,255,255,0.7);
  font-size: 0.85rem;
  text-transform: uppercase;
  background: rgba(0,0,0,0.15);
}

/* ---------- Gates ---------- */
.gates-table {
  display: flex;
  flex-direction: column;
  gap: 0.7rem;
  margin-top: 1.2rem;
}

.gate-row {
  display: grid;
  grid-template-columns: 1.3fr 0.8fr 1.6fr;
  gap: 1rem;
  align-items: center;
  padding: 0.9rem 1.1rem;
  border-radius: 10px;
  background: rgba(0,0,0,0.15);
  border-left: 4px solid transparent;
  font-size: 0.9rem;
}

.gate-veto { border-left-color: #e05a4e; }
.gate-warning { border-left-color: #ffb84d; }
.gate-ok { border-left-color: #3fbf8f; }

.gate-verdict { font-weight: bold; }
.gate-reason { color: rgba(255,255,255,0.75); font-size: 0.85rem; }

/* ---------- Annexes ---------- */
.annex {
  opacity: 0.9;
}

.annex h3 {
  font-size: 1rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: rgba(255,255,255,0.6);
}

.annex-grid {
  display: flex;
  gap: 1.5rem;
  flex-wrap: wrap;
  margin-top: 1rem;
}

.annex-block {
  flex: 1;
  min-width: 260px;
  background: rgba(0,0,0,0.12);
  border-radius: 10px;
  padding: 1rem 1.2rem;
}

.annex-block h4 {
  margin-top: 0;
  font-size: 0.9rem;
}

.annex-list {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
  font-size: 0.85rem;
}

.score-badge {
  display: inline-flex;
  width: 22px;
  height: 22px;
  border-radius: 50%;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
  font-weight: bold;
  color: #194a3e;
  margin-right: 0.4rem;
}

.score-5 { background: #3fbf8f; }
.score-4 { background: #74d3a0; }
.score-3 { background: #ffd27a; }
.score-2 { background: #ffb37a; }
.score-1 { background: #e05a4e; color: white; }

.threshold-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.82rem;
}

.threshold-table th {
  text-align: left;
  padding: 0.4rem 0.5rem;
  color: rgba(255,255,255,0.6);
  border-bottom: 1px solid rgba(255,255,255,0.2);
  font-weight: normal;
  text-transform: uppercase;
  font-size: 0.7rem;
}

.threshold-table td {
  padding: 0.5rem;
  border-bottom: 1px solid rgba(255,255,255,0.08);
}

@media (max-width: 640px) {
  .gate-row {
    grid-template-columns: 1fr;
  }
  .risk-table-row {
    grid-template-columns: 1fr;
    gap: 0.3rem;
  }
}
</style>