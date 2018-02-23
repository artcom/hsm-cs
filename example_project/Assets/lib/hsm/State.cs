﻿using System;
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
			state.enterActionWithData = null;
			return state;
		}

		public static T OnEnter<T>(this T state, Action<State, State, Dictionary<string, object>> action) where T : State {
			state.enterActionWithData = action;
			state.enterAction = null;
			return state;
		}

		public static T AddHandler<T>(this T state, string eventName, State target) where T : State {
			state.createHandler(eventName, target, Transition.External, null);
			return state;
		}

		public static T AddHandler<T>(this T state, string eventName, State target, Action<Dictionary<string, object>> action) where T : State {
			state.createHandler(eventName, target, Transition.External, action);
			return state;
		}

		public static T AddHandler<T>(this T state, string eventName, State target, Transition kind, Action<Dictionary<string, object>> action) where T : State {
			state.createHandler(eventName, target, kind, action);
			return state;
		}
	}

	[System.Serializable]
	public class State {
		[SerializeField]
		public string id;
		public Action<State, State> enterAction = null;
		public Action<State, State, Dictionary<string, object>> enterActionWithData = null;
		public Action<State> exitAction = null;
		public Dictionary<string, List<Handler>> handlers = new Dictionary<string, List<Handler>>();

		public State(string pId) {
			id = pId;
		}

		public virtual void Enter(State sourceState, State targetstate, Dictionary<string, object> data) {
			if (enterAction != null) {
				enterAction.Invoke(sourceState, targetstate);
			}
			if (enterActionWithData != null) {
				enterActionWithData.Invoke(sourceState, targetstate, data);
			}
		}
		
		public virtual void Exit(State nextState) {
			if (exitAction != null) {
				exitAction.Invoke(nextState);
			}
		}

		public void createHandler(string eventName, State target, Transition kind, Action<Dictionary<string, object>> action) {
			Handler handler = new Handler(target, kind, action);
			if (!handlers.ContainsKey(eventName)) {
				handlers[eventName] = new List<Handler>();
			}
			handlers[eventName].Add(handler);
		}
	}
}
