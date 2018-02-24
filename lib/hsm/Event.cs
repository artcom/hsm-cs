using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsm {
	
	struct Event {
		public string evt;
		public Dictionary<string, object> data;

		public Event(string evt, Dictionary<string, object> data) {
			this.evt = evt;
			this.data = data;
		}
	}
}
