using UnityEngine;

namespace Gamla
{
    public static class GamlaRandom
    {
        private static int _sid;

        static GamlaRandom()
        {
            GamlaService.OnMatchStarted.Subscribe((matchId, code, isTournament) =>
            {
                _sid = matchId.GetHashCode();
            });
            
            GamlaService.MatchEnd.Subscribe((score) => { _sid = 0; });
        }

        private static float GetValue()
        {
            if (_sid == int.MaxValue)
                _sid = 0;
            _sid++;
            Random.InitState(_sid);
            return Random.value;
        }

        /**
        * Find a point inside the unit sphere using Value()
        **/
        public static Vector3 InsideUnitSphere()
        {
            float r = GetValue();
            float phi = GetValue() * Mathf.PI;
            float theta = GetValue() * Mathf.PI * 2;

            float x = r * Mathf.Cos(theta) * Mathf.Sin(phi);
            float y = r * Mathf.Sin(theta) * Mathf.Sin(phi);
            float z = r * Mathf.Cos(phi);

            return new Vector3(x, y, z);
        }

        /**
        * Find a point inside the unit circle using Value()
        **/
        public static Vector2 InsideUnitCircle()
        {
            float radius = 1.0f;
            float rand = GetValue() * 2 * Mathf.PI;
            Vector2 val = new Vector2();

            val.x = radius * Mathf.Cos(rand);
            val.y = radius * Mathf.Sin(rand);

            return val;
        }

        /**
        * Hybrid rejection / trig method to generate points on a sphere using Value()
        **/
        public static Vector3 OnUnitSphere()
        {
            Vector3 val = new Vector3();
            float s;

            do
            {
                val.x = 2 * (float)GetValue() - 1;
                val.y = 2 * (float)GetValue() - 1;
                s = Mathf.Pow(val.x, 2) + Mathf.Pow(val.y, 2);
            }
            while (s > 1);

            float r = 2 * Mathf.Sqrt(1 - s);

            val.x *= r;
            val.y *= r;
            val.z = 2 * s - 1;

            return val;
        }

        /**
        * Quaternion random using Value()
        **/
        public static Quaternion RotationUniform()
        {
            float u1 = GetValue();
            float u2 = GetValue();
            float u3 = GetValue();

            float u1sqrt = Mathf.Sqrt(u1);
            float u1m1sqrt = Mathf.Sqrt(1 - u1);
            float x = u1m1sqrt * Mathf.Sin(2 * Mathf.PI * u2);
            float y = u1m1sqrt * Mathf.Cos(2 * Mathf.PI * u2);
            float z = u1sqrt * Mathf.Sin(2 * Mathf.PI * u3);
            float w = u1sqrt * Mathf.Cos(2 * Mathf.PI * u3);

            return new Quaternion(x, y, z, w);
        }

        /**
        * Quaternion random using Value()
        **/
        public static Quaternion Rotation()
        {
            return RotationUniform();
        }

        /**
        * Ranged random float using Value()
        **/
        public static float Range(float min, float max)
        {
            float rand = GetValue();
            return min + (rand * (max - min));
        }

        /**
        * Ranged random int using Value()
        **/
        public static int Range(int min, int max)
        {
            float rand = GetValue();
            return min + (int)(rand * (max - min));
        }
    }
}

