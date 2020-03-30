namespace HololensPETS
{
    public class MathUtils
    {
        public static double Clamp( double val, double min, double max )
        {
            if( val < min )
            {
                return min;
            }

            if( val > max )
            {
                return max;
            }

            return val;
        }

        public static double Max( double a, double b )
        {
            if( a >= b )
            {
                return a;
            }

            return b;
        }

        public static double Min( double a, double b )
        {
            if( a <= b )
            {
                return a;
            }

            return b;
        }

        public static double Abs( double val )
        {
            if( val < 0.0 )
            {
                return -val;
            }

            return val;
        }

        public static bool IsInRange( float val, float min, float max )
        {
            return ( min <= val ) && ( val <= max );
        }

        public static bool IsInRange( int val, int min, int max )
        {
            return ( min <= val ) && ( val <= max );
        }
    }
}
