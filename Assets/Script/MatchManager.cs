using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Phase
{
    Normal,
    Change,
    Pause
}

public struct TileLine
{
    public bool isVertical;
    public int lineIndex;
    public int startIndex;
    public int endIndex;

    public TileLine(bool _isVertical, int _lineIndex, int _startIndex, int _endIndex)
    {
        isVertical = _isVertical;
        lineIndex = _lineIndex;
        startIndex = _startIndex;
        endIndex = _endIndex;
    }
}
public struct Tile
{
    public int index;
    public GameObject gameObject;
}

public struct IntVector2
{
    public int x;
    public int y;

    public IntVector2(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}
// 게임의 전반적인 플로우,
// 게임의 애니메이션 담당.
public class MatchManager : Singleton<MatchManager> {

    public Phase gamePhase = Phase.Normal;

    const int WIDTH = Constants.WIDTH;
    const int HEIGHT = Constants.HEIGHT;

    const int TILE_WIDTH = Constants.TILE_WIDTH;
    const int TILE_HEIGHT = Constants.TILE_HEIGHT;


    const int SCORE_NUM = Constants.SCORE_NUM;
    

    int comboCount = 0;
    public int score = 0;

    TileGridPanel tilePanel;
    
    void Start () {
        tilePanel = GetComponent<TileGridPanel>();
        StartCoroutine(StartGame());
    }
	IEnumerator StartGame()
    {
        gamePhase = Phase.Change;
        do
        {
            // 최초 전체 타일 생성.
            // 타일에 match 할 수 있는게 없다면 다시 타일 전체 생성.
            tilePanel.MakeAllTile();
            yield return new WaitForSeconds(0.4f);
            
            while (true)
            {
                // 전체 타일 중 match되는 타일 판별
                List<TileLine> matchLine = tilePanel.CheckAllMatch();

                //match가 있다면 제거하고 타일을 리필한 다음 다시 전체 match 판별 진행.
                if (matchLine.Count > 0)
                {
                    UIManager.Instance.SetComboText(comboCount);

                    tilePanel.RemoveMatchTile(matchLine,comboCount,ref score);
                    UIManager.Instance.SetScoreText(score);
                    yield return new WaitForSeconds(0.4f);

                    List<TileObject> refillList = tilePanel.RefillTile( matchLine);
                    yield return StartCoroutine(RefillTileAnimation(refillList));

                    comboCount++;


                }
                else
                {
                    break;
                }
            }
            comboCount = 0;
            UIManager.Instance.SetComboText(comboCount);
        } while (!tilePanel.CheckCanMatch());


        gamePhase = Phase.Normal;
    }

    public void TestButton1()
    {
        tilePanel.MakeAllTile();
        
    }

    List<TileLine> matchLine;
    public void TestButton2()
    {
        matchLine = tilePanel.CheckAllMatch();

        for (int i = 0; i < matchLine.Count; i++)
        {
            Debug.Log(matchLine[i].isVertical + " " + matchLine[i].lineIndex + " " + matchLine[i].startIndex + " " + matchLine[i].endIndex + " ");
        }
        
    }
    public void TestButton3()
    {
        if (matchLine.Count > 0)
        {
            tilePanel.RemoveMatchTile(matchLine,comboCount,ref score);
            
        }
    }
    public void TestButton4()
    {
        if (matchLine.Count > 0)
        {
            tilePanel.RefillTile( matchLine);
        }
    }

    public void TestButton5()
    {
    }
    
    public void TestButton6()
    {

    }
    

    public void CallSwapAndCheck(IntVector2 _tile1, IntVector2 _tile2)
    {
        StartCoroutine(SwapAndCheck(_tile1, _tile2));
    }


    IEnumerator SwapAndCheck(IntVector2 _tile1, IntVector2 _tile2)
    {
        gamePhase = Phase.Change;
        if ((_tile2.x >= 0) && (_tile2.y >= 0) && (_tile2.x < WIDTH) && (_tile2.y < HEIGHT))
        {

            tilePanel.SwapTile( _tile1, _tile2);
            yield return StartCoroutine(SwapTileAnimation(tilePanel.GetTileGameObject(_tile1), tilePanel.GetTileGameObject(_tile2)));

            
            List<TileLine> oneMatchLines = new List<TileLine>();
            oneMatchLines.AddRange(tilePanel.CheckOneMatch( _tile1.x, _tile1.y));
            oneMatchLines.AddRange(tilePanel.CheckOneMatch( _tile2.x, _tile2.y));
            
            if (oneMatchLines.Count > 0)
            {
                tilePanel.RemoveMatchTile( oneMatchLines, comboCount, ref score);
                UIManager.Instance.SetScoreText(score);
                yield return new WaitForSeconds(0.4f);

                List<TileObject> refillList = tilePanel.RefillTile( oneMatchLines);
                yield return StartCoroutine(RefillTileAnimation(refillList));

                while (true)
                {
                    while (true)
                    {
                        // 전체 타일 중 match되는 타일 판별
                        oneMatchLines = tilePanel.CheckAllMatch();


                        //match가 있다면 제거하고 타일을 리필한 다음 다시 전체 match 판별 진행.
                        if (oneMatchLines.Count > 0)
                        {

                            comboCount++;
                            UIManager.Instance.SetComboText(comboCount);

                            tilePanel.RemoveMatchTile( oneMatchLines, comboCount, ref score);
                            UIManager.Instance.SetScoreText(score);
                            yield return new WaitForSeconds(0.3f);

                            refillList = tilePanel.RefillTile( oneMatchLines);
                            yield return StartCoroutine(RefillTileAnimation(refillList));
                        }
                        else
                        {
                            break;
                        }
                    }

                    comboCount = 0;
                    UIManager.Instance.SetComboText(comboCount);
                    if (!tilePanel.CheckCanMatch())
                    {
                        tilePanel.MakeAllTile();
                        
                    }
                    else
                    {
                        break;
                    }
                } 


            }
            else
            {
                tilePanel.SwapTile( _tile1, _tile2);

                yield return StartCoroutine(SwapTileAnimation(tilePanel.GetTileGameObject(_tile1), tilePanel.GetTileGameObject(_tile2)));
            }
            
        }

        gamePhase = Phase.Normal;

    }
    

    IEnumerator RefillTileAnimation(List<TileObject> _refillList)
    {
        for(int i = 0; i < _refillList.Count; i++)
        {
            float posX = TILE_WIDTH * 0.5f + TILE_WIDTH * _refillList[i].posX;
            float posY = TILE_HEIGHT * -0.5f - TILE_HEIGHT * _refillList[i].posY;
            Vector3 destination = new Vector3(posX, posY, 0);

            Coroutine tempCoroutine = StartCoroutine(MoveTile(_refillList[i].gameObject,destination));

            if (i == (_refillList.Count - 1))
            {
                yield return tempCoroutine;
            }
        }
        
    }
    
    IEnumerator SwapTileAnimation(GameObject _obj1,GameObject _obj2)
    {

        StartCoroutine(MoveTile(_obj1, _obj2.transform.localPosition));
        yield return StartCoroutine(MoveTile(_obj2, _obj1.transform.localPosition));
    }

    IEnumerator MoveTile(GameObject _moveObject, Vector3 _endPosition)
    {
        Vector3 _startPosition = _moveObject.transform.localPosition;
        for(int i =0; i <= 10; i++)
        {
            _moveObject.transform.localPosition = Vector3.Lerp(_startPosition, _endPosition, i*0.1f);
            yield return new WaitForSeconds(0.04f);
        }
    }
}
