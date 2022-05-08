using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager manager;

	[SerializeField]
	private Camera mainCamera;
	[SerializeField]
	private GameObject selection;

	[SerializeField]
	public int[,,] grid;
	[SerializeField]
	private Color[] colors = new Color[6];

	private bool isMoving = false;
	private GameObject objectToMove;
	private PathFinding pathFinder;

	public Text scoreText;
	private int points = 0;
	private int[] previewColors = new int[3];
	private int[] previewTypes = new int[3];
	List<GameObject> previewBalls = new List<GameObject>();

	public GameObject explosion;

	public Image fadedBackground;
	public Text finalScore;
	public Text highScore;
	public Button btnRestart;


	private void Awake()
	{
		manager = this;
		GenerateGrid();
		PreviewBall();
	}
	void Start()
	{
		pathFinder = new PathFinding();
		highScore.text = "Highscore: " + PlayerPrefs.GetInt("Highscore");
		SpawnBall();
	}

	private void Update()
	{
		BallControl();
	}

	void PreviewBall()
	{
		if(ObjectPool.instance.GetPooledObject() == null)
		{
			EndGame();
		}
		for (int i = 0; i < 3; i++)
		{
			previewColors[i] = Random.Range(0, 6);
			int type = Random.Range(0, 10);
			if(type < 3){ previewTypes[i] = 0; } else { previewTypes[i] = -1; }

		}

		for (int i = 0; i < 3; i++)
		{
			GameObject ball = ObjectPool.instance.GetPooledObject();
			if (ball == null) return;

			ball.transform.position = new Vector2(3f + i * 1.5f, 10f);
			ball.transform.GetChild(0).GetComponent<SpriteRenderer>().color = colors[previewColors[i]];
			if(previewTypes[i] == 0)
			{
				ball.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
			}
			else
			{
				ball.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
			}
			ball.SetActive(true);
			previewBalls.Add(ball);
		}
	}

	void SpawnBall()
	{
		for(int i = 0; i < previewBalls.Count; i++)
		{
			int x, y;
			do
			{
				x = Random.Range(0, 9);
				y = Random.Range(0, 9);
			}
			while (grid[x, y, 0] != -1);

			previewBalls[i].transform.position = new Vector2(x + 0.5f, y + 0.5f);
			grid[x, y, 0] = previewColors[i];
			grid[x, y, 1] = previewTypes[i];

			UpdatePathFindingGrid();
			PointUpdate(x, y, grid[x, y, 0]);
		}
		previewBalls.Clear();
		PreviewBall();
	}

	void BallControl()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
			mouseWorldPos.z = 0f;
			/*Debug.Log(mouseWorldPos);*/
			if (mouseWorldPos.x > 0 && mouseWorldPos.x < 9 && mouseWorldPos.y > 0 && mouseWorldPos.y < 9)
			{
				int x = Mathf.FloorToInt(mouseWorldPos.x);
				int y = Mathf.FloorToInt(mouseWorldPos.y);

				Vector2 position = new Vector2(x + 0.5f, y + 0.5f);
				RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, 0f);

				if (objectToMove == null && isMoving == false)
				{
					if (hit.collider != null && hit.transform.name == "Ball child")
					{
						objectToMove = hit.transform.parent.gameObject;
						GameObject tmp = Instantiate(selection, position, Quaternion.identity);
						tmp.transform.parent = objectToMove.transform;
					}
					
				}
				else if(hit.collider != null)
				{
					Destroy(objectToMove.transform.GetChild(1).gameObject);
					objectToMove = null;
					isMoving = false;
				}
				else if (hit.collider == null)
				{
					isMoving = true;
					int[] startPos = { Mathf.FloorToInt(objectToMove.transform.position.x), Mathf.FloorToInt(objectToMove.transform.position.y) };
					List<Tile> path = pathFinder.FindPath(startPos[0], startPos[1], x, y);

					Destroy(objectToMove.transform.GetChild(1).gameObject);
					if (path == null)
					{
						Debug.Log("Cant find path");
						objectToMove = null;
						isMoving = false;
						return;
					}

					grid[x, y, 0] = grid[startPos[0], startPos[1], 0];
					grid[startPos[0], startPos[1], 0] = -1;
					grid[startPos[0], startPos[1], 1] = -1;
					StartCoroutine(Move(path, x, y));
				}
				

			}
		}
	}

	private void BallMove(Vector2 position)
	{
		
		if (isMoving && (Vector2)objectToMove.transform.position != position)
		{
			objectToMove.transform.position = Vector2.Lerp(position, objectToMove.transform.position, 0.5f * Time.deltaTime);
		}
	}

	private void GenerateGrid()
	{
		grid = new int[9, 9, 2];
		for (int x = 0; x < 9; x++)
		{
			for (int y = 0; y < 9; y++)
			{
				grid[x, y, 0] = -1;
				grid[x, y, 1] = -1;
			}
		}
	}

	IEnumerator Move(List<Tile> path, int endX, int endY)
	{
		int i = 0;
		while (i < path.Count)
		{
			BallMove(new Vector2(path[i].x + 0.5f, path[i].y + 0.5f));
			i++;
			yield return new WaitForSeconds(0.02f);
		}
		objectToMove = null;
		isMoving = false;

		UpdatePathFindingGrid();
		PointUpdate(endX, endY, grid[endX, endY, 0]);
		SpawnBall();
	}
	
	void UpdatePathFindingGrid()
	{
		for( int x = 0; x < 9; x ++)
		{
			for(int y = 0; y < 9; y ++)
			{
				pathFinder.grid[x, y].colorCode = grid[x, y, 0];
				pathFinder.grid[x, y].typeCode = grid[x, y, 1];
			}
		}
	}

	void PointUpdate(int x , int y, int c)
	{
		List<int[]> vLine = new List<int[]>();
		List<int[]> hLine = new List<int[]>();
		List<int[]> d1Line = new List<int[]>();
		List<int[]> d2Line = new List<int[]>();

		#region get lines
		for (int i = 0; i < 9; i++)
		{

			if (grid[x, i, 0] == c)
				vLine.Add(new int[] { x, i });
			else if (vLine.Count < 5)
				vLine.Clear();

			if (grid[i, y, 0] == c)
				hLine.Add(new int[] { i, y });
			else if (hLine.Count < 5)
				hLine.Clear();
		}

		for (int i = 0; i < 9; i++)
		{
			if(x - i > -1 && y - i > -1)
			{
				if (grid[x - i, y - i, 0] == c) { d1Line.Add(new int[] { x - i, y - i }); } else { break; }
			}
		}
		for (int i = 1; i < 9; i++)
		{
			if (x + i < 9 && y + i < 9)
			{
				if (grid[x + i, y + i, 0] == c) { d1Line.Add(new int[] { x + i, y + i }); } else { break; }
			}
		}

		for (int i = 0; i < 9; i++)
		{
			if (x - i > -1 && y + i < 9)
			{
				if (grid[x - i, y + i, 0] == c) { d2Line.Add(new int[] { x - i, y + i }); } else { break; }
			}
		}
		for (int i = 1; i < 9; i++)
		{
			if (x + i < 9 && y - i > -1)
			{
				if (grid[x + i, y - i, 0] == c) { d2Line.Add(new int[] { x + i, y - i }); } else { break; }
			}
		}
		#endregion
		#region add point
		if (vLine.Count >= 5)
		{

			Debug.Log("vline " + vLine.Count);
			points += vLine.Count * 10;
			scoreText.text = "Score: " + points;
			for(int i = 0; i < vLine.Count; i ++)
			{
				DestroyBall(vLine[i][0], vLine[i][1]);
			}
			vLine.Clear();
		}

		if (hLine.Count >= 5)
		{
			Debug.Log("hline " + hLine.Count);
			points += hLine.Count * 10;
			scoreText.text = "Score: " + points;
			for (int i = 0; i < hLine.Count; i++)
			{
				DestroyBall(hLine[i][0], hLine[i][1]);
			}
			hLine.Clear();
		}

		if (d1Line.Count >= 5)
		{
			Debug.Log("d1line " + d1Line.Count);
			points += d1Line.Count * 10;
			scoreText.text = "Score: " + points;
			for (int i = 0; i < d1Line.Count; i++)
			{
				DestroyBall(d1Line[i][0], d1Line[i][1]);
			}
			d1Line.Clear();
		}

		if (d2Line.Count >= 5)
		{
			Debug.Log("d2line " + d2Line.Count);
			points += d2Line.Count * 10;
			scoreText.text = "Score: " + points;
			for (int i = 0; i < d2Line.Count; i++)
			{
				DestroyBall(d2Line[i][0], d2Line[i][1]);
			}
			d2Line.Clear();
		}
		#endregion
	}

	private void DestroyBall(int x, int y)
	{
		Vector2 position = new Vector2(x + 0.5f, y + 0.5f);
		RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, 0f);
		
		if(hit.collider != null)
		{
			hit.transform.GetChild(0).gameObject.SetActive(false);
			hit.transform.parent.gameObject.SetActive(false);
			
			grid[x, y, 0] = -1;
			grid[x, y, 1] = -1;
			UpdatePathFindingGrid();
			GameObject tmp = Instantiate(explosion, position, Quaternion.identity);
			Destroy(tmp, 0.5f);

		}
	}

	void EndGame()
	{
		fadedBackground.gameObject.SetActive(true);
		finalScore.text = "YOUR SCORE \n" + points.ToString();
		if(points > PlayerPrefs.GetInt("Highscore"))
		{
			PlayerPrefs.SetInt("Highscore", points);
		}
		finalScore.gameObject.SetActive(true);
		btnRestart.gameObject.SetActive(true);
	}

	IEnumerator Wait(float time)
	{
		yield return new WaitForSeconds(time);
	}
}
