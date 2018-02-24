using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsm {

	public class Handler {

		public State targetState;
		public TransitionKind kind;
		public Action<Dictionary<string, object>> action;
		
		public Handler(State targetState, TransitionKind kind, Action<Dictionary<string, object>> action) {
			this.targetState = targetState;
			this.kind = kind;
			this.action = action;
		}
	}
}