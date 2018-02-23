using UnityEngine;
//using UnityEditor;
using UnityEngine.UI;

using Hsm;

public class StateMachineManager : MonoBehaviour {

	public Text debugText;

	//[SerializeField]
	public StateMachine sm;

	void Start() {
		sm = new StateMachine();

		
		State idleState = new State("idle");
		State onState = new State("on");
		State offState = new State("off");
		
		// Idle State
		idleState.OnEnter((sourceState, targetState) => {
			Debug.Log(targetState.id + " -> on_enter (from: " +
			((sourceState != null) ? sourceState.id : "null") + ")");
		})
		.OnExit((sourceState, targetState) => {
			Debug.Log("idle exit -> " + targetState.id);
		})
		.AddHandler("start", offState);

		// On State
		onState.OnEnter((sourceState, targetState) => {
			Debug.Log(targetState.id + " -> on_enter (from: " +
			((sourceState != null) ? sourceState.id : "null") + ")");
		})
		.AddHandler("off", offState)
		.OnExit((sourceState, targetState) => {
			Debug.Log("on exit -> " + targetState.id);
		});

		// Off State
		offState.OnEnter((sourceState, targetState) => {
			Debug.Log(targetState.id + " -> on_enter (from: " +
			((sourceState != null) ? sourceState.id : "null") + ")");
		})
		.OnExit((sourceState, targetState) => {
			Debug.Log("on on_exit -> " + targetState.id);
		})
		.AddHandler("on", onState);
		
		sm.addState(idleState)
		.addState(onState)
		.addState(offState);
		debugText.text = "uninitialized";

		try {
			sm.setup();
		} catch(UnityException e) {
			Debug.LogException(e);
			debugText.text = e.Message;
		}

		sm.handleEvent("start");
		sm.handleEvent("on");
		sm.handleEvent("off");
	}

	void Awake() {
	}
}

/*
[CustomPropertyDrawer(typeof(Statemachine))]
public class StateMachineDrawer : PropertyDrawer {

	bool unfolded = false;
	int numStates = 0;

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		if (unfolded) {
			return base.GetPropertyHeight (property, label) + 20 * numStates + 10;
		} else {
			return base.GetPropertyHeight (property, label);
		}
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		// SerializedProperty currentStateProp = property.FindPropertyRelative("currentState");
		// SerializedProperty initialStateProp = property.FindPropertyRelative("initialState");
		SerializedProperty statesProp = property.FindPropertyRelative("states");
		numStates = statesProp.arraySize;

		EditorGUI.BeginProperty(position, label, property);
		unfolded = EditorGUI.Foldout(position, unfolded, label);
		EditorGUI.EndProperty();

		position.y += 20;
		position.height = 20;

		if (unfolded) {
			for (int i = 0; i < statesProp.arraySize; i++) {
				EditorGUI.PropertyField(position, statesProp.GetArrayElementAtIndex(i).FindPropertyRelative("id"));
				position.y += 20;
			}
		}
	}
}
*/
