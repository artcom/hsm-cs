using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsm {

	public class Transition {

		private State sourceState;
		private State targetState;
		private TransitionKind kind;
		private Action<Dictionary<string, object>> action;

		public Transition (State sourceState, Handler handler) {
			this.sourceState = sourceState;
			this.targetState = handler.targetState;
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
			if (targetState == null) {
				return false;
			}
			StateMachine lca = _findLeastCommonAncestor();
			lca.switchState(sourceState, targetState, action, data);
			return true;
		}

		private bool _performInternalTransition(Dictionary<string, object> data) {
			if (action != null) {
				action.Invoke(data);
			}
			return true;
		}

		private StateMachine _findLeastCommonAncestor() {
			List<StateMachine> sourcePath = sourceState.owner.getPath();
			List<StateMachine> targetPath = targetState.owner.getPath();

			StateMachine lca = null;
			for (var i = sourcePath.Count-1; i >= 0; i--) {
				StateMachine stateMachine = sourcePath[i];
				if (targetPath.Contains(stateMachine)) {
					lca = stateMachine;
					break;
				}
			}
			if (kind == TransitionKind.Local) {
				if (sourceState.hasAncestor(targetState) || targetState.hasAncestor(sourceState)) {
					Sub containingSubState = lca.currentState as Sub;
					lca = containingSubState._submachine;
				}
			}
			return lca;
		}
	}

}
