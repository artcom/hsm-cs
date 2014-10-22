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

		public new void _enter(State sourceState, State targetstate, Dictionary<string, object> data) {
			//Debug.Log("Sub._enter -- targetState: " + targetstate + " this: " + this.id); 
			base._enter(sourceState, targetstate, data);
			submachine.setup();
		}

		public new void _exit(State nextState) {
			base._exit(nextState);
			submachine.tearDown(null);
		}
	}

}