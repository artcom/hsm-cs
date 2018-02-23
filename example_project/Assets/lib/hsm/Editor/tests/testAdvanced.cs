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

		public class LoggingSub : Sub {
			public LoggingSub(string pId, StateMachine submachine) : base(pId, submachine) {}
			public override void Enter(State sourceState, State targetstate, Dictionary<string, object> data) {
				log.Add(id + ":entered(source:" + ((sourceState != null) ? sourceState.id : "null") + ")");
				base.Enter(sourceState, targetstate, data);
			}
			public override void Exit(State nextState) {
				base.Exit(nextState);
				log.Add(id + ":exited(target:" + ((nextState != null) ? nextState.id : "null") + ")");
			}
		}

		public AdvancedTests() {
			// Statemachine 'a'
			LoggingState a1 = new LoggingState("a1");
			LoggingState a2 = new LoggingState("a2");
			LoggingState a3 = new LoggingState("a3");	
			Sub a = new LoggingSub("a", new StateMachine(
				a1, a2, a3
			));

			a1.AddHandler("T1", a2);
			a1.AddHandler("TI", a1, Transition.Internal, data => {
				log.Add("a1:action(TI)");
			});

			a2.AddHandler("T2", a3, Transition.External, data => {
				log.Add("a2:action(T2)");
				sm.handleEvent("T3");
			});
		
			a3.AddHandler("T3", a1);
			
			// Statemachine 'b'
			var b1 = new LoggingState("b1");
			var b21 = new LoggingState("b21");
			var b22 = new LoggingState("b22");
			
			var b2 = new Sub("b2", new StateMachine(
				b21, b22
			));
			
			var b = new LoggingSub("b", new StateMachine(
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

			a.AddHandler("ToB", b);
			
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
		public void TestExit() {
			var sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("a1"));

			log.Clear();

			sm.handleEvent("ToB");
			Expect(sm.currentState.id, Is.EqualTo("b"));
			sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("b1"));
			Expect(log, Is.EqualTo(new[] {
				"a1:exited(target:null)",
				"a:exited(target:b)",
				"b:entered(source:a)",
				"b1:entered(source:null)"
			}));
		}

		[Test]
		public void TestInternalTransition() {
			var sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("a1"));

			log.Clear();

			sm.handleEvent("TI");
			sm.handleEvent("TI");
			sm.handleEvent("TI");
			sm.handleEvent("TI");

			Expect(sub._submachine.currentState.id, Is.EqualTo("a1"));
			Expect(log, Is.EqualTo(new[] {
				"a1:action(TI)",
				"a1:action(TI)",
				"a1:action(TI)",
				"a1:action(TI)"
			}));
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
				"a2:action(T2)",
				"a3:entered(source:a2)",
				"a3:exited(target:a1)",
				"a1:entered(source:a3)"})
			);
		}
	}
}
