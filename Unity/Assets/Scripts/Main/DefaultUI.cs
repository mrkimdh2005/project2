using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class DefaultUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI buttonInfo;

    [SerializeField] Image[] cycleingImages;
    [SerializeField] Sprite[] cycleingTargetSprites;
    
    [SerializeField] string[] cycleingImageNames;

    CircularList<Tuple<Sprite, string>> cycleingSprites;

    int imageStartingIndex;

    private void Start()
    {
        cycleingSprites = new();
        imageStartingIndex = 0;

        for (int i = 0; i < cycleingImages.Length; i++)
        {
            var tuple = Tuple.Create(cycleingTargetSprites[i], cycleingImageNames[i]);
            cycleingSprites.Push(tuple);
        }
    }

    public void ChangeMode_RightButton()
    {
        imageStartingIndex = imageStartingIndex == cycleingImages.Length - 1 ?
            0 : imageStartingIndex + 1;

        cycleingSprites.SetIndexPosition(imageStartingIndex);

        for (int i = 0; i < cycleingImages.Length; i++)
        {
            UpdateImageInfo(i);
            cycleingSprites.MoveNext();
        }

        return;
    }

    public void ChangeMode_LeftButton()
    {
        imageStartingIndex = imageStartingIndex == 0 ?
            cycleingImages.Length - 1 : imageStartingIndex - 1;

        cycleingSprites.SetIndexPosition(imageStartingIndex);

        for (int i = 4; i >= 0; i--)
        {
            cycleingSprites.MoveBack();
            UpdateImageInfo(i);
        }

        return;
    }

    private void UpdateImageInfo(int imageIndex)
    {
        cycleingImages[imageIndex].sprite = cycleingSprites.Current.Item1;

        if (imageIndex == 0)
            buttonInfo.text = cycleingSprites.Current.Item2;
    }
}