using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


[Serializable]
public struct WorldPos
{
    public int x, y;

    public WorldPos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        if (GetHashCode() == obj.GetHashCode())
            return true;
        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 47;
            hash = hash * 227 + x.GetHashCode();
            hash = hash * 227 + y.GetHashCode();
            return hash;
        }
    }
}

