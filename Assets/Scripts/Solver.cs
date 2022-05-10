using UnityEngine;
using System;
using Kociemba;

public class Solver : MonoBehaviour
{
	public GameObject detector;

	public void getSolution(Action<string> callback) {
		string info;
		string output = SearchRunTime.solution(makeString(), out info,  buildTables: true);
		callback(output);
	}

	string makeString() {
		string s = "";

		for(int i = 1; i <= 6; i++) {
			Transform side = detector.transform.Find($"{i}");

			for(int j = 1; j <= 9; j++) {
				Transform piece = side.transform.Find($"{j}");

				GameObject face = getFace((a) => {
					return a.transform.position == piece.position;
				});

				Color c = face.GetComponent<MeshRenderer>().material.color;


				if(c == new Color(1, 1, 0, 1)) {
					s += "U";
				}
				else if(c == new Color(0, 0, 1, 1)) {
					s += "B";
				}
				else if(c == new Color(0, 1, 0, 1)) {
					s += "F";
				}
				else if(c == new Color(1, 0.235f, 0, 1)) {
					s += "R";
				}
				else if(c == new Color(1, 0, 0, 1)) {
					s += "L";
				}
				else if(c == new Color(1, 1, 1, 1)) {
					s += "D";
				}
			}
		}	

		return s;
	}

	GameObject getFace(Func<GameObject, bool> check) {
		foreach(Transform ts in gameObject.transform) {
			foreach(Transform t in ts) {
				bool a = check(t.gameObject);

				if(a) {
					return t.gameObject;
				}
			}
		}

		return null;
	}
}
