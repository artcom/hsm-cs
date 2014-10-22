using System.Collections.Generic;

namespace Hsm {

	public class Sub : State {

		public StateMachine submachine;

		public Sub(string theId, StateMachine theSubmachine) : base (theId) {
			submachine = theSubmachine;
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