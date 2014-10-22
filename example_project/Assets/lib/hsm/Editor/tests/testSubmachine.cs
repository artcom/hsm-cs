using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using Hsm;

namespace UnitTesting {
	
	[TestFixture]
	internal class SubTests : AssertionHelper {

		[Test]
		public void Instantiation() {
			Sub sub = new Sub("sub", new StateMachine(new State("on"), new State("off")));
			Expect(sub.id, EqualTo("sub"));
			Expect(sub, Is.InstanceOf<State>());
			Expect(sub, Is.InstanceOf<Sub>());
		}

		[Test]
		public void SimpleSub() {
			Sub powered_on_sub = new Sub("powered_on", new StateMachine(
				new State("on").addHandler("off", (data) => { return "off"; }),
				new State("off").addHandler("on", (data) => { return "on"; })
			))
			.addHandler("power_off", (data) => { return "powered_off"; })
			.OnEnter((s, t) => {})
			.OnExit((n) => {});
			StateMachine sm = new StateMachine(
				new State("powered_off").addHandler("power_on", (data) => { return "powered_on"; }),
				powered_on_sub
			);
			sm.setup();
			Expect(sm.currentState.id, Is.EqualTo("powered_off"));
			sm.handleEvent("power_on");
			Expect(sm.currentState, Is.InstanceOf<Sub>());
			Expect(sm.currentState.id, Is.EqualTo("powered_on"));
			Sub sub = sm.currentState as Sub;

			Debug.Log(sub.submachine.currentState);
			Expect(sub.submachine.currentState, Is.Not.Null);

			Expect(sub.submachine.currentState.id, Is.EqualTo("foo"));
		}

	}
}