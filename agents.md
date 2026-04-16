# AGENTS.md

## Language Policy

- All comments must be written in Japanese
- All identifiers (class, method, variable names) must be written in English
- Explanations should be concise and in Japanese

---

## Project: MotifWeaver

## Purpose
This project implements a tiling pattern editor using graph-based topology.
The system must remain framework-agnostic and support both WPF and Blazor.

---

## Architecture Rules

1. Separation of concerns is mandatory:
   - Topology: graph structure (Vertex, Edge, Face)
   - Geometry: coordinate transformation
   - Rendering: drawing abstraction

2. Topology must:
   - Use integer coordinates only
   - Guarantee uniqueness of Vertex and Edge
   - Maintain bidirectional relationships

3. Geometry must:
   - Convert topology coordinates to screen coordinates
   - Be the only place where floating-point math is used

4. Renderer must:
   - Only consume Vector2 coordinates
   - Never depend on Topology classes
   - Be stateless except for frame buffering

---

## Coding Rules

- Language: C#
- Use explicit types (avoid var unless obvious)
- Use immutable structs where appropriate
- Use IReadOnlyList for external exposure
- Avoid unnecessary allocations in loops

---

## Naming Conventions

- PascalCase for types and methods
- camelCase for local variables
- Interfaces start with "I"
- Private fields start with "_"

---

## File Organization

/src
  /Topology
  /Geometry
  /Rendering
  /Viewer
  /Editor
  /App

Each class must be placed in its corresponding directory.

---

## Implementation Guidelines

- Always implement smallest working unit first
- Do not implement UI frameworks (WPF/Blazor) unless explicitly requested
- Prefer clarity over cleverness
- Avoid hidden side effects

---

## Testing Guidelines

- Validate topology consistency:
  - No duplicated vertices
  - Edge sharing correctness
- Validate geometry output range
- Ensure deterministic results

---

## Important Constraints

- Face.Vertices must always be ordered (clockwise)
- Edge must reference at most 2 Faces
- No floating-point operations in Topology

---

## Output Requirements

- Output complete files
- Include file path as comment at top
- Ensure code compiles

---

## Workflow

1. Understand task
2. Plan briefly
3. Implement
4. Self-review before output

---

## Forbidden

- Mixing geometry into topology
- Direct rendering inside topology
- Global mutable state

---

## Goal

Produce clean, modular, reusable core logic that can be shared across UI platforms.