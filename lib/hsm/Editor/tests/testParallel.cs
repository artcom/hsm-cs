using Hsm;
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTesting {
	
	[TestFixture]
	internal class ParallelTests : AssertionHelper {

		[Test]
		public void Instantiation() {
			// Supply parallel statemachines as List
			Parallel par = new Parallel("par", new List<StateMachine>(){
				new StateMachine(),
				new StateMachine()
			});
			Expect(par.id, Is.EqualTo("par"));
			Expect(par, Is.InstanceOf<State>());
			Expect(par, Is.InstanceOf<Parallel>());
			Expect(par.submachines.Count, Is.EqualTo(2));

			// Supply parallel statemachines as Array
			par = new Parallel("par", new[] {
				new StateMachine(),
				new StateMachine()
			});
			Expect(par.id, Is.EqualTo("par"));
			Expect(par, Is.InstanceOf<State>());
			Expect(par, Is.InstanceOf<Parallel>());
			Expect(par.submachines.Count, Is.EqualTo(2));

			// Start with an initially empty Parallel Statemachine
			par = new Parallel("par");
			par.addStateMachine(new StateMachine())
			   .addStateMachine(new StateMachine());
		    Expect(par.id, Is.EqualTo("par"));
		    Expect(par, Is.InstanceOf<State>());
		    Expect(par, Is.InstanceOf<Parallel>());
		    Expect(par.submachines.Count, Is.EqualTo(2));
		}

		[Test]
		public void Keyboard() {
			StateMachine _numlockMachine = new StateMachine(
				new State("NumLockOff").addHandler("numlock", (data) => {
					return "NumLockOn";
				}),
				new State("NumLockOn").addHandler("numlock", (data) => {
					return "NumLockOff";
				})
			);
			StateMachine _capslockMachine = new StateMachine(
				new State("CapsLockOff").addHandler("capslock", (data) => {
					return "CapsLockOn";
				}),
				new State("CapsLockOn").addHandler("capslock", (data) => {
					return "CapsLockOff";
				})
			);
				
			Parallel keyBoardOnState = new Parallel("KeyboardOn", new[] {
				_capslockMachine,
				_numlockMachine
			}).addHandler("unplug", (data) => {
				return "KeyboardOff";
			});

			StateMachine sm = new StateMachine(
				new State("KeyboardOff").addHandler("plug", (data) => {
					return "KeyboardOn";
				}), 
				keyBoardOnState
			);
			sm.setup();

			Expect(sm.currentState.id, Is.EqualTo("KeyboardOff"));
			sm.handleEvent("plug");
			Expect(sm.currentState.id, Is.EqualTo("KeyboardOn"));
			Expect(_capslockMachine.currentState.id, Is.EqualTo("CapsLockOff"));
			Expect(_numlockMachine.currentState.id, Is.EqualTo("NumLockOff"));

			// check capslock toggle
			sm.handleEvent("capslock");
			Expect(_capslockMachine.currentState.id, Is.EqualTo("CapsLockOn"));
			Expect(_numlockMachine.currentState.id, Is.EqualTo("NumLockOff"));

			sm.handleEvent("capslock");
			Expect(_capslockMachine.currentState.id, Is.EqualTo("CapsLockOff"));
			Expect(_numlockMachine.currentState.id, Is.EqualTo("NumLockOff"));

			// check numlock toggle
			sm.handleEvent("numlock");
			Expect(_capslockMachine.currentState.id, Is.EqualTo("CapsLockOff"));
			Expect(_numlockMachine.currentState.id, Is.EqualTo("NumLockOn"));

			// now unplug keyboard
			sm.handleEvent("unplug");
			Expect(sm.currentState.id, Is.EqualTo("KeyboardOff"));

			// pressing capslock while unplugged does nothing
			sm.handleEvent("capslock");
			Expect(sm.currentState.id, Is.EqualTo("KeyboardOff"));

			// plug the keyboard back in and check whether the toggles are back at their initial states
			sm.handleEvent("plug");
			Expect(sm.currentState.id, Is.EqualTo("KeyboardOn"));
			Expect(_capslockMachine.currentState.id, Is.EqualTo("CapsLockOff"));
			Expect(_numlockMachine.currentState.id, Is.EqualTo("NumLockOff"));
		}
	}
}