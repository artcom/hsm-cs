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

		private new void _exit(State nextState) {
			base._exit(nextState);
			submachine.tearDown(null);
		}
	}

}