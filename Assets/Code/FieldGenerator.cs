using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    private GameObject dotPref;
    private Sprite[] candies;
    private FieldCell[,] gameField;
    private int fieldSize;
    private float dotSize;

    [SerializeField]
    GameObject fieldCellPrefab;
    private GameManager gm;

    public FieldCell[,] GenerateField(
        GameManager gm,
        int size,
        Sprite[] candies,
        GameObject dotPrefab
    )
    {
        this.gm = gm;
        this.dotPref = dotPrefab;
        this.candies = candies;
        RectTransform fieldTransform = GetComponent<RectTransform>();
        this.fieldSize = size;
        gameField = new FieldCell[fieldSize, fieldSize];
        dotSize = fieldTransform.sizeDelta.x / (fieldSize * 1.2f);
        Vector2 placePos = new Vector2(0, dotSize * ((fieldSize - 1f) * 0.6f));

        for (int y = 0; y < size; y++)
        {
            placePos.x = dotSize * ((fieldSize - 1f) * -0.6f);
            for (int x = 0; x < size; x++)
            {
                gameField[x, y] = Instantiate(fieldCellPrefab, fieldTransform)
                    .GetComponent<FieldCell>();
                gameField[x, y].SetPosition(placePos, new Vector2(x, y));
                gameField[x, y].gameObject.name = "[" + x + " " + y + "]";
                Instantiate(dotPrefab, gameField[x, y].CellTransform);
                Sprite candy = this.candies[Random.Range(0, this.candies.Length)];
                gameField[x, y].Dot.CreateDot(
                    dotSize,
                    new Vector2(x, y),
                    candy,
                    gm.GetPopColor(candy)
                );
                placePos.x += dotSize * 1.2f;
            }
            placePos.y -= dotSize * 1.2f;
        }

        return gameField;
    }

    public void FillEmptySpaces()
    {
        int[] emptySpacesInColumns = CountEmptySpaces();
        StartCoroutine(MoveDownAndCreateNewDots(emptySpacesInColumns));
    }

    private int[] CountEmptySpaces()
    {
        int[] empties = new int[fieldSize];
        int emptycount = 0;
        for (int x = 0; x < fieldSize; x++)
        {
            emptycount = 0;
            for (int y = 0; y < fieldSize; y++)
            {
                if (gameField[x, y].IsEmpty)
                {
                    emptycount++;
                }
            }
            empties[x] = emptycount;
        }
        return empties;
    }

    IEnumerator MoveDownAndCreateNewDots(int[] emptySpaces)
    {
        for (int x = 0; x < fieldSize; x++)
        {
            if (emptySpaces[x] > 0)
            {
                for (int y = fieldSize - 2; y >= 0; y--)
                {
                    if (!gameField[x, y].IsEmpty)
                    {
                        SetEmptyCellToMoveDown(gameField[x, y].Dot, x, y);
                        yield return null;
                    }
                }
                CreateNewDots(x, emptySpaces[x]);
            }
        }
    }

    private void SetEmptyCellToMoveDown(Dot dot, int x, int y)
    {
        int posInColumn = y;
        if (dot.IsMovingDown)
        {
            return;
        }
        while (true)
        {
            posInColumn++;
            if (posInColumn == fieldSize)
            {
                return;
            }
            if (gameField[x, posInColumn].IsEmpty)
            {
                dot.Cell = gameField[x, posInColumn].CellTransform;
            }
            else
            {
                return;
            }
        }
    }

    private void CreateNewDots(int column, int count)
    {
        Vector2 spawnPos = new Vector2(0, dotSize * 2);
        int emptyCell = count - 1;
        for (int i = 0; i < count; i++)
        {
            Dot tempDot = Instantiate(dotPref, gameField[column, 0].CellTransform)
                .GetComponent<Dot>();
            tempDot.GetComponent<RectTransform>().anchoredPosition = spawnPos;
            Sprite candy = candies[Random.Range(0, candies.Length)];
            tempDot.CreateDot(
                dotSize,
                candy,
                gameField[column, emptyCell].FieldPosition,
                gameField[column, emptyCell].CellTransform,
                gm.GetPopColor(candy)
            );
            spawnPos.y += dotSize * 1.2f;
            emptyCell--;
        }
    }
}
