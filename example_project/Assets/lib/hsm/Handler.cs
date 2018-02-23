using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsm {

	public class Handler {

		public State target;
		public TransitionKind kind;
		public Action<Dictionary<string, object>> action;
		
		public Handler(State target, TransitionKind kind, Action<Dictionary<string, object>> action) {
			this.target = target;
			this.kind = kind;
			this.action = action;
		}
	}
}