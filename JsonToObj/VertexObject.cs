using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JsonToObj.Form1;

namespace JsonToObj
{
    public class VertexObject
    {
        public JToken Vertex;
        public BinaryReader reader;
        public List<AttributeLookUp> Attribute_lookup;
       


        public VertexObject(JToken vertex_Object, BinaryReader Reader, List<AttributeLookUp> attribute_lookup)
        { 
            Vertex =    vertex_Object;
            reader = Reader;    
             Attribute_lookup= attribute_lookup;
        }


    }
}
