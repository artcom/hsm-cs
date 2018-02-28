using System.Collections.Generic;

namespace Hsm {

	public class Sub : State, INestedState {

		public StateMachine _submachine;

		public Sub(string theId, StateMachine theSubmachine) : base (theId) {
			_submachine = theSubmachine;
			_submachine.container = this;
		}

		public bool Handle(string evt, Dictionary<string, object> data) {
			return _submachine.Handle(evt, data);
		}

		public override void Enter(State sourceState, State targetstate, Dictionary<string, object> data) {
			base.Enter(sourceState, targetstate, data);
			_submachine.enterState(sourceState, targetstate, data);
		}

		public override void Exit(State sourceState, State targetstate, Dictionary<string, object> data) {
			_submachine.tearDown(null);
			base.Exit(sourceState, targetstate, data);
		}

		public List<string> getActiveStateConfiguration() {
            return _submachine.getActiveStateConfiguration();
        }
	}

}
