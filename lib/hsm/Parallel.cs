using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hsm {
	public class Parallel : State, INestedState {
		public List<StateMachine> submachines = new List<StateMachine>();
		
		public Parallel(string theId, List<StateMachine> theSubmachines) : base (theId) {
			submachines = theSubmachines;
		}

		public Parallel(string theId, params StateMachine[] theSubmachines): base (theId) {
			submachines.AddRange(theSubmachines);
		}

		public bool _handle(string evt, Dictionary<string, object> data) {
			bool handled = false;

			foreach(StateMachine submachine in submachines) {
				if (submachine._handle(evt, data)) {
					handled = true;
				}
			}
			return handled;
		}

		public Parallel addStateMachine(StateMachine theStateMachine) {
			submachines.Add(theStateMachine);
			return this;
		}

		public override void _enter(State sourceState, State targetstate, Dictionary<string, object> data) {
			base._enter(sourceState, targetstate, data);
			foreach(StateMachine submachine in submachines) {
				submachine.setup();
			}
		}
		
		public override void _exit(State nextState) {
			base._exit(nextState);
			foreach(StateMachine submachine in submachines) {
				submachine.tearDown(null);
			}
		}
	}
}