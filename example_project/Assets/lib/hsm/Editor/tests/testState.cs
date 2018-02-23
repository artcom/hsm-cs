using System;
using System.Collections.Generic;
using NUnit.Framework;
using Hsm;

namespace UnitTesting {

	[TestFixture]
	class StateTests : AssertionHelper {

		[Test]
		public void InstantiationTest() {
			var state = new State("myId");
			Expect(state.id, EqualTo("myId"));
			Expect(state, Is.InstanceOf<State>());
		}

		[Test]
		public void AddHandlerTest() {
			var state = new State("On");
			var off = new State("Off");
			state.AddHandler("foo", data => off);
			Expect(state.handlers.Count, EqualTo(1));
			Expect(state.handlers.ContainsKey("foo"), Is.True);
			Expect(state.handlers["foo"], Is.InstanceOf(typeof(Func<Dictionary<string, object>, State>)));
			Expect(state.handlers["foo"].Invoke(new Dictionary<string, object>()), Is.EqualTo(off));
		}

		[Test]
		public void IsChainable() {
			var state = new State("OnHold");
			Expect(state.AddHandler("foo", data => null), Is.EqualTo(state));
			Expect(state.OnEnter((s, t) => {}), Is.EqualTo(state));
			Expect(state.OnExit(n => {}), Is.EqualTo(state));
		}

		[Test]
		public void EnterCallback() {
			var state = new State("Manyfold");
			bool called = false;
			state.OnEnter((s, t, d) => { called = true; });
			Expect(state.enterAction, Is.EqualTo(null));
			Expect(state.enterActionWithData, Is.Not.EqualTo(null));
			state.OnEnter((s, t) => { called = true; });
			Expect(state.enterActionWithData, Is.EqualTo(null));
			Expect(state.enterAction, Is.Not.EqualTo(null));
			state.Enter(state, state, new Dictionary<string, object>{{"value", "crocodile"}});
			Expect(called, Is.True);
		}

		[Test]
		public void EnterCallbackWithData() {
			var state = new State("Manyfold");
			string called = "";
			state.OnEnter((s, t) => { called = "true"; });
			Expect(state.enterActionWithData, Is.EqualTo(null));
			Expect(state.enterAction, Is.Not.EqualTo(null));
			state.OnEnter((s, t, d) => { called = (string)d["value"]; });
			Expect(state.enterAction, Is.EqualTo(null));
			Expect(state.enterActionWithData, Is.Not.EqualTo(null));
			state.Enter(state, state, new Dictionary<string, object>{{"value", "crocodile"}});
			Expect(called, Is.EqualTo("crocodile"));
		}

		[Test]
		public void ExitCallback() {
			var state = new State("Manyfold");
			bool called = false;
			state.OnExit(n => { called = true; });
			state.Exit(state);
			Expect(called, Is.True);
		}
	}
}
