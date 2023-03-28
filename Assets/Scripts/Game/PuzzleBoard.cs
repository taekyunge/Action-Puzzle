using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleBoard : Singleton<PuzzleBoard>
{
    public static PuzzleBox CurrentBox = null;

    private readonly int _ClearCount = 3;

    [SerializeField]
    private Vector2 Size = Vector2.zero;

    [HideInInspector]
    public int Width = 0;

    [HideInInspector]
    public int Height = 0;

    [SerializeField]
    private ScoreBoard _ScoreBoard = null;

    private PuzzleItem _BasePuzzleItem = null;

    private Pooling<PuzzleItem> _PuzzleItemPool = null;

    private PuzzleBox[] _PuzzleBoxes = null;

    private List<PuzzleGroup> _CheckGroups = new List<PuzzleGroup>();

    private List<Coroutine> _Animations = new List<Coroutine>();

    public bool Result = false;

    protected override void Awake()
    {
        base.Awake();

        _BasePuzzleItem = GetComponentInChildren<PuzzleItem>();
        _PuzzleBoxes = GetComponentsInChildren<PuzzleBox>();
        _PuzzleItemPool = new Pooling<PuzzleItem>(_PuzzleBoxes.Length, _BasePuzzleItem, transform);

        Width = (int)Size.x;
        Height = (int)Size.y;

        for (int i = 0; i < 5; i++)
        {
            _CheckGroups.Add(new PuzzleGroup(i));
        }
    }

    private void Start()
    {
        for (int i = _PuzzleBoxes.Length - 1; i >= 0; i--)
        {
            var item = _PuzzleItemPool.Get(_PuzzleBoxes[i].transform.position.x, transform.position.y);

            item.SetType(Random.Range(0, 5));

            _PuzzleBoxes[i].Item = item;
            _PuzzleBoxes[i].Number = i;
        }

        _ScoreBoard.Clear();
    }

    private void CheckGroupClear()
    {
        for (int i = 0; i < _CheckGroups.Count; i++)
        {
            _CheckGroups[i].Clear();
        }
    }

    /// <summary>
    /// 인접한 블럭 확인후 제거
    /// </summary>
    public void PuzzleClear()
    {
        Result = false;

        // 가로줄 체크
        for (int i = 0; i < Height; i++)
        {
            CheckGroupClear();

            for (int j = 0; j < Width; j++)
            {
                var index = j + (i * Width);

                _CheckGroups[_PuzzleBoxes[index].Item.TypeNumber].AddBox(_PuzzleBoxes[index], false);
            }

            for (int j = 0; j < _CheckGroups.Count; j++)
            {
                int count = 0;

                PuzzleBox prevBox = null;

                for (int k = 0; k < _CheckGroups[j].PuzzleBoxes.Count + 1; k++)
                {
                    if (prevBox == null)
                    {
                        prevBox = _CheckGroups[j].GetBox(k);
                        count++;
                    }
                    else
                    {
                        if (k < _CheckGroups[j].PuzzleBoxes.Count && _CheckGroups[j].PuzzleBoxes[k].Number == prevBox.Number + 1)
                        {
                            count++;
                        }
                        else
                        {
                            if (count >= _ClearCount)
                            {
                                var puzzleGroup = new PuzzleGroup(j);

                                for (int p = k - 1; p >= k - count; p--)
                                {
                                    puzzleGroup.AddBox(_CheckGroups[j].PuzzleBoxes[p]);
                                }

                                Result = true;
                            }
                            else
                            {

                                for (int p = k - 1; p >= k - count; p--)
                                {
                                    var puzzleGroup = new PuzzleGroup(j);

                                    puzzleGroup.AddBox(_CheckGroups[j].PuzzleBoxes[p]);
                                }
                            }

                            count = 1;
                        }

                        prevBox = _CheckGroups[j].GetBox(k);
                    }
                }
            }
        }

        // 세로 체크
        for (int i = 0; i < Width; i++)
        {
            CheckGroupClear();

            for (int j = 0; j < Height; j++)
            {
                var index = i + (j * Width);

                _CheckGroups[_PuzzleBoxes[index].Item.TypeNumber].AddBox(_PuzzleBoxes[index], false);
            }

            for (int j = 0; j < _CheckGroups.Count; j++)
            {
                int count = 0;

                PuzzleBox prevBox = null;

                for (int k = 0; k < _CheckGroups[j].PuzzleBoxes.Count + 1; k++)
                {
                    if (prevBox == null)
                    {
                        prevBox = _CheckGroups[j].GetBox(k);
                        count++;
                    }
                    else
                    {
                        if (k < _CheckGroups[j].PuzzleBoxes.Count && _CheckGroups[j].PuzzleBoxes[k].Number == prevBox.Number + Width)
                        {
                            count++;
                        }
                        else
                        {
                            if (count >= _ClearCount)
                            {
                                var puzzleGroup = new PuzzleGroup(j);

                                for (int p = k - 1; p >= k - count; p--)
                                {
                                    if(_CheckGroups[j].PuzzleBoxes[p].Group.PuzzleBoxes.Count >= _ClearCount)
                                    {
                                        puzzleGroup.AddBox(_CheckGroups[j].PuzzleBoxes[p].Group.PuzzleBoxes);
                                    }
                                    else
                                    {
                                        puzzleGroup.AddBox(_CheckGroups[j].PuzzleBoxes[p]);
                                    }
                                }

                                Result = true;
                            }
                            else if(count > 1)
                            {
                                var puzzleGroup = new PuzzleGroup(j);
                                                            
                                for (int p = k - 1; p >= k - count; p--)
                                {
                                    if (_CheckGroups[j].PuzzleBoxes[p].Group.PuzzleBoxes.Count >= _ClearCount)
                                    {
                                        puzzleGroup.AddBox(_CheckGroups[j].PuzzleBoxes[p].Group.PuzzleBoxes);
                                    }
                                }
                            }

                            count = 1;
                        }

                        prevBox = _CheckGroups[j].GetBox(k);
                    }
                }
            }
        }

        if(Result)
            StartCoroutine(DeletePuzzle());
    }

    private IEnumerator DeletePuzzle()
    {
        // 블럭 삭제
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                var index = j + (i * Width);

                if(_PuzzleBoxes[index].Group != null && _PuzzleBoxes[index].Group.PuzzleBoxes.Count >= _ClearCount)
                {
                    var puzzleboxes = _PuzzleBoxes[index].Group.PuzzleBoxes;
                    var typeNumber = _PuzzleBoxes[index].Group.TypeNumber;
                    var puzzleCount = _PuzzleBoxes[index].Group.PuzzleBoxes.Count;

                    _Animations.Clear();

                    for (int k = 0; k < puzzleboxes.Count; k++)
                    {
                        _Animations.Add(puzzleboxes[k].Item.Delete());
                    }

                    _ScoreBoard.AddScore((typeNumber + 100) * puzzleCount);

                    for (int k = 0; k < _Animations.Count; k++)
                    {
                        yield return _Animations[k];
                    }

                    _Animations.Clear();

                    for (int k = 0; k < puzzleboxes.Count; k++)
                    {
                        _PuzzleItemPool.Delete(puzzleboxes[k].Item);

                        puzzleboxes[k].Group = null;
                        puzzleboxes[k].Item = null;
                    }                    
                }
            }
        }

        // 빈자리 매꾸기
        for (int i = _PuzzleBoxes.Length - 1; i >= 0; i--)
        {
            var box = _PuzzleBoxes[i];

            if (!box.Item)
            {
                for (int j = 1; j < Height; j++)
                {
                    int next = i - (Width * j);

                    if (next > 0)
                    {
                        var nextBox = _PuzzleBoxes[next];

                        if (nextBox.Item)
                        {
                            box.Item = nextBox.Item;
                            nextBox.Item = null;
                            break;
                        }
                    }
                    else
                        break;
                }
            }
        }

        // 빈자리 채우기
        for (int i = _PuzzleBoxes.Length - 1; i >= 0; i--)
        {
            var puzzleBox = _PuzzleBoxes[i];

            if (!puzzleBox.Item)
            {
                var item = _PuzzleItemPool.Get(_PuzzleBoxes[i].transform.position.x, transform.position.y);

                item.SetType(Random.Range(0, 5));

                puzzleBox.Item = item;
            }
        }

        while (System.Array.Exists(_PuzzleBoxes, x => x.IsMove))
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        PuzzleClear();
    }
}