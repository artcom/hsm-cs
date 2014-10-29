using System.Collections.Generic;
using NUnit.Framework;
using Hsm;

namespace UnitTesting {
	
	[TestFixture]
	// See advanced.png
	internal class AdvancedTests : AssertionHelper {
				
		public StateMachine sm;
		public static List<string> log = new List<string>();

		public class LoggingState : State {

			public LoggingState(string pId) : base(pId) {}
			public override void _enter(State sourceState, State targetstate, Dictionary<string, object> data) {
				log.Add(id + ":entered(source:" + ((sourceState != null) ? sourceState.id : "null") + ")");
				base._enter(sourceState, targetstate, data);
			}
			public override void _exit(State nextState) {
				log.Add(id + ":exited(target:" + ((nextState != null) ? nextState.id : "null") + ")");
				base._exit(nextState);
			}
		}

		public AdvancedTests() {
			// Statemachine 'a'
			LoggingState a1 = new LoggingState("a1")
				.addHandler("T1", (data) => {
					return "a2";
					// We do not (yet) have a proper guard implementation and
					// cannot express what is depicted in diagram!
				});
			LoggingState a2 = new LoggingState("a2")
				.addHandler("T2", (data) => {
					sm.handleEvent("T3");
					return "a3";
				});
			LoggingState a3 = new LoggingState("a3")
				.addHandler("T3", (data) => {
					return "a1";
				});
			
			Sub a = new Sub("a", new StateMachine(
				a1, a2, a3
			))
			.addHandler("T1", (data) => {
				return "b2";
			});
			
			// Statemachine 'b'
			LoggingState b1 = new LoggingState("b1");
			LoggingState b21 = new LoggingState("b21");
			LoggingState b22 = new LoggingState("b22");
			
			Sub b2 = new Sub("b2", new StateMachine(
				b21, b22
			));
			
			Sub b = new Sub("b", new StateMachine(
				b1, b2
			));
			
			// Statemachine 'c'
			LoggingState c11 = new LoggingState("c11");
			LoggingState c12 = new LoggingState("c12");
			
			LoggingState c21 = new LoggingState("c21");
			LoggingState c22 = new LoggingState("c22");
			
			Parallel c = new Parallel("c",
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
			Sub sub = sm.currentState as Sub;
			Expect(sub.submachine.currentState.id, Is.EqualTo("a1"));

			sm.handleEvent("T1");
			Expect(sm.currentState.id, Is.EqualTo("a"));
			sub = sm.currentState as Sub;
			Expect(sub.submachine.currentState.id, Is.EqualTo("a2"));

			log.Clear(); // start test at a2
			sm.handleEvent("T2");
			Expect(sm.currentState.id, Is.EqualTo("a"));
			sub = sm.currentState as Sub;
			Expect(sub.submachine.currentState.id, Is.EqualTo("a1"));
			Expect(log, Is.EqualTo(new[] {
				"a2:exited(target:a3)",
				"a3:entered(source:a2)",
				"a3:exited(target:a1)",
				"a1:entered(source:a3)"})
			);
		}
	}
}
