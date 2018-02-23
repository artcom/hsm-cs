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

		public bool performTransition(Dictionary<string, object> data) {
			if (kind == TransitionKind.Internal) {
				return _performInternalTransition(data);
			} else {
				return _performExternalTransition(data);
			}
		}

		private bool _performExternalTransition(Dictionary<string, object> data) {
			if (target == null) {
				return false;
			}
			StateMachine lca = _findLeastCommonAncestor();
			lca._switchState(source, target, action, data);
			return true;
		}

		private bool _performInternalTransition(Dictionary<string, object> data) {
			if (action != null) {
				action.Invoke(data);
			}
			return true;
		}

		private StateMachine _findLeastCommonAncestor() {
			List<StateMachine> sourcePath = source.owner.getPath();
			List<StateMachine> targetPath = target.owner.getPath();
			foreach(StateMachine stateMachine in sourcePath) {
				if (targetPath.Contains(stateMachine)) {
					return stateMachine;
				}
			}
			return null;
		}
	}

}
