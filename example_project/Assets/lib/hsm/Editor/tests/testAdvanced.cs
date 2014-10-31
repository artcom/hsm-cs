using System.Collections.Generic;
using NUnit.Framework;
using Hsm;

namespace UnitTesting {
	
	[TestFixture]
	// See advanced.png
	class AdvancedTests : AssertionHelper {
				
		public StateMachine sm;
		public static List<string> log = new List<string>();

		public class LoggingState : State {

			public LoggingState(string pId) : base(pId) {}
			public override void Enter(State sourceState, State targetstate, Dictionary<string, object> data) {
				log.Add(id + ":entered(source:" + ((sourceState != null) ? sourceState.id : "null") + ")");
				base.Enter(sourceState, targetstate, data);
			}
			public override void Exit(State nextState) {
				log.Add(id + ":exited(target:" + ((nextState != null) ? nextState.id : "null") + ")");
				base.Exit(nextState);
			}
		}

		public AdvancedTests() {
			// Statemachine 'a'
			LoggingState a1 = new LoggingState("a1")
				.AddHandler("T1", data => {
					return "a2";
					// We do not (yet) have a proper guard implementation and
					// cannot express what is depicted in diagram!
				});
			LoggingState a2 = new LoggingState("a2")
				.AddHandler("T2", data => {
					sm.handleEvent("T3");
					return "a3";
				});
			LoggingState a3 = new LoggingState("a3")
				.AddHandler("T3", data => {
					return "a1";
				});
			
			Sub a = new Sub("a", new StateMachine(
				a1, a2, a3
			))
			.AddHandler("T1", data => {
				return "b2";
			});
			
			// Statemachine 'b'
			var b1 = new LoggingState("b1");
			var b21 = new LoggingState("b21");
			var b22 = new LoggingState("b22");
			
			var b2 = new Sub("b2", new StateMachine(
				b21, b22
			));
			
			var b = new Sub("b", new StateMachine(
				b1, b2
			));
			
			// Statemachine 'c'
			var c11 = new LoggingState("c11");
			var c12 = new LoggingState("c12");
			
			var c21 = new LoggingState("c21");
			var c22 = new LoggingState("c22");
			
			var c = new Parallel("c",
			    new StateMachine(c11, c12),
			    new StateMachine(c21, c22)
			);
			sm = new StateMachine(a, b, c);
		}
		
		[SetUp]
		public void SetUp() {
			sm.setup();
		}
		
		[TearDown]
		public void TearDown() {
			sm.tearDown(null);
			log.Clear();
		}

		[Test]
		public void Enter() {
			Expect(sm.currentState.id, Is.EqualTo("a"));
		}

		[Test]
		public void RunToCompletion() {
			var sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("a1"));

			sm.handleEvent("T1");
			Expect(sm.currentState.id, Is.EqualTo("a"));
			sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("a2"));

			log.Clear(); // start test at a2
			sm.handleEvent("T2");
			Expect(sm.currentState.id, Is.EqualTo("a"));
			sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("a1"));
			Expect(log, Is.EqualTo(new[] {
				"a2:exited(target:a3)",
				"a3:entered(source:a2)",
				"a3:exited(target:a1)",
				"a1:entered(source:a3)"})
			);
		}
	}
}
