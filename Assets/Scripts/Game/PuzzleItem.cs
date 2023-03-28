using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleItem : MonoBehaviour
{
    public int TypeNumber;

    private Image _Image = null;

    [SerializeField]
    private Sprite[] _GemSprites;

    private float Speed = 7.5f;
    private float _Time = 0;
    
    public void SetType(int typeNumber)
    {
        if(_Image == null)
            _Image = GetComponent<Image>();

        TypeNumber = typeNumber;

        if (_Image == null)
            return;

        _Image.sprite = _GemSprites[typeNumber];
        _Image.transform.localScale = Vector3.one;
    }

    public Coroutine Delete()
    {
        return StartCoroutine(DeleteAnimation());
    }

    public IEnumerator DeleteAnimation()
    {
        _Time = 1;

        var localScale = _Image.transform.localScale;

        while (_Time > 0)
        {
            _Time -= Time.deltaTime * Speed;

            _Image.transform.localScale = localScale * _Time;

            yield return null;
        }
    }
}
