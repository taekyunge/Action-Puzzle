using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleBox : MonoBehaviour
{
    private PuzzleItem _Item = null;

    public PuzzleItem Item
    {
        set
        {
            if(value != null)
            {
                _DeltaTime = 0.0f;
                _StartPos = value.transform.position;
            }
            
            _Item = value;
        }
        get
        {
            return _Item;
        }
    }

    public int Number = 0;

    private float _Speed = 7f;

    private float _DeltaTime = 1f;

    private Vector3 _StartPos = Vector3.zero;


    public PuzzleGroup Group;

    public bool IsMove
    {
        get { return _DeltaTime < 1; }
    }

    private void Update()
    {
        if (_Item == null)
            return;

        if (PuzzleBoard.CurrentBox == this)
        {
            _Item.transform.position = Input.mousePosition;
        }
        else
        {
            if (_DeltaTime < 1)
            {
                _DeltaTime += Time.deltaTime * _Speed;

                _Item.transform.position = Vector3.Lerp(_StartPos, transform.position, _DeltaTime);

                if (_DeltaTime >= 1)
                    _Item.transform.position = transform.position;
            }
        }
    }

    public void OnPointerEnter()
    {
        if (!PuzzleBoard.Instance.Result && PuzzleBoard.CurrentBox != null && PuzzleBoard.CurrentBox != this)
        {
            var item = PuzzleBoard.CurrentBox.Item;

            PuzzleBoard.CurrentBox.Item = Item;
            Item = item;

            PuzzleBoard.CurrentBox = this;
        }
    }

    public void OnPointerDown()
    {
        if(!IsMove)
            PuzzleBoard.CurrentBox = this;
    }

    public void OnPointerUp()
    {
        if(PuzzleBoard.CurrentBox != this)
            PuzzleBoard.Instance.PuzzleClear();

        PuzzleBoard.CurrentBox = null;
    }
}
