using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace LearnOpenTK.Render
{
    public class Shape
    {
        public static Mesh CreateCube()
        {
            List<int> indices = new List<int>() {
                0, 1, 2, 0, 2, 3,       //front
                4, 5, 6, 4, 6, 7,       //right 
                8, 9, 10, 8, 10, 11,    //back 
                12, 13, 14, 12, 14, 15, //left 
                16, 17, 18, 16, 18, 19, //upper 
                20, 21, 22, 20, 22, 23  //bottom
            }; 

            List<Vertex> verts = new List<Vertex>();

            // front
            verts.Add( new Vertex() { Position = new Vector3(-1, -1, 1), Normal = new Vector3(), Color = new Vector4(0, 0, 1, 1) });
            verts.Add( new Vertex() { Position = new Vector3(1, -1, 1), Normal = new Vector3(), Color = new Vector4(1, 0, 1, 1) });
            verts.Add( new Vertex() { Position = new Vector3(1, 1, 1), Normal = new Vector3(), Color = new Vector4(1, 1, 1, 1) });
            verts.Add( new Vertex() { Position = new Vector3(-1, 1, 1), Normal = new Vector3(), Color = new Vector4(0, 1, 1, 1) });

            // right
            verts.Add(new Vertex() { Position = new Vector3(1, 1, 1), Normal = new Vector3(), Color = new Vector4(1, 1, 1, 1) });
            verts.Add(new Vertex() { Position = new Vector3(1, 1, -1), Normal = new Vector3(), Color = new Vector4(1, 0, 1, 1) });
            verts.Add(new Vertex() { Position = new Vector3(1, -1, -1), Normal = new Vector3(), Color = new Vector4(1, 1, 1, 1) });
            verts.Add(new Vertex() { Position = new Vector3(1, -1, 1), Normal = new Vector3(), Color = new Vector4(0, 1, 1, 1) });

            // back 
            verts.Add(new Vertex() { Position = new Vector3(-1, -1, -1), Normal = new Vector3(), Color = new Vector4(1, 1, 1, 1) });
            verts.Add(new Vertex() { Position = new Vector3(1, -1, -1), Normal = new Vector3(), Color = new Vector4(1, 0, 1, 1) });
            verts.Add(new Vertex() { Position = new Vector3(1, 1, -1), Normal = new Vector3(), Color = new Vector4(1, 1, 1, 1) });
            verts.Add(new Vertex() { Position = new Vector3(-1, 1, -1), Normal = new Vector3(), Color = new Vector4(0, 1, 1, 1) });

            // left
            verts.Add(new Vertex() { Position = new Vector3(-1, -1, -1), Normal = new Vector3(), Color = new Vector4(1, 1, 1, 1) });
            verts.Add(new Vertex() { Position = new Vector3(-1, -1, 1), Normal = new Vector3(), Color = new Vector4(1, 0, 1, 1) });
            verts.Add(new Vertex() { Position = new Vector3(-1, 1, 1), Normal = new Vector3(), Color = new Vector4(1, 1, 1, 1) });
            verts.Add(new Vertex() { Position = new Vector3(-1, 1, -1), Normal = new Vector3(), Color = new Vector4(0, 1, 1, 1) });

            // upper
            verts.Add(new Vertex() { Position = new Vector3(1, 1, 1), Normal = new Vector3(), Color = new Vector4(1, 1, 1, 1) });
            verts.Add(new Vertex() { Position = new Vector3(-1, 1, 1), Normal = new Vector3(), Color = new Vector4(1, 0, 1, 1) });
            verts.Add(new Vertex() { Position = new Vector3(-1, 1, -1), Normal = new Vector3(), Color = new Vector4(1, 1, 1, 1) });
            verts.Add(new Vertex() { Position = new Vector3(1, 1, -1), Normal = new Vector3(), Color = new Vector4(0, 1, 1, 1) });

            // bottom 
            verts.Add(new Vertex() { Position = new Vector3(-1, -1, -1), Normal = new Vector3(), Color = new Vector4(1, 1, 1, 1) });
            verts.Add(new Vertex() { Position = new Vector3(1, -1, -1), Normal = new Vector3(), Color = new Vector4(1, 0, 1, 1) });
            verts.Add(new Vertex() { Position = new Vector3(1, -1, 1), Normal = new Vector3(), Color = new Vector4(1, 1, 1, 1) });
            verts.Add(new Vertex() { Position = new Vector3(-1, -1, 1), Normal = new Vector3(), Color = new Vector4(0, 1, 1, 1) });

            var mesh = new Mesh();
            mesh.Load(verts, indices, 12);

            return mesh;
        }
    }
}
