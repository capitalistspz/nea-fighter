using System;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace common.physics
{
    // Vector2 but for integers
    public struct Vector2I : INetSerializable, IEquatable<Vector2I>
    {
        public int X, Y;
        public static readonly Vector2I Zero = new(0, 0);
        public static readonly Vector2I One = new (1, 1);
        public Vector2I(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public static Vector2I operator +(Vector2I value1, Vector2I value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }
            
        public static Vector2I operator -(Vector2I value1, Vector2I value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }

        public static Vector2I operator *(Vector2I value, int multiplier)
        {
            value.X *= multiplier;
            value.Y *= multiplier;
            return value;
        }
        public static Vector2I operator /(Vector2I value, int divisor)
        {
            value.X /= divisor;
            value.Y /= divisor;
            return value;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(X);
            writer.Put(Y);
        }
        public static implicit operator Vector2(Vector2I v2I)
        {
            return new Vector2(v2I.X, v2I.Y);
        }
        public void Deserialize(NetDataReader reader)
        {
            X = reader.GetInt();
            Y = reader.GetInt();
        }
        
        public bool Equals(Vector2I other)
        {
            return X == other.X && Y == other.Y;
        }
        
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}