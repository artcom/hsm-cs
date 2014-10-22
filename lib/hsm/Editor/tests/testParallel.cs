using Hsm;
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTesting {
	
	[TestFixture]
	internal class ParallelTests : AssertionHelper {

		[Test]
		public void Instantiation() {
			// Supply parallel statemachines as List
			Parallel par = new Parallel("par", new List<StateMachine>(){ new StateMachine(), new StateMachine() });
			Expect(par.id, Is.EqualTo("par"));
			Expect(par, Is.InstanceOf<State>());
			Expect(par, Is.InstanceOf<Parallel>());
			Expect(par.submachines.Count, Is.EqualTo(2));

			// Supply parallel statemachines as Array
			par = new Parallel("par", new[] { new StateMachine(), new StateMachine() });
			Expect(par.id, Is.EqualTo("par"));
			Expect(par, Is.InstanceOf<State>());
			Expect(par, Is.InstanceOf<Parallel>());
			Expect(par.submachines.Count, Is.EqualTo(2));
		}

		[Test]
		public void Keyboard() {
			Parallel keyBoardOnState = new Parallel("KeyboardOn", new[] {
				new StateMachine(
					new State("NumLockOff"),
					new State("NumLockOn")
				),
				new StateMachine(
					new State("CapsLockOn"),
					new State("CapsLockOff")
				)
			});

			StateMachine sm = new StateMachine(
				new State("KeyboardOff"), 
				keyBoardOnState
			);
			sm.setup();
		}
	}
}