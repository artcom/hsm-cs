using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsm {

	public static class ExtensionMethods {
		public static T OnEnter<T>(this T state, Action action) where T : State {
			state.enterAction = action;
			state.enterActionWithData = null;
			return state;
		}

		public static T OnEnter<T>(this T state, Action<Dictionary<string, object>> action) where T : State {
			state.enterActionWithData = action;
			state.enterAction = null;
			return state;
		}

		public static T OnExit<T>(this T state, Action action) where T : State {
			state.exitAction = action;
			state.exitActionWithData = null;
			return state;
		}

		public static T OnExit<T>(this T state, Action<Dictionary<string, object>> action) where T : State {
			state.exitActionWithData = action;
			state.exitAction = null;
			return state;
		}

		public static T AddHandler<T>(this T state, string eventName, State target) where T : State {
			state.createHandler(eventName, target, TransitionKind.External, null);
			return state;
		}

		public static T AddHandler<T>(this T state, string eventName, State target, TransitionKind kind) where T : State {
			state.createHandler(eventName, target, kind, null);
			return state;
		}

		public static T AddHandler<T>(this T state, string eventName, State target, Action<Dictionary<string, object>> action) where T : State {
			state.createHandler(eventName, target, TransitionKind.External, action);
			return state;
		}

		public static T AddHandler<T>(this T state, string eventName, State target, TransitionKind kind, Action<Dictionary<string, object>> action) where T : State {
			state.createHandler(eventName, target, kind, action);
			return state;
		}
	}
	
	public class State {
		[SerializeField]
		public string id;
		public StateMachine owner;
		public Action enterAction = null;
		public Action<Dictionary<string, object>> enterActionWithData = null;
		public Action exitAction = null;
		public Action<Dictionary<string, object>> exitActionWithData = null;
		public Dictionary<string, List<Handler>> handlers = new Dictionary<string, List<Handler>>();

		public State(string pId) {
			id = pId;
		}

		public virtual void Enter(State sourceState, State targetstate, Dictionary<string, object> data) {
			if (enterAction != null) {
				enterAction.Invoke();
			}
			if (enterActionWithData != null) {
				enterActionWithData.Invoke(data);
			}
		}
		
		public virtual void Exit(State sourceState, State targetstate, Dictionary<string, object> data) {
			if (exitAction != null) {
				exitAction.Invoke();
			}
			if (exitActionWithData != null) {
				exitActionWithData.Invoke(data);
			}
		}

		public void createHandler(string eventName, State target, TransitionKind kind, Action<Dictionary<string, object>> action) {
			Handler handler = new Handler(target, kind, action);
			if (!handlers.ContainsKey(eventName)) {
				handlers[eventName] = new List<Handler>();
			}
			handlers[eventName].Add(handler);
		}

		public bool hasAncestorStateMachine(StateMachine stateMachine) {
			for (var i = 0; i < owner.getPath().Count; ++i) {
				if (owner.getPath()[i] == stateMachine) {
					return true;
				}
			}
			return false;
		}
	}
}
