using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsm {

	public class Handler {

		public State target;
		public Action<Dictionary<string, object>> action;
		
		public Handler(State target, Action<Dictionary<string, object>> action) {
			this.target = target;
			this.action = action;
		}
	}
}