using System.Collections.Generic;

namespace Hsm {
	public class Parallel : State, INestedState {
		public List<StateMachine> _submachines = new List<StateMachine>();
		
		public Parallel(string theId, List<StateMachine> theSubmachines) : base (theId) {
			_submachines = theSubmachines;
			_setContainer();
		}

		public Parallel(string theId, params StateMachine[] theSubmachines): base (theId) {
			_submachines.AddRange(theSubmachines);
			_setContainer();
		}

		public Parallel AddStateMachine(StateMachine theStateMachine) {
			_submachines.Add(theStateMachine);
			_setContainer();
			return this;
		}

		private void _setContainer() {
			foreach (StateMachine stateMachine in _submachines) {
				stateMachine.container = this;
			}
		}

		public bool Handle(string evt, Dictionary<string, object> data) {
			bool handled = false;
			foreach(var submachine in _submachines) {
				if(submachine.Handle(evt, data)) {
					handled = true;
				}
			}
			return handled;
		}
		
		public override void Enter(State sourceState, State targetstate, Dictionary<string, object> data) {
			base.Enter(sourceState, targetstate, data);
			foreach(var submachine in _submachines) {
				if (targetstate.hasAncestorStateMachine(submachine)) {
					submachine.enterState(sourceState, targetstate, data);
				} else {
					submachine.setup(data);
				}
			}
		}
		
		public override void Exit(State sourceState, State targetstate, Dictionary<string, object> data) {
			foreach(var submachine in _submachines) {
				submachine.tearDown(data);
			}
			base.Exit(sourceState, targetstate, data);
		}

		public List<string> getActiveStateConfiguration() {
            List<string> states = new List<string>();
            foreach(var submachine in _submachines) {
                states.AddRange(submachine.getActiveStateConfiguration());
            }
            return states;
    	}
	}
}
