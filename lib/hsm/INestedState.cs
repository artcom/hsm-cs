using System.Collections.Generic;

namespace Hsm {

	interface INestedState {
		bool Handle(string evt, Dictionary<string, object> data);
		List<string> getActiveStateConfiguration();
	}

}
