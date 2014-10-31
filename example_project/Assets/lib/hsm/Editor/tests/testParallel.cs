using Hsm;
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTesting {

	[TestFixture]
	class ParallelTests : AssertionHelper {

		[Test]
		public void Instantiation() {
			// Supply parallel statemachines as List
			var par = new Parallel("par", new List<StateMachine>{
				new StateMachine(),
				new StateMachine()
			});
			Expect(par.id, Is.EqualTo("par"));
			Expect(par, Is.InstanceOf<State>());
			Expect(par, Is.InstanceOf<Parallel>());
			Expect(par._submachines.Count, Is.EqualTo(2));

			// Supply parallel statemachines as Array
			par = new Parallel("par", new[] {
				new StateMachine(),
				new StateMachine()
			});
			Expect(par.id, Is.EqualTo("par"));
			Expect(par, Is.InstanceOf<State>());
			Expect(par, Is.InstanceOf<Parallel>());
			Expect(par._submachines.Count, Is.EqualTo(2));

			// Start with an initially empty Parallel Statemachine
			par = new Parallel("par");
			par.AddStateMachine(new StateMachine()).AddStateMachine(new StateMachine());
		  Expect(par.id, Is.EqualTo("par"));
		  Expect(par, Is.InstanceOf<State>());
		  Expect(par, Is.InstanceOf<Parallel>());
		  Expect(par._submachines.Count, Is.EqualTo(2));
		}

		[Test]
		public void Keyboard() {
			var _numlockMachine = new StateMachine(
				new State("NumLockOff").AddHandler("numlock", data => "NumLockOn"),
				new State("NumLockOn").AddHandler("numlock", data => "NumLockOff")
			);
			var _capslockMachine = new StateMachine(
				new State("CapsLockOff").AddHandler("capslock", data => "CapsLockOn"),
				new State("CapsLockOn").AddHandler("capslock", data => "CapsLockOff")
			);

      Parallel keyBoardOnState = new Parallel("KeyboardOn", new[] {
				_capslockMachine,
				_numlockMachine
			}).AddHandler("unplug", data => "KeyboardOff");

			var sm = new StateMachine(
				new State("KeyboardOff").AddHandler("plug", data => "KeyboardOn"),
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
