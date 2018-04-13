using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Constants
{
    public const int TILE_SCREEN_WIDTH = 450;
    public const int TILE_SCREEN_HEIGHT = 450;

    public const int WIDTH = 9;
    public const int HEIGHT = 9;

    public const int TILE_WIDTH = TILE_SCREEN_WIDTH / WIDTH;
    public const int TILE_HEIGHT = TILE_SCREEN_HEIGHT / HEIGHT;

    public const int SCORE_NUM = 10;
}


// match 게임에 대한 로직
public class TileGridPanel : MonoBehaviour {
    

    const int WIDTH = Constants.WIDTH;
    const int HEIGHT = Constants.HEIGHT;

    const int TILE_WIDTH = Constants.TILE_WIDTH;
    const int TILE_HEIGHT = Constants.TILE_HEIGHT;


    const int SCORE_NUM = Constants.SCORE_NUM;

    public List<GameObject> tileList;
    Tile[,] tileGrid = new Tile[Constants.HEIGHT, Constants.WIDTH];



    // 전체 타일을 랜덤하게 생성하는 함수
    // 최초 시작시와 매칭 할 수 있는게 없을 경우 호출한다.
    public void MakeAllTile()
    {
        Transform[] childrens = GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < childrens.Length; i++)
        {
            if (childrens[i] != transform)
            {
                PoolManager.Instance.pushObject("Tile", childrens[i].gameObject, null);
            }
        }
        for (int i = 0; i < Constants.HEIGHT; i++)
        {
            for (int j = 0; j < Constants.WIDTH; j++)
            {
                int index = Random.Range(0, tileList.Count);

                GameObject obj = PoolManager.Instance.getObject("Tile", null);
                obj.transform.SetParent(transform);
                obj.GetComponent<TileObject>().SetTile(tileList[index], i, j);

                RectTransform objTransform = obj.transform as RectTransform;
                objTransform.sizeDelta = new Vector2(Constants.TILE_WIDTH, Constants.TILE_HEIGHT);
                objTransform.localPosition = new Vector3(Constants.TILE_WIDTH * 0.5f + Constants.TILE_WIDTH * j, Constants.TILE_HEIGHT * -0.5f - TILE_HEIGHT * i, 0);

                obj.SetActive(true);

                tileGrid[i, j].index = index;
                tileGrid[i, j].gameObject = obj;


            }

        }

    }

    // 모든 타일에 대해 매칭 판별하는 함수
    // 매칭에 해당하는 모든 리스트를 반환한다.
    public List<TileLine> CheckAllMatch()
    {
        List<TileLine> matchLines = new List<TileLine>();
        for (int i = 0; i < Constants.HEIGHT; i++)
        {
            int beforeTile = -1;
            int count = 0;
            int startIndex = -1;
            for (int j = 0; j < Constants.WIDTH; j++)
            {
                if (beforeTile == tileGrid[i, j].index)
                {
                    if (count == 0)
                    {
                        startIndex = j - 1;
                    }
                    count++;
                }
                else
                {
                    if (count >= 2)
                    {
                        TileLine line = new TileLine(false, i, startIndex, j - 1);

                        matchLines.Add(line);
                    }
                    count = 0;
                    startIndex = -1;
                }
                beforeTile = tileGrid[i, j].index;

            }
            // 맨 마지막 타일을 위한 예외처리.
            if (count >= 2)
            {
                TileLine line = new TileLine(false, i, startIndex, WIDTH - 1);
                matchLines.Add(line);
            }
        }

        for (int i = 0; i < Constants.WIDTH; i++)
        {
            int beforeTile = -1;
            int count = 0;
            int startIndex = -1;
            for (int j = 0; j < Constants.HEIGHT; j++)
            {
                if (beforeTile == tileGrid[j, i].index)
                {
                    if (count == 0)
                    {
                        startIndex = j - 1;
                    }

                    count++;

                }
                else
                {
                    if (count >= 2)
                    {
                        TileLine line = new TileLine(true, i, startIndex, j - 1);

                        matchLines.Add(line);
                    }
                    count = 0;
                    startIndex = -1;

                }


                beforeTile = tileGrid[j, i].index;

            }
            if (count >= 2)
            {
                TileLine line = new TileLine(true, i, startIndex, Constants.HEIGHT - 1);

                matchLines.Add(line);
            }
        }
        return matchLines;
    }

    // 매칭된 타일을 제거하는 함수
    public void RemoveMatchTile( List<TileLine> _tileLines,int _comboCount,ref int _score)
    {
        for (int i = 0; i < _tileLines.Count; i++)
        {
            int lineIndex = _tileLines[i].lineIndex;
            //Debug.Log(_tileLines[i].isVertical + " " + _tileLines[i].lineIndex + " " + _tileLines[i].startIndex + " " + _tileLines[i].endIndex);
            if (_tileLines[i].isVertical)
            {
                for (int j = _tileLines[i].startIndex; j <= _tileLines[i].endIndex; j++)
                {
                    if (tileGrid[j, lineIndex].index != -1)
                    {
                        _score += SCORE_NUM * (_comboCount + 1);
                        
                        tileGrid[j, lineIndex].index = -1;

                        GameObject particleObj = PoolManager.Instance.getObject("Particle", null);
                        particleObj.transform.position = tileGrid[j, lineIndex].gameObject.transform.position;
                        particleObj.SetActive(true);
                        PoolManager.Instance.pushObject("Tile", tileGrid[j, lineIndex].gameObject, null);

                    }
                }
            }
            else
            {
                for (int j = _tileLines[i].startIndex; j <= _tileLines[i].endIndex; j++)
                {
                    if (tileGrid[lineIndex, j].index != -1)
                    {
                        _score += SCORE_NUM * (_comboCount + 1);
                        
                        tileGrid[lineIndex, j].index = -1;

                        GameObject particleObj = PoolManager.Instance.getObject("Particle", null);
                        particleObj.transform.position = tileGrid[lineIndex, j].gameObject.transform.position;
                        particleObj.SetActive(true);
                        PoolManager.Instance.pushObject("Tile", tileGrid[lineIndex, j].gameObject, null);

                    }
                }
            }
        }
        
    }

    // 제거된 타일의 빈공간을 채우는 함수.
    public List<TileObject> RefillTile( List<TileLine> _tileLines)
    {
        List<TileObject> refillTileList = new List<TileObject>();
        //리필 할 세로 라인 영역 판별
        bool[] refillArray = new bool[WIDTH];
        for (int i = 0; i < _tileLines.Count; i++)
        {
            if (_tileLines[i].isVertical)
            {
                refillArray[_tileLines[i].lineIndex] = true;
            }
            else
            {
                for (int j = _tileLines[i].startIndex; j <= _tileLines[i].endIndex; j++)
                {
                    refillArray[j] = true;
                }
            }
        }
        // 리필 할 영역에서 타일들을 밑으로 내려 빈 공간을 메꾼다.
        int[] spaceArray = new int[WIDTH];
        for (int i = 0; i < WIDTH; i++)
        {
            if (refillArray[i])
            {
                int space = 0;
                for (int j = HEIGHT - 1; j >= 0; j--)
                {
                    if (tileGrid[j, i].index == -1)
                    {
                        space++;
                    }
                    else
                    {
                        if (space > 0)
                        {
                            tileGrid[j + space, i] = tileGrid[j, i];
                            tileGrid[j, i].index = -1;

                            TileObject tempTileObject = tileGrid[j + space, i].gameObject.GetComponent<TileObject>();
                            tempTileObject.setTilePos(j + space, i);

                            refillTileList.Add(tempTileObject);
                        }
                    }
                }
                spaceArray[i] = space;
            }
        }
        // 메꾸고 남은 최상단 공간에 새로운 타일을 생성한다.
        for (int i = 0; i < WIDTH; i++)
        {
            if (refillArray[i])
            {
                for (int j = 0; j < spaceArray[i]; j++)
                {
                    if (tileGrid[j, i].index == -1)
                    {
                        int index = Random.Range(0, tileList.Count);

                        GameObject obj = PoolManager.Instance.getObject("Tile", null);
                        obj.transform.SetParent(transform);
                        TileObject tempTileObject = obj.GetComponent<TileObject>();
                        tempTileObject.SetTile(tileList[index], j, i);

                        RectTransform objTransform = obj.transform as RectTransform;
                        objTransform.sizeDelta = new Vector2(TILE_WIDTH, TILE_HEIGHT);
                        objTransform.localPosition = new Vector3(TILE_WIDTH * 0.5f + TILE_WIDTH * i, TILE_HEIGHT * 0.5f + TILE_HEIGHT * (spaceArray[i] - 1 - j), 0);

                        obj.SetActive(true);

                        tileGrid[j, i].index = index;
                        tileGrid[j, i].gameObject = obj;


                        refillTileList.Add(tempTileObject);

                    }
                }

            }
        }
        return refillTileList;
    }

    // 채워진 타일이 매칭 가능한 패널이 존재하는지 확인.
    // 존재하지 않는다면 타일 생성을 다시해야한다.
    // 존재여부만 확인하면되니 일치하는게 있을시 바로 리턴한다.
    // 만약 힌트를 사용할거라면 List로 반환하도록 한다.
    public bool CheckCanMatch()
    {
        // 0~5 , 6~7
        IntVector2[] checkPosition =
        {
            new IntVector2(-2,-1),
            new IntVector2(-3,0),
            new IntVector2(-2,1),
            new IntVector2(1,-1),
            new IntVector2(2,0),
            new IntVector2(1,1),
            new IntVector2(-1,-1),
            new IntVector2(-1,1)
        };



        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 1; j < WIDTH; j++)
            {
                if (tileGrid[i, j - 1].index == tileGrid[i, j].index)
                {

                    for (int k = 0; k < 6; k++)
                    {
                        int deltaX = checkPosition[k].x + j;
                        int deltaY = checkPosition[k].y + i;

                        if ((deltaX >= 0) && (deltaY >= 0) && (deltaX < WIDTH) && (deltaY < HEIGHT))
                        {
                            if (tileGrid[deltaY, deltaX].index == tileGrid[i, j].index)
                            {
                               // Debug.Log("type=1 x = " + j + " y = " + i + " checkPositionX = " + checkPosition[k].x + " checkPositionY = " + checkPosition[k].y);
                                return true;
                            }
                        }
                    }

                }
                if ((j - 2) >= 0)
                {
                    if (tileGrid[i, j - 2].index == tileGrid[i, j].index)
                    {
                        for (int k = 6; k < 8; k++)
                        {
                            int deltaX = checkPosition[k].x + j;
                            int deltaY = checkPosition[k].y + i;

                            if ((deltaX >= 0) && (deltaY >= 0) && (deltaX < WIDTH) && (deltaY < HEIGHT))
                            {
                                if (tileGrid[deltaY, deltaX].index == tileGrid[i, j].index)
                                {

                                    //Debug.Log("type=2 x = " + j + " y = " + i + " checkPositionX = " + checkPosition[k].x + " checkPositionY = " + checkPosition[k].y);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

        }

        for (int i = 0; i < WIDTH; i++)
        {
            for (int j = 1; j < HEIGHT; j++)
            {
                if (tileGrid[j - 1, i].index == tileGrid[j, i].index)
                {

                    for (int k = 0; k < 6; k++)
                    {
                        int deltaX = checkPosition[k].y + i;
                        int deltaY = checkPosition[k].x + j;

                        if ((deltaX >= 0) && (deltaY >= 0) && (deltaX < WIDTH) && (deltaY < HEIGHT))
                        {
                            if (tileGrid[deltaY, deltaX].index == tileGrid[j, i].index)
                            {
                                //Debug.Log("type=3 x = " + i + " y = " + j + " checkPositionX = " + checkPosition[k].x + " checkPositionY = " + checkPosition[k].y);
                                return true;
                            }
                        }
                    }

                }
                if ((j - 2) >= 0)
                {
                    if (tileGrid[j - 2, i].index == tileGrid[j, i].index)
                    {
                        for (int k = 6; k < 8; k++)
                        {
                            int deltaX = checkPosition[k].y + i;
                            int deltaY = checkPosition[k].x + j;

                            if ((deltaX >= 0) && (deltaY >= 0) && (deltaX < WIDTH) && (deltaY < HEIGHT))
                            {
                                if (tileGrid[deltaY, deltaX].index == tileGrid[j, i].index)
                                {

                                   // Debug.Log("type=4 x = " + i + " y = " + j + " checkPositionX = " + checkPosition[k].x + " checkPositionY = " + checkPosition[k].y);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

        }
        return false;
    }

    // 특정 위치에 있는 타일에 대해 4방향으로 매칭이 있는지 확인하는 함수.
    // 유저가 swap 시 swap된 두 타일에 대해 사용한다.
    public List<TileLine> CheckOneMatch( int _x, int _y)
    {
        List<TileLine> line = new List<TileLine>();
        IntVector2[] checkPosition =
        {
            new IntVector2(-1,0),
            new IntVector2(1,0),
            new IntVector2(0,-1),
            new IntVector2(0,1)
        };
        int[] dircetCount = { 0, 0, 0, 0 };

        for (int i = 0; i < 4; i++)
        {

            int deltaX = checkPosition[i].x + _x;
            int deltaY = checkPosition[i].y + _y;

            while (true)
            {
                if ((deltaX < 0) || (deltaY < 0) || (deltaX >= WIDTH) || (deltaY >= HEIGHT))
                {
                    break;
                }
                if (tileGrid[_y, _x].index == tileGrid[deltaY, deltaX].index)
                {
                    dircetCount[i]++;
                }
                else
                {
                    break;

                }
                deltaX += checkPosition[i].x;
                deltaY += checkPosition[i].y;

            }


        }
        if ((dircetCount[0] + dircetCount[1]) >= 2)
        {
            line.Add(new TileLine(false, _y, (_x - dircetCount[0]), (_x + dircetCount[1])));
        }
        if ((dircetCount[2] + dircetCount[3]) >= 2)
        {
            line.Add(new TileLine(true, _x, (_y - dircetCount[2]), (_y + dircetCount[3])));
        }
        /*
        for (int i = 0; i < line.Count; i++)
        {
            Debug.Log(line[i].isVertical + " " + line[i].lineIndex + " " + line[i].startIndex + " " + line[i].endIndex + " ");
        }
        */
        return line;
    }

    public void SwapTile( IntVector2 _tile1, IntVector2 _tile2)
    {
        Tile temp = tileGrid[_tile1.y, _tile1.x];
        tileGrid[_tile1.y, _tile1.x] = tileGrid[_tile2.y, _tile2.x];
        tileGrid[_tile2.y, _tile2.x] = temp;

        tileGrid[_tile1.y, _tile1.x].gameObject.GetComponent<TileObject>().setTilePos(_tile1.y, _tile1.x);
        tileGrid[_tile2.y, _tile2.x].gameObject.GetComponent<TileObject>().setTilePos(_tile2.y, _tile2.x);
    }

    public GameObject GetTileGameObject(IntVector2 _tile)
    {
        return tileGrid[_tile.y, _tile.x].gameObject;

    }


}
