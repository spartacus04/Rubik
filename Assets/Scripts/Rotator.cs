using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Threading.Tasks;

public class Rotator : MonoBehaviour
{
	public List<GameObject> sides = new List<GameObject>();

	// Inizializzazione
	public void Start() {
		sides.Clear();
		
		foreach(Transform child in transform) {
			sides.Add(child.gameObject);
		}
	}

	// Interpretazione delle mosse
	public (int type, int clockwise, int count) getMove(string move) {
		int type;
		
		switch(move[0]) {
			case 'U':
				type = 0;
				break;
			case 'D':
				type = 1;
				break;
			case 'L':
				type = 2;
				break;
			case 'R':
				type = 3;
				break;
			case 'F':
				type = 4;
				break;
			case 'B':
				type = 5;
				break;
			default:
				type = -1;
				break;
		}
			
		if(move.Length == 1) return (type, 1, 1);

		if(move[1] == '2') {
			return (type, 1, 2);
		} else if(move[1] == '\'') {
			return (type, -1, 1);
		} else {
			return (type, 1, 1);
		}
	}

	// Scelta delle mosse da eseguire
	public async Task Rotate(string move, float duration = 0.5f) {
		var (type, clockwise, count) = getMove(move);

		if(type == -1) return;

			switch(type) {
				case 0:
					await RotateAroundPoint(new Vector3(0, 1, 0), count, duration, clockwise);
					break;
				case 1:
					await RotateAroundPoint(new Vector3(0, -1, 0), count, duration, clockwise);
					break;
				case 2:
					await RotateAroundPoint(new Vector3(-1, 0, 0), count, duration, clockwise);
					break;
				case 3:
					await RotateAroundPoint(new Vector3(1, 0, 0), count, duration, clockwise);
					break;
				case 4:
					await RotateAroundPoint(new Vector3(0, 0, -1), count, duration, clockwise);
					break;
				case 5:
					await RotateAroundPoint(new Vector3(0, 0, 1), count, duration, clockwise);
					break;
		}
	}

	public async Task RotateAroundPoint(Vector3 center, int times, float duration, int clockwise) {
		for (int i = 0; i < times; i++)
		{
			TaskCompletionSource<bool> promise = new TaskCompletionSource<bool>();
		
			GameObject[] objs = getGameObjectsInSide(center);

			if(duration == 0) {
				GameObject rotator = new GameObject("Rotator");
				rotator.transform.SetParent(transform);
				rotator.transform.localPosition = center;

				foreach(GameObject obj in objs) {
					obj.transform.parent = rotator.transform;
				}

				rotator.transform.Rotate(center * 90 * clockwise);

				foreach(GameObject obj in objs) {
					obj.transform.parent = transform;
					obj.transform.localPosition = new Vector3(Mathf.Round(obj.transform.localPosition.x), Mathf.Round(obj.transform.localPosition.y), Mathf.Round(obj.transform.localPosition.z));
				}

				Destroy(rotator);

				promise.SetResult(true);
			}
			else {
				StartCoroutine(LerpRotate(objs, center, 90 * clockwise * (1 / duration), duration, () => {
					promise.SetResult(true);
				}));
			}


			await promise.Task;
		}
	}

	// Restituisce tutti pezzi di una faccia data la posizione del pezzo centrale
	public GameObject[] getGameObjectsInSide(Vector3 v) {
		List<GameObject> objects = new List<GameObject>();
		if(v.x != 0) {
			foreach(GameObject obj in sides) {
				if(obj.transform.localPosition.x == v.x) {
					objects.Add(obj);
				}
			}
		}
		else if(v.y != 0) {
			foreach(GameObject obj in sides) {
				if(obj.transform.localPosition.y == v.y) {
					objects.Add(obj);
				}
			}
		}
		else if(v.z != 0) {
			foreach(GameObject obj in sides) {
				if(obj.transform.localPosition.z == v.z) {
					objects.Add(obj);
				}
			}
		}

		return objects.ToArray();
	}

	// Coroutine per ruotare una faccia
	IEnumerator LerpRotate(GameObject[] objects, Vector3 center, float angle, float duration, Action cb)
    {
		GameObject rotator = new GameObject("Rotator");
		rotator.transform.SetParent(transform);
		rotator.transform.localPosition = center;

		foreach(GameObject obj in objects) {
			obj.transform.parent = rotator.transform;
		}

		// Rotation
		float elapsedTime = 0;
		while (elapsedTime < duration)
		{
			rotator.transform.Rotate(center * angle * Time.deltaTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		
		// Rounding
		rotator.transform.rotation = new Quaternion(Mathf.Round(rotator.transform.rotation.x), Mathf.Round(rotator.transform.rotation.y), Mathf.Round(rotator.transform.rotation.z), Mathf.Round(rotator.transform.rotation.w));

		foreach(GameObject obj in objects) {
			obj.transform.parent = transform;
			obj.transform.localPosition = new Vector3(Mathf.Round(obj.transform.localPosition.x), Mathf.Round(obj.transform.localPosition.y), Mathf.Round(obj.transform.localPosition.z));
		}

		Destroy(rotator);

		cb();
	}
}
