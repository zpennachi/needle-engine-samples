# Switching lightmaps

Both real-time and static lighting has its advantages. The biggest limitation of static lighting is when we want it to react to the environment. 

One technique to allow for simple changes is to bake multiple lightmaps and switch between them. 

## Components

### Lightmap Baker
An editor-only script where you define your lightmap variants. Every variant has a set of objects and a set of emissive materials. When a variant is selected, the assigned objects will be activated, and all objects from the other variants will be deactivated. Similarly, the material's emission will be turned on and off.

This component is the only way how to bake multiple lightmaps all at once. If you bake lighting through the Lighting window, those resulting lightmaps are outside of the lightmap switching system.

If you open the sample for the first time, navigate to this component and hit Bake All. (Located on the `Lightmaps` game object)

### Lightmap Configurations
Lightmap Baker populates this component with the resulting lightmap variants, and it supplies the runtime API to control lightmap switching.

The default behaviour is that it cycles through all the lightmaps. You can change the cycle style by enabling ping-pong, so the selection looks like this: `1 2 3 2 1 2 3` instead of `1 2 3 1 2 3 `.

You can turn off the automatic switching by disabling the `autoSwitch`. 

To switch to a specific lightmap, use the `setLightmap(index)`, and for manual cycling, use the `selectNext()`. You can create a simple UI menu and call these methods directly from the buttons.

## Use cases

### Scene light
Differentiate between day & night with atmospheric lighting.

### Glowing object
One example would be a lamp and turning it on & off. Or turning a car's lights on & off and accompanied by an engine start sound and flares. 

## Limitations
- These components nor the commonly used shaders support custom blending between the lightmaps.
- Baked lighting is only useful for static scenes where the objects affected are not moving.
- You can't bake lightmaps during runtime


### Attribution
```
Author: ChristyHsu
Source: https://sketchfab.com/3d-models/vr-apartment-loft-interior-baked-54bc929a26094b6cb36f977903fbcd97
License: CC-BY-4.0 (https://creativecommons.org/licenses/by/4.0/)
```