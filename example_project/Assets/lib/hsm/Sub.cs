using System.Collections.Generic;

namespace Hsm {

	public class Sub : State, INestedState {

		public StateMachine _submachine;

		public Sub(string theId, StateMachine theSubmachine) : base (theId) {
			_submachine = theSubmachine;
		}

		public bool Handle(string evt, Dictionary<string, object> data) {
      return _submachine.Handle(evt, data);
		}

		public override void Enter(State sourceState, State targetstate, Dictionary<string, object> data) {
			base.Enter(sourceState, targetstate, data);
			_submachine.setup();
		}

		public override void Exit(State nextState) {
			base.Exit(nextState);
			_submachine.tearDown(null);
		}
	}

}
