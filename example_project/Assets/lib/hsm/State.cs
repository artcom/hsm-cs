using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Hsm {
	
	[System.Serializable]
	public class State {
		[SerializeField]
		public string id;
		
		private Action<State, State> enterAction = null;
		private Action<State> exitAction = null;
		
		public Dictionary<string, Func<Dictionary<string, object>, String>> handlers; // = new Dictionary<string, Action>();
		
		public State(string pId) {
			this.handlers = new Dictionary<string, Func<Dictionary<string, object>, String>>();
			id = pId;
		}
		
		public State addHandler(string evt, Func<Dictionary<string, object>, String> handler) {
			handlers[evt] = handler;
			return this;
		}
		
		public void _enter(State sourceState, State targetstate, Dictionary<string, object> data) {
			if (enterAction != null) {
				enterAction.Invoke(sourceState, targetstate);
			}
		}
		
		public void _exit(State nextstate) {
			if (exitAction != null) {
				exitAction.Invoke(nextstate);
			}
		}
		
		public State OnEnter(Action<State, State> action) {
			enterAction = action;
			return this;
		}
		
		public State OnExit(Action<State> action) {
			exitAction = action;
			return this;
		}
	}

}
