public abstract class Weapon
{
    public int[,] weaponRange { get; protected set; }
    public (int X, int Y) size;

    public int count { get; protected set; }

    public Weapon(int cnt)
    {
        this.weaponRange = new int[0, 0];
        this.size = (0, 0);
        this.count = cnt;
    }
}

public class BoringWeapon : Weapon
{
    public BoringWeapon(int cnt) : base(cnt)
    {
        int[,] array =
        {
            {1}
        };
        this.weaponRange = array;
        this.size = (1, 1);
    }
}

public class BigWeapon : Weapon
{
    public BigWeapon(int cnt) : base(cnt)
    {
        int[,] array =
        {
            { 1, 1, 1 },
            { 1, 1, 1 },
            { 1, 1, 1 }
        };
        this.weaponRange = array;
        this.size = (3, 3);
    }
}

public class VeryBigWeapon : Weapon
{
    public VeryBigWeapon(int cnt) : base(cnt)
    {
        int[,] array =
        {
            {1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1}
        };
        this.weaponRange = array;
        this.size = (5, 5);
    }
}

public class LineWeapon : Weapon
{
    public LineWeapon(int cnt) : base(cnt)
    {
        int[,] array =
        {
            {1},
            {1},
            {1},
            {1},
            {1}
        };
        this.weaponRange = array;
        this.size = (1, 5);
    }
}

