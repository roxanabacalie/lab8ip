using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Monster
{
    class PrototypeManager
    {
        public static object Clone(object obj)
        {
            object objClone = null;
            MemoryStream memory = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            binForm.Serialize(memory, obj);
            memory.Position = 0;
            objClone = binForm.Deserialize(memory);
            return objClone;
        }
    }
}
