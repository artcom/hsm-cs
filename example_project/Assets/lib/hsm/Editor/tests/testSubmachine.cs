using Hsm;
using NUnit.Framework;

namespace UnitTesting {
	
	[TestFixture]
	internal class SubTests : AssertionHelper {

		[Test]
		public void Instantiation() {
			Sub sub = new Sub("sub", new StateMachine(new State("on"), new State("off")));
			Expect(sub.id, EqualTo("sub"));
			Expect(sub, Is.InstanceOf<State>());
			Expect(sub, Is.InstanceOf<Sub>());
		}

		[Test]
		public void QuietLoud() {
			Sub onState = new Sub("OnState", new StateMachine(
				new State("Quiet")
					.addHandler("volume_up", (data) => {
						return "Loud";
					}
				),
				new State("Loud")
					.addHandler("volume_down", (data) => {
						return "Quiet";
					}
				)
			))
				.addHandler("switched_off", (data) => { return "OffState"; }); 
			State offState = new State("OffState")
				.addHandler("switched_on", (data) => { return "OnState"; });

			StateMachine sm = new StateMachine(offState, onState);
			sm.setup();

			Expect(sm.currentState.id, Is.EqualTo("OffState"));
			sm.handleEvent("switched_on");
			Expect(sm.currentState.id, Is.EqualTo("OnState"));
			Sub sub = sm.currentState as Sub;
			Expect(sub.submachine.currentState.id, Is.EqualTo("Quiet"));

			sm.handleEvent("volume_up");
			Expect(sub.submachine.currentState.id, Is.EqualTo("Loud"));

			sm.handleEvent("switched_off");
			Expect(sm.currentState.id, Is.EqualTo("OffState"));

			sm.handleEvent("switched_on");
			Expect(sm.currentState.id, Is.EqualTo("OnState"));
			sub = sm.currentState as Sub;
			Expect(sub.submachine.currentState.id, Is.EqualTo("Quiet"));
		}

		[Test]
		public void SimpleSub() {
			Sub powered_on_sub = new Sub("powered_on", new StateMachine(
				new State("on").addHandler("off", (data) => { return "off"; }),
				new State("off").addHandler("on", (data) => { return "on"; })
			))
			.addHandler("power_off", (data) => { return "powered_off"; })
			.OnEnter((s, t) => {
				/// Debug.Log(s);
			})
			.OnExit((n) => {
				// Debug.Log(n);
			});
			StateMachine sm = new StateMachine(
				new State("powered_off").addHandler("power_on", (data) => { return "powered_on"; }),
				powered_on_sub
			);
			sm.setup();
			Expect(sm.currentState.id, Is.EqualTo("powered_off"));
			sm.handleEvent("power_on");
			Expect(sm.currentState, Is.InstanceOf<Sub>());
			Expect(sm.currentState.id, Is.EqualTo("powered_on"));
			Sub sub = sm.currentState as Sub;

			Expect(sub.submachine.currentState, Is.Not.Null);
			Expect(sub.submachine.currentState.id, Is.EqualTo("on"));
		}

	}
}