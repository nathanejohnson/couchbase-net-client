﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Couchbase.Core.Serializers
{
    interface ITypeSerializer2
    {
        byte[] Serialize<T>(T value);

        T Deserialize<T>(ArraySegment<byte> buffer, int offset, int length);

        T Deserialize<T>(byte[] buffer, int offset, int length);
    }
}
