using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using Hsm;

namespace UnitTesting {

	[TestFixture]
	internal class ToggleTests : AssertionHelper {

		private int enteredOnCount = 0;
		private int exitedOffCount = 0;

		public StateMachine sm;

		public ToggleTests() {
			sm = new StateMachine(
				new State("OffState")
				.addHandler("switched_on", (data) => {
					return "OnState";
				})
				.OnExit((t) => {
					exitedOffCount += 1;
				}),
				new State("OnState")
				.addHandler("switched_off", (data) => {
					return "OffState";
				})
				.OnEnter((s, t) => {
					enteredOnCount++;
				})
			);
		}

		[SetUp]
		public void SetUp() {
			enteredOnCount = 0;
			enteredOnCount = 0;
			sm.setup();
		}

		[TearDown]
		public void TearDown() {
			sm.tearDown(null);
		}

		[Test]
		public void testToggle() {
			Expect(sm.currentState.id, Is.EqualTo("OffState"));

			sm.handleEvent("switched_off");
			Expect(sm.currentState.id, Is.EqualTo("OffState"));

			sm.handleEvent("switched_on");
			Expect(sm.currentState.id, Is.EqualTo("OnState"));

			sm.handleEvent("switched_on");
			Expect(sm.currentState.id, Is.EqualTo("OnState"));
		}

		[Test]
		public void testEnterExit() {
			Expect(sm.currentState.id, Is.EqualTo("OffState"));
			Expect(enteredOnCount, Is.EqualTo(0));
			Expect(exitedOffCount, Is.EqualTo(0));

			sm.handleEvent("switched_off");
			Expect(enteredOnCount, Is.EqualTo(0));
			Expect(exitedOffCount, Is.EqualTo(0));

			sm.handleEvent("switched_on");
			Expect(enteredOnCount, Is.EqualTo(1));
			Expect(exitedOffCount, Is.EqualTo(1));

			sm.handleEvent("switched_on");
			Expect(enteredOnCount, Is.EqualTo(1));
			Expect(exitedOffCount, Is.EqualTo(1));

			sm.handleEvent("switched_off");
			Expect(enteredOnCount, Is.EqualTo(1));
			Expect(exitedOffCount, Is.EqualTo(1));
		}

	}
}