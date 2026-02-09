# Proportion Layout for Unity UI

A lightweight Unity UI layout plugin that sizes child elements by proportion. It provides a custom LayoutGroup and a companion element component to allocate space by weight, with optional spacing and reverse order.

## Features

- Proportional sizing on a horizontal or vertical axis
- Automatic fill on the secondary axis
- Per-element weight via a simple component
- Optional spacing between elements
- Optional reverse order

## Installation

1. Copy `ProportionLayoutGroup.cs` and `ProportionLayoutElement.cs` into your Unity project (any folder under `Assets`).
2. Ensure the object using the layout has a `RectTransform` (Unity UI).

## Usage

1. Create a parent UI GameObject with a `RectTransform`.
2. Add `ProportionLayoutGroup` to the parent.
3. Create child UI elements (each with `RectTransform`).
4. Add `ProportionLayoutElement` to each child that should participate in the proportional layout.
5. Set the `proportion` value on each child to define its share of the available space.
6. Configure `Direction`, `Spacing`, and `Reverse Order` on the parent as needed.

Only children with `ProportionLayoutElement` and active in hierarchy are included in the layout.

## Inspector Properties

**ProportionLayoutGroup**

- Direction: Horizontal (width by proportion) or Vertical (height by proportion)
- Spacing: pixel spacing between elements on the primary axis
- Reverse Order: when enabled, layout order is reversed relative to the hierarchy
- Padding: standard `LayoutGroup` padding on the parent

**ProportionLayoutElement**

- Proportion: non-negative weight on the primary axis

## Example

Three children with proportions 1, 2, and 3 in Horizontal mode:

- Total proportion = 1 + 2 + 3 = 6
- Child sizes = 1/6, 2/6, 3/6 of the available width
- Height of each child fills the parent height (minus padding)
