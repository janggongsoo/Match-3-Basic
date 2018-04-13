using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 타일 오브젝트에 대한 클래스
// 클릭 및 드래그 시 동작에 대한 정의
public class TileObject : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler {
    public int posX;
    public int posY;
    bool isMove = false;
    Vector2 startPosition;

    Image tileImage;
    
    public void SetTile(GameObject _tile, int _posY, int _posX)
    {
        if (tileImage == null)
        {
            tileImage = GetComponent<Image>();
        }
        tileImage.sprite = _tile.GetComponent<Image>().sprite;
        posY = _posY;
        posX = _posX;

    }

    public void setTilePos(int _posY,int _posX)
    {
        posY = _posY;
        posX = _posX;

    }
   
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (MatchManager.Instance.gamePhase != Phase.Normal)
        {
            return;
        }
        isMove = false;
        startPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(MatchManager.Instance.gamePhase != Phase.Normal)
        {
            return;
        }
        if (!isMove)
        {

            Vector2 deltaPostion = eventData.position - startPosition;
            if (deltaPostion.x >= 20)
            {
                MatchManager.Instance.CallSwapAndCheck(new IntVector2(posX, posY), new IntVector2(posX + 1, posY));
                isMove = true;
            }
            else if (deltaPostion.x <= -20)
            {
                MatchManager.Instance.CallSwapAndCheck(new IntVector2(posX, posY), new IntVector2(posX - 1, posY));
                isMove = true;
            }
            else if (deltaPostion.y >= 20)
            {
                MatchManager.Instance.CallSwapAndCheck(new IntVector2(posX, posY), new IntVector2(posX, posY - 1));
                isMove = true;
            }
            else if (deltaPostion.y <= -20)
            {
                MatchManager.Instance.CallSwapAndCheck(new IntVector2(posX, posY), new IntVector2(posX, posY + 1));
                isMove = true;
            }
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag = " + eventData.position);
        isMove = false;
    }
}
