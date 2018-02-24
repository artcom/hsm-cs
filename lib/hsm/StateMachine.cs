using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hsm {

	public class StateMachine {
		public State container;
		public List<State> states = new List<State>();
		public State initialState;
		public State currentState;

		private bool eventInProgress = false;

		private Queue<Event> eventQueue = new Queue<Event>();

		public StateMachine(List<State> pStates) {
			states = pStates;
			_setOwners();
			_setInitialState();
		}

		public StateMachine(params State[] pStates) {
			states.AddRange(pStates);
			_setOwners();
			_setInitialState();
		}

		private void _setOwners() {
			foreach (State state in states) {
				state.owner = this;
			}
		}

		private void _setInitialState() {
			if (states.Count == 0) {
				return;
			}
			initialState = states[0];
		}

		public void setup() {
			if (states.Count == 0) {
				throw new UnityException("StateMachine.setup: Must have states!");
			}
			enterState(null, initialState, new Dictionary<string, object>());
		}

		public void tearDown(Dictionary<string, object> data) {
			currentState.Exit(currentState, null, data);
			currentState = null;
		}

		public StateMachine addState(State pState) {
			states.Add(pState);
			_setOwners();
			_setInitialState();
			return this;
		}

		public void handleEvent(string evt) {
			handleEvent(evt, new Dictionary<string, object>());
		}

		public void handleEvent(string evt, Dictionary<string, object> data) {
			Event myEvent = new Event(evt, data);
			eventQueue.Enqueue(myEvent);
			if (eventInProgress == true) {
				// EnQueue
			} else {
				// DeQueue
				eventInProgress = true;
				Event curEvent;
				while (eventQueue.Count > 0) {
					curEvent = eventQueue.Dequeue();
					Handle(curEvent.evt, curEvent.data);
				}
				eventInProgress = false;
			}
		}

		public bool Handle(string evt, Dictionary<string, object> data) {
			// check if current state is a (nested) statemachine, if so, give it the event.
			// if it handles the event, stop processing here.
			if (currentState is INestedState) {
				INestedState nested = currentState as INestedState;
				if (nested.Handle(evt, data)) {
					return true;
				}
			}
			
			if (currentState == null) {
				return false;
			}
			if (!currentState.handlers.ContainsKey(evt)) {
				return false;
			}
			
			List<Handler> handlers = currentState.handlers[evt];
			foreach (Handler handler in handlers) {
				Transition transition = new Transition(currentState, handler);
				if (transition.performTransition(data)) {
					return true;
				}
			}
			return false;
		}

		public void switchState(State sourceState, State targetState, Action<Dictionary<string, object>> action, Dictionary<string, object> data) {
			currentState.Exit(sourceState, targetState, data);
			if (action != null) {
				action.Invoke(data);
			}
			enterState(sourceState, targetState, data);
		}

		public void enterState(State sourceState, State targetState, Dictionary<string, object> data) {
			var targetPath = targetState.owner.getPath();
			var targetLevel = targetPath.Count;
			var thisLevel = this.getPath().Count;
			if (targetLevel < thisLevel) {
				currentState = initialState;
			} else if (targetLevel == thisLevel) {
				currentState = targetState;
			} else {
				currentState = targetPath[thisLevel].container;
			}
			currentState.Enter(sourceState, targetState, data);
		}

		public List<StateMachine> getPath() {
			List<StateMachine> path = new List<StateMachine>();
			StateMachine stateMachine = this;
			while (stateMachine != null) {
				path.Insert(0, stateMachine);
				if (stateMachine.container == null) {
					break;
				}
				stateMachine = stateMachine.container.owner;
			}
			return path;
		}

	}
}

