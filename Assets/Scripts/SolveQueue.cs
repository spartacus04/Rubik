using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SolveQueue : MonoBehaviour
{
	[Range(0.1f, 1)]
	public float duration = 0.5f;
	public List<string> moves = new List<string>();

	private Rotator rotator;

	void Start() {
		rotator = GetComponent<Rotator>();
	}

	public async Task startMoves() {
		while(moves.Count > 0) {
			await rotator.Rotate(moves[0], duration);
			moves.RemoveAt(0);
		}
	}
}
