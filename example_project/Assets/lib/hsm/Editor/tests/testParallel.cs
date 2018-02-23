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
			State numLockOff = new State("NumLockOff");
			State numLockOn = new State("NumLockOn");
			var _numlockMachine = new StateMachine(
				numLockOff.AddHandler("numlock", numLockOn),
				numLockOn.AddHandler("numlock", numLockOff)
			);
			State capsLockOff = new State("CapsLockOff");
			State capsLockOn = new State("CapsLockOn");
			var _capslockMachine = new StateMachine(
				capsLockOff.AddHandler("capslock", capsLockOn),
				capsLockOn.AddHandler("capslock", capsLockOff)
			);

			State keyBoardOff = new State("KeyboardOff");
			Parallel keyBoardOn = new Parallel("KeyboardOn", new[] {
				_capslockMachine,
				_numlockMachine
			}).AddHandler("unplug", keyBoardOff);

			var sm = new StateMachine(
				keyBoardOff.AddHandler("plug", keyBoardOn),
				keyBoardOn
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
			Expect(_numlockMachine.currentState, Is.EqualTo(null));
			Expect(_capslockMachine.currentState, Is.EqualTo(null));

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
