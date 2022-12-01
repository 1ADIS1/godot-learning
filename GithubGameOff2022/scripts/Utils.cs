using System;
using System.Collections;

// Class with useful algorithms, functions, and data structures.
public static class Utils
{
    public static bool IsBitEnabled(int bitMask, int index)
    {
        return (bitMask & (1 << index)) != 0;
    }

    public static int EnableBit(int bitMask, int index)
    {
        return bitMask | (1 << index);
    }

    // TODO: define length of bitmask
    public static int CountEnabledBits(int bitmask)
    {
        int number = 0;
        for (int index = 0; index < 4; index++)
        {
            if (IsBitEnabled(bitmask, index))
            {
                number++;
            }
        }

        return number;
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
