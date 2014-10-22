using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hsm {

	[System.Serializable]
	public class StateMachine {
		[SerializeField]
		public readonly List<State> states = new List<State>();
		[SerializeField]
		public State initialState;
		[SerializeField]
		public State currentState;

		public StateMachine() {
		}

		public StateMachine(List<State> pStates) {
			states = pStates;
		}

		public StateMachine(params State[] pStates) {
			states.AddRange(pStates);
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

		public void tearDown(State nextState) {
			currentState._exit(nextState);
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
			// TODO: Add support for Run-To-Completion Model
			_handle(evt, data);
		}

		public bool _handle(string evt, Dictionary<string, object> data) {
			// check if current state is a (nested) statemachine, if so, give it the event.
			// if it handles the event, stop processing here.
			if (currentState is Sub /*|| currentState is Parallel*/) {
				Sub mySub = currentState as Sub;
				if (mySub._handle(evt, data)) {
					return true;
				}
			}

			if (!currentState.handlers.ContainsKey(evt)) {
				return false;
			}
			string result = currentState.handlers[evt].Invoke(data);
			State nextstate = states.Find(state => state.id == result);
			if (nextstate != null) {
				_switchState(currentState, nextstate, data);
				return true;
			}
			return false;
		}

		public void _enterState(State sourceState, State targetState, Dictionary<string, object> data) {
			currentState = targetState;
			targetState._enter(sourceState, targetState, data);
		}

		private void _switchState(State sourceState, State targetState, Dictionary<string, object> data) {
			sourceState._exit(targetState);
			_enterState(sourceState, targetState, data);
		}
	}
}

