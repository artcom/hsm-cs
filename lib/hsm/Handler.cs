using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsm {

	public class Handler {

		public State targetState;
		public TransitionKind kind;
		public Action<Dictionary<string, object>> action;
		public Func<Dictionary<string, object>, bool> guard;
		
		public Handler(State targetState, TransitionKind kind, Action<Dictionary<string, object>> action, Func<Dictionary<string, object>, bool> guard) {
			this.targetState = targetState;
			this.kind = kind;
			this.action = action;
			this.guard = guard;
		}
	}
}