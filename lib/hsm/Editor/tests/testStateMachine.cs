using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using Hsm;

namespace UnitTesting {
	
	[TestFixture]
	internal class StateMachineTests : AssertionHelper {

		
		[Test]
		public void InstantiateEmpty() {
			StateMachine statemachine = new StateMachine();
			Expect(statemachine, Is.InstanceOf<StateMachine>());
			Expect(statemachine.states.Count, Is.EqualTo(0));
		}

		[Test]
		public void InstantiateWithStates() {
			// with List<State>
			StateMachine statemachine_from_list = new StateMachine(new List<State> {new State("foo"), new State("bar")});
			Expect(statemachine_from_list, Is.InstanceOf<StateMachine>());
			Expect(statemachine_from_list.states.Count, Is.EqualTo(2));
			statemachine_from_list.addState(new State("krokodil"));
			Expect(statemachine_from_list.states.Count, Is.EqualTo(3));
			Expect(statemachine_from_list.states[2].id, Is.EqualTo("krokodil"));

			// with State[]
			StateMachine statemachine_from_array = new StateMachine(new[] {new State("foo"), new State("bar")}); // or new State[]
			Expect(statemachine_from_array, Is.InstanceOf<StateMachine>());
			Expect(statemachine_from_array.states.Count, Is.EqualTo(2));
			statemachine_from_array.addState(new State("krokodil"));
			Expect(statemachine_from_array.states.Count, Is.EqualTo(3));
			Expect(statemachine_from_array.states[2].id, Is.EqualTo("krokodil"));
			
			// with multiple State arguments
			StateMachine statemachine_from_arguments = new StateMachine(new State("foo"), new State("bar"));
			Expect(statemachine_from_arguments, Is.InstanceOf<StateMachine>());
			Expect(statemachine_from_arguments.states.Count, Is.EqualTo(2));
			statemachine_from_arguments.addState(new State("krokodil"));
			Expect(statemachine_from_arguments.states.Count, Is.EqualTo(3));
			Expect(statemachine_from_arguments.states[2].id, Is.EqualTo("krokodil"));

			// Empty at construction time
			StateMachine intially_empty = new StateMachine();
			Debug.Log(intially_empty);
		}

		[Test]
		public void InitialState() {
			StateMachine sm = new StateMachine(new State("first"), new State("second"));
			sm.setup();
			Expect(sm.initialState.id, Is.EqualTo("first"));
		}
	}
}