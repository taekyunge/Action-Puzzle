using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGroup
{
    public int TypeNumber = 0;

    public List<PuzzleBox> PuzzleBoxes = new List<PuzzleBox>();

    public PuzzleGroup(int typeNumber)
    {
        TypeNumber = typeNumber;
    }

    public void Clear()
    {
        PuzzleBoxes.Clear();
    }

    public void AddBox(PuzzleBox puzzleBox, bool group = true)
    {
        if (!PuzzleBoxes.Exists(x => x == puzzleBox))
        {
            if(group)
                puzzleBox.Group = this;

            PuzzleBoxes.Add(puzzleBox);
        }
    }

    public void AddBox(List<PuzzleBox> puzzleBoxes, bool group = true)
    {
        for (int i = 0; i < puzzleBoxes.Count; i++)
        {
            AddBox(puzzleBoxes[i], group);
        }
    }

    public void RemoveBox(PuzzleBox puzzleBox)
    {
        if (PuzzleBoxes.Exists(x => x == puzzleBox))
            PuzzleBoxes.Remove(puzzleBox);
    }

    public PuzzleBox GetBox(int index)
    {
        if (index >= 0 && index < PuzzleBoxes.Count)
            return PuzzleBoxes[index];

        return null;
    }
}
