<script setup lang="ts">
import { computed } from 'vue'
import type { CriterionKey, Ratings } from '@/types'
import { CRITERION_LABELS, CRITERION_ORDER } from '@/lib/labels'

const props = withDefaults(
  defineProps<{
    ratings: Ratings
    size?: number
    color?: string
  }>(),
  { size: 280, color: 'var(--color-forest-500)' },
)

const cx = computed(() => props.size / 2)
const cy = computed(() => props.size / 2)
const maxR = computed(() => props.size / 2 - 44)

// Axes start at the top and go clockwise.
function angle(i: number): number {
  return (Math.PI * 2 * i) / CRITERION_ORDER.length - Math.PI / 2
}

function point(value: number, i: number): [number, number] {
  const r = (value / 5) * maxR.value
  return [cx.value + r * Math.cos(angle(i)), cy.value + r * Math.sin(angle(i))]
}

const rings = [1, 2, 3, 4, 5]

function ringPolygon(level: number): string {
  return CRITERION_ORDER.map((_, i) => point(level, i).join(',')).join(' ')
}

const dataPolygon = computed(() =>
  CRITERION_ORDER.map((key, i) => point(props.ratings[key], i).join(',')).join(' '),
)

const axes = computed(() =>
  CRITERION_ORDER.map((key: CriterionKey, i) => {
    const [x, y] = point(5, i)
    const [lx, ly] = point(5.9, i)
    return { key, label: CRITERION_LABELS[key], x, y, lx, ly, value: props.ratings[key] }
  }),
)
</script>

<template>
  <svg :width="size" :height="size" :viewBox="`0 0 ${size} ${size}`" class="overflow-visible">
    <!-- Rings -->
    <polygon
      v-for="r in rings"
      :key="r"
      :points="ringPolygon(r)"
      fill="none"
      stroke="var(--color-forest-200)"
      stroke-width="1"
    />
    <!-- Axes -->
    <line
      v-for="ax in axes"
      :key="`ax-${ax.key}`"
      :x1="cx"
      :y1="cy"
      :x2="ax.x"
      :y2="ax.y"
      stroke="var(--color-forest-200)"
      stroke-width="1"
    />
    <!-- Data polygon -->
    <polygon
      :points="dataPolygon"
      :fill="color"
      fill-opacity="0.18"
      :stroke="color"
      stroke-width="2"
      stroke-linejoin="round"
    />
    <circle
      v-for="(ax, i) in axes"
      :key="`pt-${ax.key}`"
      :cx="point(ratings[ax.key], i)[0]"
      :cy="point(ratings[ax.key], i)[1]"
      r="3.5"
      :fill="color"
    />
    <!-- Labels -->
    <g v-for="ax in axes" :key="`lbl-${ax.key}`">
      <text
        :x="ax.lx"
        :y="ax.ly"
        text-anchor="middle"
        dominant-baseline="middle"
        class="fill-ink-700"
        style="font-size: 12px; font-weight: 600"
      >
        {{ ax.label }}
      </text>
      <text
        :x="ax.lx"
        :y="ax.ly + 14"
        text-anchor="middle"
        dominant-baseline="middle"
        :fill="color"
        style="font-size: 11px; font-weight: 700"
      >
        {{ ax.value }}/5
      </text>
    </g>
  </svg>
</template>
