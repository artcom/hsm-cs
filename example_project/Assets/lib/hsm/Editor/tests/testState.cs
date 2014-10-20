using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using Hsm;

namespace UnitTesting {

	[TestFixture]
	internal class StateTests : AssertionHelper {

		[Test]
		public void InstantiationTest() {
			State state = new State("myId");
			Expect(state.id, EqualTo("myId"));
			Expect(state, Is.InstanceOf<State>());
		}

		[Test]
		public void AddHandlerTest() {
			State state = new State("On");
			state.addHandler("foo", (data) => {
				return "Off";
			});
			Expect(state.handlers.Count, EqualTo(1));
			Expect(state.handlers.ContainsKey("foo"), Is.True);
			Expect(state.handlers["foo"], Is.InstanceOf(typeof(Func<Dictionary<string, object>, String>)));
			Expect(state.handlers["foo"].Invoke(new Dictionary<string, object>()), Is.EqualTo("Off"));
		}

		[Test]
		public void IsChainable() {
			State state = new State("OnHold");
			Expect(state.addHandler("foo", (data) => { return null; }), Is.EqualTo(state));
			Expect(state.OnEnter((s, t) => {}), Is.EqualTo(state));
			Expect(state.OnExit((n) => {}), Is.EqualTo(state));
		}

		[Test]
		public void EnterCallback() {
			State state = new State("Manyfold");
			bool called = false;
			state.OnEnter((s, t) => { called = true; });
			state._enter(state, state, new Dictionary<string, object>());
			Expect(called, Is.True);
		}

		[Test]
		public void ExitCallback() {
			State state = new State("Manyfold");
			bool called = false;
			state.OnExit((n) => { called = true; });
			state._exit(state);
			Expect(called, Is.True);
		}
	}

}