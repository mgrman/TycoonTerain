using System;

namespace Votyra.Core.Models
{
    [Serializable]
    public struct UI_Vector3i
    {
        public int x;
        public int y;
        public int z;

        public UI_Vector3i(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3i(UI_Vector3i value)
        {
            return new Vector3i(value.x, value.y,value.z);
        }

        public static implicit operator UI_Vector3i(Vector3i value)
        {
            return new UI_Vector3i(value.x, value.y,value.z);
        }

        public static bool operator ==(UI_Vector3i a, UI_Vector3i b)
        {
            return a.x == b.x && a.y == b.y&& a.z == b.z;
        }

        public static bool operator !=(UI_Vector3i a, UI_Vector3i b)
        {
            return a.x != b.x || a.y != b.y|| a.z != b.z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UI_Vector3i))
            {
                return false;
            }
            var that = (UI_Vector3i)obj;

            return this == that;
        }

        public override int GetHashCode()
        {
            return x + y * 7+ z * 13;
        }
    }
}