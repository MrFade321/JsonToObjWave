using Newtonsoft.Json;
using System.Text.Json.Nodes;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.Numerics;

namespace JsonToObj
{
    public partial class Form1 : Form
    {
        private JToken JsonModle;


        public Form1()
        {
            InitializeComponent();
        }

        public class AttributeLookUp
        {
            public string Key;
            public int offset;
            public AttributeLookUp(string key, int offset)
            {
                Key = key;
                this.offset = offset;
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {            

            List<VertexObject> VertexObjects = new List<VertexObject>();            


            string temp = File.ReadAllText("Goatman_melee.json");
            JsonModle = JsonConvert.DeserializeObject<JToken>(temp);

            var verticesArrayJson = JsonModle["tStructure"]["#ptChunks"][0]["Vertices"];

            foreach (var vertexObject in verticesArrayJson)
            {
                Stream stream = new MemoryStream();     ///VertexAttributes
                var Writer = new BinaryWriter(stream);  /// I am not sure about this section in the code it is ptChunkVertices so idk man....
                int[] vertexArrayArray = vertexObject["VertexAttributes"].ToObject<int[]>();

                foreach (var data in vertexArrayArray)
                {
                    Writer.Write(data);
                }

                var MyReader = new BinaryReader(stream);

                List<AttributeLookUp> LookUpTable = new List<AttributeLookUp>();
                var Desc = vertexObject["InputDescriptions"];
                foreach (var Descriptions in Desc)
                {
                    string LookupKey = Descriptions["eSemantic"].ToObject<string>() + "-" + Descriptions["eFormat"].ToObject<string>();
                    int offset = Descriptions["nOffset"].ToObject<int>();
                    LookUpTable.Add(new AttributeLookUp(LookupKey, offset));
                }

                VertexObjects.Add(new VertexObject(vertexObject, MyReader, LookUpTable));                

            }


            var facesArray = JsonModle["tStructure"]["#ptChunks"][0]["Faces"];
            var subObjects = JsonModle["tStructure"]["#ptChunks"][0]["SubObjects"];


            foreach (var subObjectContainer in subObjects)
            {
                var SubList = subObjectContainer["ptSubObjects"];

                foreach (var SubObject in SubList)
                {
                    int bufferIndex = SubObject["BufferIndex"].ToObject<int>();
                    var buffer = VertexObjects[bufferIndex];
                    var LookUpTable = VertexObjects[bufferIndex].Attribute_lookup;
                    var bufferDataReader = buffer.reader;
                    int vertexWidth = buffer.Vertex["VertexWidth"].ToObject<int>();
                    var vertexObject = SubObject["ptSegments"][0];
                    int vertexCount = vertexObject["nVertCount"].ToObject<int>();

                    List<Vector3> tempVert = new List<Vector3>();
                    List<Vector2> tempUV= new List<Vector2>();

                    for (int vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
                    {
                        bufferDataReader.BaseStream.Seek(vertexObject["VertexBufferByteOffset"].ToObject<int>() + (vertexWidth + (vertexIndex)),SeekOrigin.Begin);
                        if (bufferDataReader.BaseStream.Position >= bufferDataReader.BaseStream.Length)
                        {
                            var test = 1; // oopies
                        }

                        var x = bufferDataReader.ReadSingle();
                        var y = bufferDataReader.ReadSingle();
                        var z = bufferDataReader.ReadSingle();
                        tempVert.Add(new Vector3(x, y, z));

                        var uvOffset = GrabAttributeOffset(LookUpTable, "1-7");
                        bufferDataReader.BaseStream.Seek(vertexObject["VertexBufferByteOffset"].ToObject<int>() + (vertexWidth + (vertexIndex)) + uvOffset, SeekOrigin.Begin);
                        var u = bufferDataReader.ReadHalf();
                        var v = bufferDataReader.ReadHalf();
                        tempUV.Add(new Vector2((float)u, (float)v));

                    }
                    var test2 = 2;
                }
            }


        }

        public int GrabAttributeOffset(List<AttributeLookUp> LookUpTable, string key)
        {
            foreach (var item in LookUpTable)
            {
                if (item.Key == key)
                    return item.offset;
            }
            return 0;
        }



    }
}