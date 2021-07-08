using System.Numerics;

namespace Generation.Classes
{
    public class Walker
    {
        public (int,  int) position;
        public (int,  int) direction;

        public Walker(int x, int y, int directionX, int directionY)
        {
            position = (x, y);
            direction = (directionX, directionY);
        }
        
    }
}