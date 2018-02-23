using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsm {

	public class Transition {

		private State source;
		private State target;
		private TransitionKind kind;
		private Action<Dictionary<string, object>> action;

		public Transition (State source, Handler handler) {
			this.source = source;
			this.target = handler.target;
			this.kind = handler.kind;
			this.action = handler.action;
		}

		public bool performTransition(Dictionary<string, object> data, StateMachine stateMachine) {
			if (kind == TransitionKind.Internal) {
				return _performInternalTransition(data);
			} else {
				return _performExternalTransition(data, stateMachine);
			}
		}

		private bool _performExternalTransition(Dictionary<string, object> data, StateMachine stateMachine) {
			if (target == null) {
				return false;
			}
			stateMachine._switchState(source, target, action, data);
			return true;
		}

		private bool _performInternalTransition(Dictionary<string, object> data) {
			if (action != null) {
				action.Invoke(data);
			}
			return true;
		}
	}

}
