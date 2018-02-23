using NUnit.Framework;
using Hsm;

namespace UnitTesting {

	[TestFixture]
	class ToggleTests : AssertionHelper {

		private int enteredOnCount;
		private int exitedOffCount;

		public StateMachine _sm;

		public ToggleTests() {
			State offState = new State("OffState");
			State onState = new State("OnState");
			_sm = new StateMachine(
				offState
				.AddHandler("switched_on", onState)
				.OnExit((sourceState, targetState) => {
					exitedOffCount += 1;
				}),
				onState
				.AddHandler("switched_off", offState)
				.OnEnter((s, t) => enteredOnCount++)
			);
		}

		[SetUp]
		public void SetUp() {
			enteredOnCount = 0;
			enteredOnCount = 0;
			_sm.setup();
		}

		[TearDown]
		public void TearDown() {
			_sm.tearDown(null);
		}

		[Test]
		public void TestToggle() {
			Expect(_sm.currentState.id, Is.EqualTo("OffState"));

			_sm.handleEvent("switched_off");
			Expect(_sm.currentState.id, Is.EqualTo("OffState"));

			_sm.handleEvent("switched_on");
			Expect(_sm.currentState.id, Is.EqualTo("OnState"));

			_sm.handleEvent("switched_on");
			Expect(_sm.currentState.id, Is.EqualTo("OnState"));
		}

		[Test]
		public void TestEnterExit() {
			Expect(_sm.currentState.id, Is.EqualTo("OffState"));
			Expect(enteredOnCount, Is.EqualTo(0));
			Expect(exitedOffCount, Is.EqualTo(0));

			_sm.handleEvent("switched_off");
			Expect(enteredOnCount, Is.EqualTo(0));
			Expect(exitedOffCount, Is.EqualTo(0));

			_sm.handleEvent("switched_on");
			Expect(enteredOnCount, Is.EqualTo(1));
			Expect(exitedOffCount, Is.EqualTo(1));

			_sm.handleEvent("switched_on");
			Expect(enteredOnCount, Is.EqualTo(1));
			Expect(exitedOffCount, Is.EqualTo(1));

			_sm.handleEvent("switched_off");
			Expect(enteredOnCount, Is.EqualTo(1));
			Expect(exitedOffCount, Is.EqualTo(1));
		}
	}
}
