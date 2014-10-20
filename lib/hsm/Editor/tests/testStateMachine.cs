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
			StateMachine statemachine = new StateMachine(new List<State>{new State("foo"), new State("bar")});
			Expect(statemachine, Is.InstanceOf<StateMachine>());
			Expect(statemachine.states.Count, Is.EqualTo(2));
		}
	}
}