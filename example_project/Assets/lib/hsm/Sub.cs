using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Hsm {

	public class Sub : State {

		public StateMachine submachine;

		public Sub(string theId, StateMachine theSubmachine) : base (theId) {
			submachine = theSubmachine;
		}

		private new void _enter(State sourceState, State targetstate, Dictionary<string, object> data) {
			base._enter(sourceState, targetstate, data);
			submachine._enterState(sourceState, targetstate, data);
		}

		// This should not be necessary! Ideally we should be able to achieve chaining in a simpler way
		public new Sub addHandler(string evt, Func<Dictionary<string, object>, String> handler) {
			base.addHandler(evt, handler);
			return this;
		}

		public new Sub OnExit(Action<State> action) {
			base.OnExit(action);
			return this;
		}

		public new Sub OnEnter(Action<State, State> action) {
			base.OnEnter(action);
			return this;
		}

		private new void _exit(State nextState) {
			base._exit(nextState);
			submachine.tearDown(null);
		}
	}

}