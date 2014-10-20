using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hsm {

	[System.Serializable]
	public class StateMachine {
		[SerializeField]
		public readonly List<State> states = new List<State>();
		[SerializeField]
		private State initialState;
		[SerializeField]
		private State currentState;

		public StateMachine() {
		}

		public StateMachine(List<State> pStates) {
			states = pStates;
		}

		public void setup() {
			if (states.Count == 0) {
				throw new UnityException("StateMachine.setup: Must have states!");
			}
			if (initialState == null) {
				initialState = states[0];
			}
			_enterState(null, initialState, new Dictionary<string, object>());
		}

		public StateMachine addState(State pState) {
			// TODO: Check if state with id already exists
			states.Add(pState);
			return this;
		}

		public void handleEvent(string evt) {
			handleEvent(evt, new Dictionary<string, object>());
		}

		public void handleEvent(string evt, Dictionary<string, object> data) {
			if (!currentState.handlers.ContainsKey(evt)) {
				Debug.LogWarning("unhandled event " + evt + " in state " + currentState.id);
				return;
			}
			string result = currentState.handlers[evt].Invoke(data);
			State nextstate = states.Find(state => state.id == result);
			if (nextstate != null) {
				_switchState(currentState, nextstate, data);
			}
		}

		private void _enterState(State sourceState, State targetState, Dictionary<string, object> data) {
			Debug.Log("enterState: " + targetState.id);
			currentState = targetState;
			targetState._enter(sourceState, targetState, data);
		}

		private void _switchState(State sourceState, State targetState, Dictionary<string, object> data) {
			sourceState._exit(targetState);
			currentState = targetState;
			targetState._enter(sourceState, targetState, data);
		}
	}
}

