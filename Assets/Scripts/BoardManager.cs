using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager sharedInstance;
    public List<Sprite> prefabs = new List<Sprite>();
    public GameObject currentCandy;
    public int xSize, ySize;

    private GameObject[,] candies;

    public bool isShifting { get; set; }

    private Candy selectedCandy;

    public const int MinCandiesToMatch = 2;

    // Start is called before the first frame update
    void Start()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Vector2 offset = currentCandy.GetComponent<BoxCollider2D>().size;
        CreateInitialBoard(offset);

    }

    private void CreateInitialBoard(Vector2 offset)
    {
        candies = new GameObject[xSize, ySize];

        float startX = this.transform.position.x;
        float startY = this.transform.position.y;

        int idx = -1;

        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                GameObject newCandy = Instantiate(
                currentCandy,
                    new Vector3(startX + (offset.x * x),
                                startY + (offset.y * y),
                                0),
                    currentCandy.transform.rotation
                 );
                newCandy.name = string.Format("Candy[{0}][{1}]", x, y);

                do
                {
                    idx = Random.Range(0, prefabs.Count);
                } while ((x > 0 && idx == candies[x - 1, y].GetComponent<Candy>().id) ||
                        (y > 0 && idx == candies[x, y - 1].GetComponent<Candy>().id));



                Sprite sprite = prefabs[idx];
                newCandy.GetComponent<SpriteRenderer>().sprite = sprite;
                newCandy.GetComponent<Candy>().id = idx;

                newCandy.transform.parent = this.transform;
                candies[x, y] = newCandy;

            }
        }
    }

    public IEnumerator FindNullCandies()
    {
        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                if(candies[x,y].GetComponent<SpriteRenderer>().sprite == null)
                {
                    yield return StartCoroutine(MakeCandiesFall(x, y));
                    break;
                }
            }
        }


        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                candies[x, y].GetComponent<Candy>().FindAllMatches();
            }
        }

    }

    private IEnumerator MakeCandiesFall(int x, int yStart,
                                        float shiftDelay = 0.05f)
    {

        isShifting = true;

        List<SpriteRenderer> renderes = new List<SpriteRenderer>();
        int nullCandies = 0;

        for(int y = yStart; y < ySize; y++)
        {
            SpriteRenderer spriteRenderer = candies[x, y].GetComponent<SpriteRenderer>();
            if (spriteRenderer.sprite == null)
            {
                nullCandies++;
            }
            renderes.Add(spriteRenderer);
        }

        for (int i = 0; i < nullCandies; i++)
        {
            GUIManager.sharedInstance.Score += 10;

            yield return new WaitForSeconds(shiftDelay);
            for(int j = 0; j < renderes.Count - 1; j++)
            {
                renderes[j].sprite = renderes[j + 1].sprite;
                renderes[j + 1].sprite = GetNewCandy(x, ySize-1);
            }
        }

        isShifting = false;
    }

    private Sprite GetNewCandy(int x, int y)
    {
        List<Sprite> possibleCandies = new List<Sprite>();
        possibleCandies.AddRange(prefabs);

        if(x>0)
        {
            possibleCandies.Remove(
                candies[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (x < xSize - 1)
        {
            possibleCandies.Remove(
                candies[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (y > 0)
        {
            possibleCandies.Remove(
                candies[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }

        return possibleCandies[Random.Range(0, possibleCandies.Count)];
    }

}
