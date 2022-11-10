using System;

// Class with useful algorithms, functions, and data structures.
// TODO: transfer Grid class to utils ?
public static class Utils
{
    // public static float findMedian()
    // {
    //     return 0f;
    // }

    // public static float findMean()
    // {
    //     return 0f;
    // }

    public static bool IsBitEnabled(int bitMask, int index)
    {
        return (bitMask & (1 << index)) != 0;
    }

    /**
    Takes integer in the interval from [0, 100].
    
    Returns true if new integer generated in the range [0, chance] is 
    less or equals to chance.
    */
    public static bool Chance(int chance)
    {
        if (chance < 0 || chance > 100)
        {
            throw new System.Exception("Given chance is not in the range of [0, 100]");
        }

        return new Random().Next(0, 100) <= chance;
    }
}
