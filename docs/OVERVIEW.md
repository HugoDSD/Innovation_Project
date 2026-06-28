# Project Overview

## The problem

Using generative AI in a project is never free. Every request consumes
electricity and water, costs money, and can carry legal or data-privacy risk.
Most teams adopt AI without ever measuring whether the value it brings actually
justifies those costs — they simply assume it does.

## What this project is

An **AI Impact Evaluator**: a web tool that helps a company decide whether
using AI for a given task is genuinely worth it. The user describes their
intended use of AI, and the tool returns a clear, data-backed verdict instead
of a gut feeling.

## Who it is for

- **Companies** (small businesses, startups, larger firms) weighing an AI
  feature or workflow.
- **IT and management teams** who need to justify an AI decision.
- **Sustainability / CSR roles** accountable for the environmental footprint of
  technology choices.

## How it works, from the user's point of view

1. **Describe the project.** The user explains what the AI will be used for —
   which tasks it replaces, roughly how much time it would save (across reports,
   images, and presentations), and how sensitive the data and legal context are.
2. **Choose the AI setup.** The user picks an AI model and a cloud provider, and
   gives an estimate of how much text (tokens) each request involves.
3. **Get the evaluation.** The tool calculates the real impact and shows a
   verdict with the numbers behind it.

## What it measures

For each scenario the tool estimates three kinds of impact:

- **Environmental** — energy used (kWh), carbon emitted (kg CO₂, based on the
  French electricity mix), and water consumed (litres).
- **Economic** — the cost of the AI usage, in dollars.
- **Social** — how much working time is saved, weighed against a risk score
  built from data-sensitivity and legal-risk.

It then delivers a verdict — broadly, **worth it** or **not worth it** — and
lets the user record their own opinion on the result (Useful / Average / Not
useful / Better without AI). Every evaluation is saved so a user can look back
over their history.

## Current status

This is a working prototype built as a student innovation project at EFREI
Paris. The core loop — sign in, describe a project, get a real impact
evaluation, rate it, and review past evaluations — is implemented end to end.

Some ideas from the original concept are intentionally left for later: simulating
an actual AI prompt inside the tool, and comparing several AI models side by side
to recommend the best fit. The database currently runs locally per developer;
hosting it in the cloud is a planned next step.

## Where to go next

- Setup and how to run it: [SETUP.md](SETUP.md)
- Technical details, API, and project structure: [../README.md](../README.md)
