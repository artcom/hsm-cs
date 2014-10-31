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

		public bool Handle(string evt, Dictionary<string, object> data) {
			bool handled = false;

			foreach(StateMachine submachine in submachines) {
				if (submachine.Handle(evt, data)) {
					handled = true;
				}
			}
			return handled;
		}

		public Parallel AddStateMachine(StateMachine theStateMachine) {
			submachines.Add(theStateMachine);
			return this;
		}

		public override void Enter(State sourceState, State targetstate, Dictionary<string, object> data) {
			base.Enter(sourceState, targetstate, data);
			foreach(StateMachine submachine in submachines) {
				submachine.setup();
			}
		}
		
		public override void Exit(State nextState) {
			base.Exit(nextState);
			foreach(StateMachine submachine in submachines) {
				submachine.tearDown(null);
			}
		}
	}
}
