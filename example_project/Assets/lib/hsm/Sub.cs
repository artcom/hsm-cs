using System.Collections.Generic;

namespace Hsm {

	public class Sub : State, INestedState {

		public StateMachine submachine;

		public Sub(string theId, StateMachine theSubmachine) : base (theId) {
			submachine = theSubmachine;
		}

		public bool _handle(string evt, Dictionary<string, object> data) {
			return submachine._handle(evt, data);
		}

		public override void _enter(State sourceState, State targetstate, Dictionary<string, object> data) {
			base._enter(sourceState, targetstate, data);
			submachine.setup();
		}

		public override void _exit(State nextState) {
			base._exit(nextState);
			submachine.tearDown(null);
		}
	}

}