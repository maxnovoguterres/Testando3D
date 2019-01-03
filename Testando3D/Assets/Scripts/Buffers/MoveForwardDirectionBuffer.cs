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
    public struct MoveForwardDirectionBuffer : IBufferElementData
    {
        public static MoveForwardDirectionBuffer MoveFowardDirectionX { get {
                var r = new MoveForwardDirectionBuffer
                {
                    Value = Direction.X
                }; return r; }
        }
        public static MoveForwardDirectionBuffer MoveFowardDirectionY
        {
            get
            {
                var r = new MoveForwardDirectionBuffer
                {
                    Value = Direction.Y
                }; return r;
            }
        }
        public static MoveForwardDirectionBuffer MoveFowardDirectionZ
        {
            get
            {
                var r = new MoveForwardDirectionBuffer
                {
                    Value = Direction.Z
                }; return r;
            }
        }

        public static implicit operator Direction(MoveForwardDirectionBuffer e) { return e.Value; }
        public static implicit operator MoveForwardDirectionBuffer(Direction e) { return new MoveForwardDirectionBuffer { Value = e }; }

        public Direction Value { get; set; }
    }
}
