using System.Collections.Generic;

namespace Hsm {

	interface INestedState {
		bool _handle(string evt, Dictionary<string, object> data);
  }

}
