# HSM for Unity3d/C#

## General

A hierarchical statemachine for Unity implemented in C#

The Hsm allows programmatic declaration of states and event handlers.

The Hsm follows the [UML-Statemachine](http://en.wikipedia.org/wiki/UML_state_machine) specs.

The following state machine is used [in the tests](lib/hsm/Editor/tests):

![image](doc/exports/advanced.png)

# States and State Machines

States are specific by creating Hsm.State instances. They are then composed to a state machine by passing them to the Hsm.StateMachine constructor.

![image](doc/exports/simple.png)

```cs
using Hsm;

State a1 = new State("a1");
State a2 = new State("a2");
State a3 = new State("a3");

StateMachine a = new StateMachine(a1, a2, a3);
```

Alternatively the state machine can also be constructed in several other way. For more details see the [Instantiate*-tests](lib/hsm/Editor/tests/testStateMachine.cs):

```cs
using Hsm;

// one big constuctor
StateMachine a = new StateMachine(
    new State("a1"),
    new State("a2"),
    new State("a3")
);

// or attaching the states after state machine construction
StateMachine a = new StateMachine();
a.AddState(new State("a1"));
a.AddState(new State("a2"));
a.AddState(new State("a3"));

// or by using *chaining*, which in C# terminology is often called *Fluent Interface*
StateMachine a = new StateMachine()
.AddState(new State("a1"))
.AddState(new State("a2"))
.AddState(new State("a3"));
```

By Convention, the first state passed is the initial state. The state machine is then initialized by

```cs
a.setup();
```

This starts the state machine and activates the initial state, calling its enter handler (see below). The state machine is now ready to handle events.

# Entry and Exit handlers

Each state can have an `exitAction` and an `enterAction`. They will be invoked when a state is entered or exited.

Example of specifying enter/exit actions (using chaining):

```cs
using Hsm;

State a = new State("a")
.OnExit((targetState) => {
    // your code here
})
.OnEnter((sourceState, targetState) => {
    // your code here
});
```

# Actions and State Transitions

Each state has a map of event handlers. These handlers will be called when the state receives the respective event. Event handlers are added to the handlers list of each state (based on previous example state machine `a`):

![image](doc/exports/simpleWithTransition.png)

```cs
a3.AddHandler("T3", a2);
a3.AddHandler("T3", a2, data => {
    // ...
});
```

# Internal, External

Internal and External can be selected by using the `Transition` enum:

```cs
a3.AddHandler("T3", a2); // external by default
a3.AddHandler("T3", a2, Transition.External);
a3.AddHandler("TI", a3, Transition.Internal, data => {
    // ...
});
```

External transitions are used by default. When using internal transitions target state must be the similar to the handling state.

# Local Transitions (not implemented)

Local Transitions are currently not implemented. For inspiration on what this could look like see:

* [Local vs. external transitions](http://en.wikipedia.org/wiki/UML_state_machine#Local_versus_external_transitions)

# Guards (not implemented)

Guards are currently not implemented. For inspiration on what this could look like see: https://github.com/Mask/hsm-js

# Sub-StateMachines (nested)

StateMachines can be nested in other state machines by using the `Hsm.Sub` adapter class.

All events are propagated into the sub-state machines, and the sub state machine is initialed and torn down on enter/exit of its containing state.

![image](doc/exports/simpleSub.png)

```cs
using Hsm;

Sub a = new Sub("a", new StateMachine(
    new State(a1),
    new State(a2),
    new State(a3)
));
```

For more details on how to construct a Hsm.Sub consult the [tests](lib/hsm/Editor/tests/testSubmachine.cs).


# Parallel State-Machines (orthogonal regions)

Parallel state machines are constructed with the Hsm.Parallel adapter class.

All events are propagated to all contained orthogonal state machines contained in the Hsm.Parallel. An events is treated as handled as soon as one of those state machines handles an event (returns `true`).

![image](doc/exports/simpleParallel.png)

```cs
using Hsm;

Parallel c = new Parallel("c",
    new StateMachine(
        new State("c11"),
        new State("c12")
    ),
    new StateMachine(
        new State("c21"),
        new State("c22")
    )
);
```

For more details on how to construct a Hsm.Parallel consult the [tests](lib/hsm/Editor/tests/testParallel.cs).

# Development Setup

In order to use the `hsm` as bower package dependency in your unity project (`$ bower install`) create or edit the following files in your projects folder:

* `.bowerrc` file specifying the repository url and target folder:

```json
{
    "registry": "<repository_url>",
	"directory": "Assets/bower_packages" 
}
```

* `bower.json` file specifying the dependency name and wanted version:

```json
{
    "name" : "<some application name>",
    ...
    "dependencies": {
        "hsm" : "latest"
    }
}
```

## Generating API documentation

* *Requires* `doxygen` & `graphviz` (e.g. `brew install doxygen graphviz` on OS X)

Execute

```sh
doxygen doc/doxygen/doxygenConfig
```

to generate html documentation. It will be put at `doc/generated/html`
