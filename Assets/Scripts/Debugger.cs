using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UI;

public class Debugger : MonoBehaviour
{
	public string move;
	private Rotator rotator;
	private SolveQueue solveQueue;
	[HideInInspector]

	private bool resetText = false;
	public bool isMoving = true;
	public Text text;
	public Text inp;
	public Text outp;
	public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        rotator = GetComponent<Rotator>();
		solveQueue = GetComponent<SolveQueue>();
    }

	public async void rotatorSolve() {
		if(!isMoving) return;

		isMoving = false;
		TaskCompletionSource<string> promise = new TaskCompletionSource<string>();

		GetComponent<Solver>().getSolution((output) => {
			promise.SetResult(output);
			outp.text += output;
			resetText = true;
			Debug.Log("Solve: " + output); 
		});

		string output = await promise.Task;

		List<string> moves = output.Split(' ').ToList();
		moves.RemoveAt(moves.Count - 1);

		solveQueue.moves = moves;
		await solveQueue.startMoves();
		isMoving = true;
	}

	public async void scramble() {

		if(resetText) {
			inp.text = "Scramble: \n";
			outp.text = "Solve: \n";
			resetText = false;
		}

		if(!isMoving) return;

		isMoving = false;
		List<string> temp = solveQueue.moves;
		List<string> scramble = new List<string>();

		string getRandomMove() {
			string[] moves = { "U", "D", "L", "R", "F", "B" };
			string[] modifier = { "", "'", "2" };

			string move = $"{moves[Random.Range(0, 6)]}{modifier[Random.Range(0, 3)]}";

			return move;
		}

		Random.InitState(System.DateTime.Now.Millisecond);
		int times = Random.Range(10, 20);


		string lastMove = "*";
		for(int i = 0; i < times; i++) {
			string move = getRandomMove();

			if(move.StartsWith(new string(lastMove[0], 1))) {
				i--;
				continue;
			}

			scramble.Add(move);
			lastMove = move;
		}

		inp.text += string.Join(" ", scramble);

		solveQueue.moves = scramble;

		await solveQueue.startMoves();

		solveQueue.moves = temp;
		isMoving = true;
	}

    // Update is called once per frame
    async void Update()
    {
		if(!isMoving) return;

		isMoving = false;
        if(move != "") {
			await rotator.Rotate(move, 1);
			inp.text += move;
			move = "";
		}

		isMoving = true;
    }

	public void setMove() {
		move = text.text;
	}

	public void setSpeed() {
		solveQueue.duration = slider.value;
	}
}