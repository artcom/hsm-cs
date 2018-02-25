using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsm {

	public class Transition {

		private State sourceState;
		private State targetState;
		private TransitionKind kind;
		private Action<Dictionary<string, object>> action;
		private Func<Dictionary<string, object>, bool> guard;

		public Transition (State sourceState, Handler handler) {
			this.sourceState = sourceState;
			this.targetState = handler.targetState;
			this.kind = handler.kind;
			this.action = handler.action;
			this.guard = handler.guard;
		}

		public bool performTransition(Dictionary<string, object> data) {
			if (!_canPerformTransition(data)) {
				return false;
			}
			if (kind == TransitionKind.Internal) {
				return _performInternalTransition(data);
			} else if (kind == TransitionKind.Local) {
				return _performLocalTransition(data);
			} else {
				return _performExternalTransition(data);
			}
		}

		private bool _performInternalTransition(Dictionary<string, object> data) {
			if (action != null) {
				action.Invoke(data);
			}
			return true;
		}

		private bool _performLocalTransition(Dictionary<string, object> data) {
			if (targetState == null) {
				return false;
			}
			if (!sourceState.hasAncestor(targetState) && !targetState.hasAncestor(sourceState)) {
				return false;
			}
			StateMachine lca = _findLeastCommonAncestor();
			Sub containingSubState = lca.currentState as Sub;
			lca = containingSubState._submachine;
			lca.switchState(sourceState, targetState, action, data);
			return true;
		}

		private bool _performExternalTransition(Dictionary<string, object> data) {
			if (targetState == null) {
				return false;
			}
			StateMachine lca = _findLeastCommonAncestor();
			lca.switchState(sourceState, targetState, action, data);
			return true;
		}

		private bool _canPerformTransition(Dictionary<string, object> data) {
			return (guard == null || guard.Invoke(data));
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
			return lca;
		}
	}

}
