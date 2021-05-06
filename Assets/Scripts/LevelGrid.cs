using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class LevelGrid
{
    public int minWidth = -20;
    public int maxWidth = 20; // Yhden lisää jotta käärme voi syntyä toiselta puolelta
    public int minHeight = -15;
    public int maxHeight = 15; // Yhden lisää jotta käärme voi syntyä toiselta puolelta

    // LevelGrid saa kentän koon parametreina
    public LevelGrid(int minWidth, int maxWidth, int minHeight, int maxHeight)
    {
        this.minWidth = minWidth;
        this.maxWidth = maxWidth;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
    }
    // Käärme liikkuu ruudun läpi
    public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
    {
        // jos käärme menee leveyssuunnassa min rajan yli
        if (gridPosition.x <= minWidth)
        {
            gridPosition.x = maxWidth - 1; // täytyy miinustaa jotta käärme tulisi toiselta puolelta läpi
        }
        // jos käärme menee leveyssuunnassa max rajan yli
        if (gridPosition.x >= maxWidth)
        {
            gridPosition.x = minWidth;
        }
        // jos käärme menee pituussuunnassa min rajan yli
        if (gridPosition.y <= minHeight)
        {
            gridPosition.y = maxHeight - 1; // täytyy miinustaa jotta käärme tulisi toiselta puolelta läpi
        }
        // jos käärme menee pituussuunnassa max rajan yli
        if (gridPosition.y >= maxHeight)
        {
            gridPosition.y = minHeight;
        }
        return gridPosition;
    }
}
