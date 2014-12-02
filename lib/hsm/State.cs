using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsm {

	public static class ExtensionMethods {
		public static T OnExit<T>(this T state, Action<State> action) where T : State {
			state.exitAction = action;
			return state;
		}

		public static T OnEnter<T>(this T state, Action<State, State> action) where T : State {
			state.enterAction = action;
			return state;
	}

		public static T AddHandler<T>(this T state, string evt, Func<Dictionary<string, object>, string> handler) where T : State {
			state.handlers[evt] = handler;
			return state;
		}
	}

	[System.Serializable]
	public class State {
		[SerializeField]
		public string id;
		public Action<State, State> enterAction = null;
		public Action<State> exitAction = null;
		public Dictionary<string, Func<Dictionary<string, object>, string>> handlers =
			new Dictionary<string, Func<Dictionary<string, object>, string>>();

		public State(string pId) {
			id = pId;
		}

		public virtual void Enter(State sourceState, State targetstate, Dictionary<string, object> data) {
			if (enterAction != null) {
				enterAction.Invoke(sourceState, targetstate);
			}
		}
		
		public virtual void Exit(State nextState) {
			if (exitAction != null) {
				exitAction.Invoke(nextState);
			}
		}
	}

}
