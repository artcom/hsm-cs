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
			state.AddHandler("foo", off);
			Expect(state.handlers.Count, EqualTo(1));
			Expect(state.handlers.ContainsKey("foo"), Is.True);

			List<Handler> handlers = state.handlers["foo"];
			Expect(handlers, Is.InstanceOf(typeof(List<Handler>)));

			Handler handler = handlers[0];
			Expect(handler.targetState, Is.InstanceOf(typeof(State)));
			Expect(handler.action, Is.Null);
		}

		[Test]
		public void IsChainable() {
			var state = new State("OnHold");
			Expect(state.AddHandler("foo", null), Is.EqualTo(state));
			Expect(state.OnEnter(() => {}), Is.EqualTo(state));
			Expect(state.OnExit(() => {}), Is.EqualTo(state));
		}

		[Test]
		public void EnterCallback() {
			var state = new State("Manyfold");
			bool called = false;
			state.OnEnter((data) => { called = true; });
			Expect(state.enterAction, Is.EqualTo(null));
			Expect(state.enterActionWithData, Is.Not.EqualTo(null));
			state.OnEnter(() => { called = true; });
			Expect(state.enterActionWithData, Is.EqualTo(null));
			Expect(state.enterAction, Is.Not.EqualTo(null));
			state.Enter(state, state, new Dictionary<string, object>{{"value", "crocodile"}});
			Expect(called, Is.True);
		}

		[Test]
		public void EnterCallbackWithData() {
			var state = new State("Manyfold");
			string called = "";
			state.OnEnter((s, t, d) => { called = "true"; });
			Expect(state.enterActionWithData, Is.EqualTo(null));
			Expect(state.enterAction, Is.Not.EqualTo(null));
			state.OnEnter(data => { called = (string)data["value"]; });
			Expect(state.enterAction, Is.EqualTo(null));
			Expect(state.enterActionWithData, Is.Not.EqualTo(null));
			state.Enter(state, state, new Dictionary<string, object>{{"value", "crocodile"}});
			Expect(called, Is.EqualTo("crocodile"));
		}

		[Test]
		public void ExitCallback() {
			var state = new State("Manyfold");
			bool called = false;
			state.OnExit((s, t, d) => { called = true; });
			state.Exit(state, state, null);
			Expect(called, Is.True);
		}
	}
}
