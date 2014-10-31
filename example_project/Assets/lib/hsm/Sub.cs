using System.Collections.Generic;

namespace Hsm {

	public class Sub : State, INestedState {

		public StateMachine submachine;

		public Sub(string theId, StateMachine theSubmachine) : base (theId) {
			submachine = theSubmachine;
		}

		public bool Handle(string evt, Dictionary<string, object> data) {
      return submachine.Handle(evt, data);
		}

		public override void Enter(State sourceState, State targetstate, Dictionary<string, object> data) {
			base.Enter(sourceState, targetstate, data);
			submachine.setup();
		}

		public override void Exit(State nextState) {
			base.Exit(nextState);
			submachine.tearDown(null);
		}
	}

}
