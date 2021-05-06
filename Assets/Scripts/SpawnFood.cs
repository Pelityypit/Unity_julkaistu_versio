using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour
{
    private Vector2Int foodGridPosition;
    private GameObject foodGameObject;
    public GameObject SnakeObj;
    private int minWidth = -20;
    private int maxWidth = 20;
    private int minHeight = -15;
    private int maxHeight = 15;
    private int rand;
    public void SpawnFoods()
    {
        rand = Random.Range(0, GameAssets.instance.foodSprite.Length);
        do
        {
            // satunnaiset sijainnit x ja y akseleilla pelikentällä
            foodGridPosition = new Vector2Int(Random.Range(minWidth + 1, maxWidth - 1), Random.Range(minHeight + 1, maxHeight - 1));
            // Käärmeen pään ja kehon päälle ei ilmesty ruokaa
        } while (SnakeObj.GetComponent<Snake>().GetFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1);
        // luodaan uusi peliobjekti "food", annetaan typeofilla sille spriterenderer komponentti
        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        // haetaan objektille gameassetsin instancesta foodsprite
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.foodSprite[rand];
        // määritetään foodgameobjektille sijainti pelikentällä
        foodGameObject.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y);
    }

    // verrataan käärmeen ja hedelmän sijainteja
    // jos sijainnit samat, tuhotaan foodgameobject ja spawnataan uusi
    public bool TrySnakeEatFood(Vector2Int snakeGridPosition)
    {
        if (snakeGridPosition == foodGridPosition)
        {
            Object.Destroy(foodGameObject);
            SpawnFoods();
            Score.AddScore();
            return true;
        }
        else
        {
            return false;
        }
    }
}
