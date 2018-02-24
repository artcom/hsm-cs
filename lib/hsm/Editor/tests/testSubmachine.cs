using Hsm;
using NUnit.Framework;

namespace UnitTesting {
	
	[TestFixture]
	class SubTests : AssertionHelper {

		[Test]
		public void Instantiation() {
			var sub = new Sub("sub", new StateMachine(new State("on"), new State("off")));
			Expect(sub.id, EqualTo("sub"));
			Expect(sub, Is.InstanceOf<State>());
			Expect(sub, Is.InstanceOf<Sub>());
		}

		[Test]
		public void QuietLoud() {
			State quiet = new State("Quiet");
			State loud = new State("Loud");
			State offState = new State("OffState");
			Sub onState = new Sub("OnState", new StateMachine(
				quiet.AddHandler("volume_up", loud),
				loud.AddHandler("volume_down", quiet)
			));
			onState.AddHandler("switched_off", offState);
			offState.AddHandler("switched_on", onState);

			var sm = new StateMachine(offState, onState);
			sm.setup();

			Expect(sm.currentState.id, Is.EqualTo("OffState"));
			sm.handleEvent("switched_on");
			Expect(sm.currentState.id, Is.EqualTo("OnState"));
			var sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("Quiet"));

			sm.handleEvent("volume_up");
			Expect(sub._submachine.currentState.id, Is.EqualTo("Loud"));

			sm.handleEvent("switched_off");
			Expect(sm.currentState.id, Is.EqualTo("OffState"));
			Expect(sub._submachine.currentState, Is.EqualTo(null));

			sm.handleEvent("switched_on");
			Expect(sm.currentState.id, Is.EqualTo("OnState"));
			sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("Quiet"));
		}

		[Test]
		public void SimpleSub() {
			State onState = new State("on");
			State offState = new State("off");
			
			State powered_off = new State("powered_off");
			Sub powered_on = new Sub("powered_on", new StateMachine(
				onState.AddHandler("off", offState),
				offState.AddHandler("on", onState)
			));
			
			powered_on.AddHandler("power_off", powered_off);
			var sm = new StateMachine(
				powered_off.AddHandler("power_on", powered_on),
				powered_on
			);
			sm.setup();
			Expect(sm.currentState.id, Is.EqualTo("powered_off"));
			sm.handleEvent("power_on");
			Expect(sm.currentState, Is.InstanceOf<Sub>());
			Expect(sm.currentState.id, Is.EqualTo("powered_on"));
			var sub = sm.currentState as Sub;

			Expect(sub._submachine.currentState, Is.Not.Null);
			Expect(sub._submachine.currentState.id, Is.EqualTo("on"));
		}

	}
}
