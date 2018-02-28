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
			public LoggingState(string pId) : base(pId) {
				this.id = pId;
			}
			public override void Enter(State sourceState, State targetstate, Dictionary<string, object> data) {
				log.Add(id + ":entered");
				base.Enter(sourceState, targetstate, data);
			}
			public override void Exit(State sourceState, State targetstate, Dictionary<string, object> data) {
				log.Add(id + ":exited");
				base.Exit(sourceState, targetstate, data);
			}
		}

		public class LoggingSub : Sub {
			public LoggingSub(string pId, StateMachine submachine) : base(pId, submachine) {
				this.id = pId;
			}
			public override void Enter(State sourceState, State targetstate, Dictionary<string, object> data) {
				log.Add(id + ":entered");
				base.Enter(sourceState, targetstate, data);
			}
			public override void Exit(State sourceState, State targetstate, Dictionary<string, object> data) {
				base.Exit(sourceState, targetstate, data);
				log.Add(id + ":exited");
			}
		}

		public class LoggingParallel : Parallel {
			public LoggingParallel(string pId, params StateMachine[] submachines) : base(pId, submachines) {
				this.id = pId;
			}
			public override void Enter(State sourceState, State targetstate, Dictionary<string, object> data) {
				log.Add(id + ":entered");
				base.Enter(sourceState, targetstate, data);
			}
			public override void Exit(State sourceState, State targetstate, Dictionary<string, object> data) {
				base.Exit(sourceState, targetstate, data);
				log.Add(id + ":exited");
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

			a1.AddHandler("TI", a1, TransitionKind.Internal, data => {
				log.Add("a1:action(TI)");
			});


			a1.AddHandler("T1", a2);			
			a2.AddHandler("T2", a3, TransitionKind.External, data => {
				log.Add("a2:action(T2)");
				sm.handleEvent("T3");
			});
		
			a3.AddHandler("T3", a1);
			
			// Statemachine 'b'
			var b1 = new LoggingState("b1");
			var b21 = new LoggingState("b21");
			var b22 = new LoggingState("b22");
			
			var b2 = new LoggingSub("b2", new StateMachine(
				b21, b22
			));

			var b311 = new LoggingState("b311");
			var b312 = new LoggingState("b312");
			var b321 = new LoggingState("b321");
			var b322 = new LoggingState("b322");
			var b3 = new LoggingParallel("b3",
				new StateMachine(b311, b312),
				new StateMachine(b321, b322)
			);
			
			var b = new LoggingSub("b", new StateMachine(
				b1, b2, b3
			));

			a.AddHandler("T4", b2);
			a.AddHandler("T5", b);
			b.AddHandler("T6", b22, TransitionKind.Local);
			b22.AddHandler("T7", b, TransitionKind.Local);

			a1.AddHandler("T8", b322);
			b311.AddHandler("T9", a1);

			a.AddHandler("T10", b3);

			b311.AddHandler("T11", b311, TransitionKind.Internal, data => {
				log.Add("b311:action(T11)");
			});
			b321.AddHandler("T11", b321, TransitionKind.Internal, data => {
				log.Add("b321:action(T11)");
			});

			b311.AddHandler("T12", b312, data => {
				return (data["v"] == null);
			});
			b321.AddHandler("T12", b322, data => {
				return (data["v"] != null);
			});
			
			sm = new StateMachine(a, b);
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
		public void TestEnter() {
			Expect(sm.currentState.id, Is.EqualTo("a"));
		}

		[Test]
		public void TestPath() {
			sm.handleEvent("T4");
			var sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("b2"));

			var subsub = sub._submachine.currentState as Sub;
			Expect(subsub._submachine.currentState.id, Is.EqualTo("b21"));
			
			List<StateMachine> path = subsub._submachine.getPath();
			Expect(path.Count, Is.EqualTo(3));
		}

		[Test]
		public void TestActiveStateConfiguration() {
			List<string> activeStates = sm.getActiveStateConfiguration();
			Expect(activeStates.Count, Is.EqualTo(2));
			Expect(activeStates[0], Is.EqualTo("a"));		
			Expect(activeStates[1], Is.EqualTo("a1"));		
		}

		[Test]
		public void TestExit() {
			var sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("a1"));

			log.Clear();

			sm.handleEvent("T5");
			Expect(sm.currentState.id, Is.EqualTo("b"));
			sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("b1"));
			Expect(log, Is.EqualTo(new[] {
				"a1:exited",
				"a:exited",
				"b:entered",
				"b1:entered"
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
		public void TestInternalTransitionParallel() {
			sm.handleEvent("T10");
			var sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("b3"));

			log.Clear();

			sm.handleEvent("T11");
			Expect(log, Is.EqualTo(new[] {
				"b311:action(T11)",
				"b321:action(T11)"
			}));
		}

		[Test]
		public void TestLocalTransition() {
			sm.handleEvent("T5");

			var sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("b1"));

			log.Clear();
			sm.handleEvent("T6");

			var subsub = sub._submachine.currentState as Sub;
			Expect(subsub._submachine.currentState.id, Is.EqualTo("b22"));
			Expect(log, Is.EqualTo(new[] {
				"b1:exited",
				"b2:entered",
				"b22:entered"
			}));

			log.Clear();

			sm.handleEvent("T7");
			Expect(sub._submachine.currentState.id, Is.EqualTo("b1"));
			Expect(log, Is.EqualTo(new[] {
				"b22:exited",
				"b2:exited",
				"b1:entered"
			}));
		}

		[Test]
		public void TestParallelStates() {
			var sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("a1"));

			log.Clear();

			sm.handleEvent("T8");
			sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("b3"));

			var par = sub._submachine.currentState as Parallel;
			Expect(par._submachines[0].currentState.id, Is.EqualTo("b311"));
			Expect(par._submachines[1].currentState.id, Is.EqualTo("b322"));

			Expect(log, Is.EqualTo(new[] {
				"a1:exited",
				"a:exited",
				"b:entered",
				"b3:entered",
				"b311:entered",
				"b322:entered"
			}));

			log.Clear();

			sm.handleEvent("T9");
			sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("a1"));

			Expect(log, Is.EqualTo(new[] {
				"b311:exited",
				"b322:exited",
				"b3:exited",
				"b:exited",
				"a:entered",
				"a1:entered"
			}));
		}

		[Test]
		public void TestParallelStatesGuarded() {
			sm.handleEvent("T10");
			var sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("b3"));

			var par = sub._submachine.currentState as Parallel;
			Expect(par._submachines[0].currentState.id, Is.EqualTo("b311"));
			Expect(par._submachines[1].currentState.id, Is.EqualTo("b321"));
			
			log.Clear();

			Dictionary<string, object> data = new Dictionary<string, object>
			{
				{"v", "foobar"}
			};
			sm.handleEvent("T12", data);
			Expect(par._submachines[0].currentState.id, Is.EqualTo("b311"));
			Expect(par._submachines[1].currentState.id, Is.EqualTo("b322"));

			Expect(log, Is.EqualTo(new[] {
				"b321:exited",
				"b322:entered"
			}));
		}

		[Test]
		public void TestRunToCompletion() {
			var sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("a1"));

			sm.handleEvent("T1");
			Expect(sm.currentState.id, Is.EqualTo("a"));
			sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("a2"));

			log.Clear(); // start test at a2
			log = new List<string>();

			sm.handleEvent("T2");
			Expect(sm.currentState.id, Is.EqualTo("a"));
			
			sub = sm.currentState as Sub;
			Expect(sub._submachine.currentState.id, Is.EqualTo("a1"));
			
			Expect(log, Is.EqualTo(new[] {
				"a2:exited",
				"a2:action(T2)",
				"a3:entered",
				"a3:exited",
				"a1:entered"
				})
			);

			System.Console.WriteLine(log);
		}
	}
}
