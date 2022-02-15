![Banner](https://github.com/cheliotk/unity_abm_framework/blob/main/abmu_banner.png)

# Unity ABM Framework (ABMU)
> An Agent-Based Modelling (ABM) Framework for Unity3D

The Agent-Based Modelling Framework for Unity3D (**ABMU** for short) is a set of tools that allow the development of [Agent-Based Models (ABM)](https://en.wikipedia.org/wiki/Agent-based_model) in the [Unity3D](https://unity.com/) platform.

ABMU allows modellers to take advantage of Unity's built-in methods and classes to create complex agent behaviours in 3D, including Physics, Navigation, Animation, etc, all written in native `C#`.

Developers familiar with Unity3D should find the ABMU framework quite familiar to work with, as it provides simple hooks for converting methods written in Unity into ABMU behaviours.

ABMU is extendable, and can be coupled with other Unity libraries and assets. 

Behind-the-scenes, ABMU implements a scheduling system to handle the execution of agent behaviours at the correct time, simplifying the timestep implementation process and allowing modellers to focus on the agent and model behaviour.

## Installation / Getting started

### From Github

Clone this repository and open in the Unity Editor.
### Unity Package

Download the [unity package](unityPackage/abm_framework.unitypackage) and import into an existing Unity project.

### Initial Configuration

All classes in ABMU are contained within the `ABMU` namespace, so scripts implementing ABMU functionality should import the `ABMU` libraries:

``` csharp
using ABMU;
using ABMU.Core;
```

## Examples

Example models demonstrating ABMU applicability are provided in the project, in the [Examples](Assets/abm_framework/Examples) folder. Six example models are included:

- The simple random movement model used in [the tutorial](../../wiki/Using-ABMU).
- A **Neighbour Detection** model, where randomly wandering agents alter their speed and size based on the number of other agents in their neighbourhood.
- An implementation of Reynolds' (1987) [3D Boids model](http://www.red3d.com/cwr/boids/) ([ref](https://doi.org/10.1145/37401.37406))
- Schelling's (1971) [Segregation model](http://nifty.stanford.edu/2014/mccown-schelling-model-segregation/) ([ref](https://doi.org/10.1080/0022250X.1971.9989794)), adapted to run in 3D space
- An implementation of Epstein & Axtell's (1996) [Sugarscape model](https://en.wikipedia.org/wiki/Sugarscape) ([ref](https://doi.org/10.7551/mitpress/3374.003.0004))
- A **3D Navigation** model, where agents wander randomly within an indoor multi-storey environment using Unity's NavMesh and pathfinding.

To run the examples, load the respective scene in Unity and press Play.

## Documentation

More detailed documentation on **ABMU** methods and classes as well as tutorials can be found in the [Wiki](../../wiki).

## Other

**ABMU** has been developed and tested with Unity version 2018.3.0f2. It should probably work with all newer versions of Unity as well (and probably older versions too), but no guarantees. 

## Licensing

The code in this project is licensed under [MIT license](../../blob/master/LICENSE).
