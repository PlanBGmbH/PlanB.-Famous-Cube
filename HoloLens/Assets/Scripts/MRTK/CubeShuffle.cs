using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Implements the shuffle logic
/// </summary>
public class CubeShuffle : MonoBehaviour
{
    /// <summary>
    /// Side movement working list
    /// </summary>
    public static List<string> moveList = new List<string>();

    /// <summary>
    /// Possible movements of the cube
    /// </summary>
    private readonly List<string> allMoves = new List<string>()
    {
        "U", "D", "L", "R", "F", "B",
        "U2", "D2", "L2", "R2", "F2", "B2",
        "U'", "D'", "L'", "R'", "F'", "B'"
    };

    /// <summary>
    /// Cube state object of the program
    /// </summary>
    private CubeState cubeState;

    /// <summary>
    /// Cube read object of the program
    /// </summary>
    private ReadCube readCube;

    // Start is called before the first frame update
    void Start()
    {
        cubeState = FindObjectOfType<CubeState>();
        readCube = FindObjectOfType<ReadCube>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveList.Count > 0 && !CubeState.autoRotating && CubeState.started)
        {
            DoMove(moveList[0]);

            moveList.Remove(moveList[0]);

        }
    }

    /// <summary>
    /// Start shuffle methode incl. genarting movement list -> working is in the update methode
    /// </summary>
    public void Shuffle()
    {
        int shuffleLen = 11;
        if (shuffleLen < 10)
        {
            //shuffleLength = UnityEngine.Random.Range(10, 30);
        }
        else if (shuffleLen > 10)
        {
            List<string> moves = new List<string>();
            //int shuffleLength = UnityEngine.Random.Range(0, shuffleLen);
            for (int i = 0; i < shuffleLen; i++)
            {
                int randomMove = UnityEngine.Random.Range(0, allMoves.Count);
                moves.Add(allMoves[randomMove]);
            }
            moveList = moves;
        }

    }

    /// <summary>
    /// Run a move of side
    /// </summary>
    /// <param name="move"></param>
    void DoMove(string move)
    {
        readCube.ReadState();
        CubeState.autoRotating = true;
        if (move == "U")
        {
            RotateSide(cubeState.up, -90);
        }
        if (move == "U'")
        {
            RotateSide(cubeState.up, 90);
        }
        if (move == "U2")
        {
            RotateSide(cubeState.up, -180);
        }
        if (move == "D")
        {
            RotateSide(cubeState.down, -90);
        }
        if (move == "D'")
        {
            RotateSide(cubeState.down, 90);
        }
        if (move == "D2")
        {
            RotateSide(cubeState.down, -180);
        }
        if (move == "L")
        {
            RotateSide(cubeState.left, -90);
        }
        if (move == "L'")
        {
            RotateSide(cubeState.left, 90);
        }
        if (move == "L2")
        {
            RotateSide(cubeState.left, -180);
        }
        if (move == "R")
        {
            RotateSide(cubeState.right, -90);
        }
        if (move == "R'")
        {
            RotateSide(cubeState.right, 90);
        }
        if (move == "R2")
        {
            RotateSide(cubeState.right, -180);
        }
        if (move == "F")
        {
            RotateSide(cubeState.front, -90);
        }
        if (move == "F'")
        {
            RotateSide(cubeState.front, 90);
        }
        if (move == "F2")
        {
            RotateSide(cubeState.front, -180);
        }
        if (move == "B")
        {
            RotateSide(cubeState.back, -90);
        }
        if (move == "B'")
        {
            RotateSide(cubeState.back, 90);
        }
        if (move == "B2")
        {
            RotateSide(cubeState.back, -180);
        }

    }

    /// <summary>
    /// Roate side of cube
    /// </summary>
    /// <param name="side">cube side</param>
    /// <param name="angle">Angle of movement</param>
    void RotateSide(List<GameObject> side, float angle)
    {
        PivotRotation pr = side[4].transform.parent.GetComponent<PivotRotation>();
        pr.StartAutoRotate(side, angle);
    }
}
