<!---![Logo of the project](https://raw.githubusercontent.com/jehna/readme-best-practices/master/sample-logo.png)--->

# Unity ABM Framework (ABMU)
> An Agent-Based Modelling (ABM) Framework for Unity3D

The Agent-Based Modelling Framework for Unity3D (**ABMU** for short) is a set of tools that allow the development of Agent-Based Models (ABM) in the Unity3D platform.

ABMU allows modellers to take advantage of Unity's built-in methods and classes to create complex agent behaviours in 3D, including Physics, Navigation, Animation, etc, all written in native `C#`.

Developers familiar with Unity3D should find the ABMU framework quite familiar to work with, as it provdes simple hooks for converting native Unity methods into ABMU behaviours.

ABMU is extendable, and can be coupled with other Unity libraries and assets. 

Behind-the-scenes, ABMU implements a scheduling system to handle the execution of agent behaviours at the correct time, simplifying the timestep implementation process and allowing developers to focus on the agent and model behaviour.

## Installation / Getting started

### From Github

Clone this repository and open in the Unity Editor.

### From the Unity Asset Store

Download the asset from the [Unity Asset Store](linkToAsset.com)

### Unity Package

Download the [unity package](linkToUnityPackage) and import into an existing Unity project.

### Initial Configuration

All classes in ABMU are contained within the `ABM` namespace, so scripts implementing ABMU functionality should import the `ABM` libraries:

```
using ABM;
using ABM.Core;
```

## Usage

### Overview

ABMU implements agent behaviour through the concept of the `Stepper` object. A `Stepper` is a single self-contained method that is defined and executed by an agent. When a `Stepper` is created, it is automatically added to the `Scheduler` queue to be executed at subsequent updates.

The overview of a model implemented in ABMU is the following:
1. `Agents` define their own behaviours as `Steppers`, and register them to the `Scheduler`.
2. The `Controller` handles top-level model elements, such as adding `Agents` to the simulation, keeping track of simulation parameters, etc.
3. On each frame, the `Scheduler` loops through all `Steppers` registered to execute at that frame, and calls the relevant method on each agent.

A simulation in ABMU is contained within a Unity scene, and requires 3 elements:
1. A **Controller** object
2. An **Agent** object
3. A **Scheduler** object

ABMU provides two abstract classes for implementing the first two, the `AbstractController` and the `AbstractAgent` class. The `Scheduler` object is created automatically by the controller when the simulation is started.

### Creating the simulation core elements

When creating a controller for a simulation, the simulation controller should inherit from the `AbstractController` class:

```
using UnityEngine;
using ABM.Core;

public class SimpleController : AbstractController
{
    // Simulation controller code goes here
}
```
Similarly, a simulation agent should inherit from the `AbstractAgent` class:

```
using UnityEngine;
using ABM.Core;

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
Agent behaviours should be defined as methods within the agent class, and can then be added to the scheduler queue for execution as **Steppers**, using the `CreateStepper(MethodName)` method. The following code defines an agent behaviour (the `Move()` method), creates a stepper from that behaviour, and registers it to the scheduler.

```
using UnityEngine;
using ABM.Core;

public class SimpleAgent : AbstractAgent
{
    public override void Init(){
        base.Init();
        CreateStepper(Move);
    }

    void Move(){
        this.transform.position += Random.insideUnitSphere;
    }
}
```
Once a stepper has been created and registered, it will be automatically executed every frame, until it is deregistered. Steppers can be removed from the scheduler queue using the `DestroyStepper(Stepper s)` function.

### Instantiating agents and running a simulation

Once an agent class has been created, it should be added to a GameObject in the Unity Editor and (preferably) converted into a prefab, in order to add a reference to it in the controller. Agents should be added to the simulation via the controller. The following code defines a reference to the agent GameObject in the controller, and once the scene is started, adds 100 agents in the scene, and starts execution of the simulation.

```
using UnityEngine;
using ABM.Core;

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
<!-- 
### Building

If your project needs some additional steps for the developer to build the
project after some code changes, state them here:

```shell
./configure
make
make install
```

Here again you should state what actually happens when the code above gets
executed.

### Deploying / Publishing

In case there's some step you have to take that publishes this project to a
server, this is the right time to state it.

```shell
packagemanager deploy awesome-project -s server.com -u username -p password
```

And again you'd need to tell what the previous code actually does.

## Features

What's all the bells and whistles this project can perform?
* What's the main functionality
* You can also do another thing
* If you get really randy, you can even do this

## Configuration

Here you should write what are all of the configurations a user can enter when
using the project.

#### Argument 1
Type: `String`  
Default: `'default value'`

State what an argument does and how you can use it. If needed, you can provide
an example below.

Example:
```bash
awesome-project "Some other value"  # Prints "You're nailing this readme!"
```

#### Argument 2
Type: `Number|Boolean`  
Default: 100

Copy-paste as many of these as you need.

## Contributing

When you publish something open source, one of the greatest motivations is that
anyone can just jump in and start contributing to your project.

These paragraphs are meant to welcome those kind souls to feel that they are
needed. You should state something like:

"If you'd like to contribute, please fork the repository and use a feature
branch. Pull requests are warmly welcome."

If there's anything else the developer needs to know (e.g. the code style
guide), you should link it here. If there's a lot of things to take into
consideration, it is common to separate this section to its own file called
`CONTRIBUTING.md` (or similar). If so, you should say that it exists here.

## Links

Even though this information can be found inside the project on machine-readable
format like in a .json file, it's good to include a summary of most useful
links to humans using your project. You can include links like:

- Project homepage: https://your.github.com/awesome-project/
- Repository: https://github.com/your/awesome-project/
- Issue tracker: https://github.com/your/awesome-project/issues
  - In case of sensitive bugs like security vulnerabilities, please contact
    my@email.com directly instead of using issue tracker. We value your effort
    to improve the security and privacy of this project!
- Related projects:
  - Your other project: https://github.com/your/other-project/
  - Someone else's project: https://github.com/someones/awesome-project/ -->


## Licensing


The code in this project is licensed under [MIT license](../blob/master/LICENSE).