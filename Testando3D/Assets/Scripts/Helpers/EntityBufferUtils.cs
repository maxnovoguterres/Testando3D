using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;

namespace Assets.Scripts.Helpers
{
    public static class EntityBufferUtils
    {
        public static NativeArray<T> BufferValues<T>(params object[] data) where T : struct, IBufferElementData
        {
            var r = new NativeArray<T>(data.Length, Allocator.TempJob);

            object buffer = Activator.CreateInstance(typeof(T));

            for (var i = 0; i < data.Length; i++)
            {
                buffer.GetType().GetProperty("Value").SetValue(buffer, data[i], null);

                r[i] = (T)buffer;
            }

            return r;
        }
    }
}
