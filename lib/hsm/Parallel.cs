using System.Collections.Generic;

namespace Hsm {
	public class Parallel : State, INestedState {
		public List<StateMachine> _submachines = new List<StateMachine>();
		
		public Parallel(string theId, List<StateMachine> theSubmachines) : base (theId) {
			_submachines = theSubmachines;
		}

		public Parallel(string theId, params StateMachine[] theSubmachines): base (theId) {
			_submachines.AddRange(theSubmachines);
		}

		public bool Handle(string evt, Dictionary<string, object> data) {
			bool handled = false;

			foreach(var submachine in _submachines) {
				handled |= submachine.Handle(evt, data);
				/*if (submachine.Handle(evt, data)) {
					handled = true;
				}*/
			}
			return handled;
		}

		public Parallel AddStateMachine(StateMachine theStateMachine) {
			_submachines.Add(theStateMachine);
			return this;
		}

		public override void Enter(State sourceState, State targetstate, Dictionary<string, object> data) {
			base.Enter(sourceState, targetstate, data);
			foreach(var submachine in _submachines) {
				submachine.setup();
			}
		}
		
		public override void Exit(State nextState) {
			base.Exit(nextState);
			foreach(var submachine in _submachines) {
				submachine.tearDown(null);
			}
		}
	}
}
