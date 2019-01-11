using Assets.Scripts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Assets.Scripts.Buffers
{
    [InternalBufferCapacity(3)]
    public struct DirectionBuffer : IBufferElementData
    {
        public static DirectionBuffer MoveFowardDirectionX { get {
                var r = new DirectionBuffer
                {
                    Value = Direction.X
                }; return r; }
        }
        public static DirectionBuffer MoveFowardDirectionY
        {
            get
            {
                var r = new DirectionBuffer
                {
                    Value = Direction.Y
                }; return r;
            }
        }
        public static DirectionBuffer MoveFowardDirectionZ
        {
            get
            {
                var r = new DirectionBuffer
                {
                    Value = Direction.Z
                }; return r;
            }
        }

        public static implicit operator Direction(DirectionBuffer e) { return e.Value; }
        public static implicit operator DirectionBuffer(Direction e) { return new DirectionBuffer { Value = e }; }

        public Direction Value { get; set; }
    }
}
