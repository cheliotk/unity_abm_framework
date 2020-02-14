![Banner](https://github.com/cheliotk/unity_abm_framework/raw/master/abmu_banner.png)

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

```
using ABMU;
using ABMU.Core;
```

## Usage

### Overview

ABMU implements agent behaviour through the concept of the `Stepper` object. A `Stepper` is a single self-contained method that is defined and executed by an agent. When a `Stepper` is created, it is automatically added to the execution queue to be executed at subsequent updates.

The overview of a model implemented in ABMU is the following:
1. `Agents` define their own behaviours as `Steppers`, and register them with the `Controller`.
2. The `Controller` passes `Steppers` to the `Scheduler` queue for execution, and handles general top-level model elements, such as adding `Agents` to the simulation, keeping track of simulation parameters, etc.
3. On each frame, the `Scheduler` loops through all `Steppers` registered to execute at that frame, and calls the relevant method on each agent.

A simulation in ABMU is contained within a Unity scene, and requires 3 elements:
1. A **Controller** object
2. An **Agent** prefab object
3. A **Scheduler** object

ABMU provides two abstract classes for implementing the first two, the `AbstractController` and the `AbstractAgent` class. The `Scheduler` object is created automatically by the controller when the simulation is started.

### Creating the simulation core elements

When creating a controller for a simulation, the simulation controller should inherit from the `AbstractController` class:

```
using UnityEngine;
using ABMU.Core;

public class SimpleController : AbstractController
{
    // Simulation controller code goes here
}
```
Similarly, a simulation agent should inherit from the `AbstractAgent` class:

```
using UnityEngine;
using ABMU.Core;

public class SimpleAgent : AbstractAgent
{
    // Simulation Agent code goes here
}
```
The controller and agent abstract classes define methods for initializing the classes, which can (and most probably should) be overriden to implement functionality specific to the simulation:

```
public class SimpleController : AbstractController
{
    public override void Init(){
        base.Init();
        // Simulation-specific code goes here
    }
}

public class SimpleAgent : AbstractAgent
{
    public override void Init(){
        base.Init();
        // Simulation-specific code goes here
    }
}
```
Note that when overriding an abstract method, it is important to call the method on the base class as well, before implementing any additional functionality.

### Defining Behaviours
Agent behaviours should be defined as methods within the agent class, and can then be added to the scheduler queue for execution as **Steppers**, using the `CreateStepper(MethodName)` method. The following code defines an agent behaviour (the `Move()` method), creates a stepper from that behaviour, and registers it to the scheduler (Stepper registration to the scheduler happens automatically when `CreateStepper()` is called).

```
using UnityEngine;
using ABMU.Core;

public class SimpleAgent : AbstractAgent
{
    public override void Init(){
        base.Init();
        CreateStepper(Move);    // Converting the behaviour into a Stepper. Registration to the Scheduler happens automatically when a new Stepper is created 
    }

    void Move(){    // Definition of the behaviour method
        this.transform.position += Random.insideUnitSphere;
    }
}
```
Once a stepper has been created and registered, it will be automatically executed every frame, until it is deregistered. Steppers can be removed from the scheduler queue using the `DestroyStepper(Stepper s)` function.

### Instantiating agents

Once an agent class has been created, it should be added to a GameObject in the Unity Editor and converted into a prefab, to act as an agent avatar.

Agents should be added to the simulation via the controller. The following code defines a reference to the agent GameObject in the controller (the created agent prefab should be added to the `agentPrefab` reference field manually from within the Unity Editor), and once the scene is started, adds 100 agents in the scene, and starts execution of the simulation.

```
using UnityEngine;
using ABMU.Core;

public class SimpleController : AbstractController
{
    public GameObject agentPrefab;
    public int numberOfAgents = 100;

    public override void Init(){
        base.Init();

        for (int i = 0; i < numberOfAgents; i++)
        {
            GameObject a = Instantiate(agentPrefab);
            a.GetComponent<SimpleAgent>().Init();
        }
    }
}
```
### Running a simulation

**ABMU** automatically controls and advances the simulation, so no additional code is needed from the user. The `AbstractController` class has a `Step()` function that advances the simulation by one tick, and is executed during `LateUpdate()`. The controller's `Step()` function calls the `Tick()` method on the `Scheduler`, which keeps a record of all steppers registered so far and executes them in order.

As initialization and updating is handled by **ABMU**, users **should not** implement any of Unity's event functions, i.e. `Start()`, `Update()`, `FixedUpdate()`, `LateUpdate()`, etc, for agent behaviour and model-related updates, as they may interfere with **ABMU**'s execution order. Unity's event functions can be safely used for non-model related behaviours, such as rendering (for an example of this see the [3DNavigation example](Assets/abm_framework/Examples/5.3DNavigation/), where rooms implement the `Start()` and `LateUpdate()` methods to detect agents and render accordingly).

## Examples

More examples are provided in the project, in the [Examples](Assets/abm_framework/Examples) folder. Six example models are included:

- The simple random movement model used in this document.
- A **Neighbour Detection** model, where randomly wandering agents alter their speed and size based on the number of other agents in their neighbourhood.
- An implementation of Raynolds' (1987) [3D Boids model](http://www.red3d.com/cwr/boids/) ([ref](https://doi.org/10.1145/37401.37406))
- Schelling's (1971) [Segregation model](http://nifty.stanford.edu/2014/mccown-schelling-model-segregation/) ([ref](https://doi.org/10.1080/0022250X.1971.9989794)), adapted to run in 3D space
- An implementation of Epstein & Axtell's (1996) [Sugarscape model](https://en.wikipedia.org/wiki/Sugarscape) ([ref](https://doi.org/10.7551/mitpress/3374.003.0004))
- A **3D Navigation** model, where agents wander randomly within an indoor multi-storey environment using Unity's NavMesh and pathfinding.

To run the examples, load the respective scene in Unity and press Play.

## Documentation

More detailed documentation on **ABMU** methods and classes can be found in the [Wiki](../../wiki).

## Other

**ABMU** has been developed and tested with Unity version 2018.3.0f2. It should probably work with all newer versions of Unity as well (and probably older versions too), but no guarantees. 

## Licensing

The code in this project is licensed under [MIT license](../blob/master/LICENSE).
