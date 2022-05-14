using System;
using System.Collections.Generic;
using System.Text;
using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

namespace LearnOpenTK
{
    class Table
    {
        //BFO
        private int vertexBufferObject;
        //VAO
        private int vertexArrayObject;
        //EBO
        private int elementBufferObject;


        Texture texture;
        Texture texture2;

        Shader shader;

        private float[] vertex;
        private uint[] indexies;

        public Table()
        {
            BlenderImportModel table = new BlenderImportModel("E:/OpenGL/LearnOpenTK/Chapter1/9-Camera/Models/rightSphere.obj");
            vertex = table.GetVertex().ToArray();
            indexies = table.GetIndexies().ToArray();
        }

        public void Init()
        {
            ////BIND VBO
            //vertexBufferObject = GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            //GL.BufferData(BufferTarget.ArrayBuffer, vertex.Length * sizeof(float), vertex, BufferUsageHint.StaticDraw);

            ////Create shader
            //shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            //shader.Use();
            ////var vertexLocation = shader.GetAttribLocation("aPosition");
            ////GL.EnableVertexAttribArray(vertexLocation);
            ////GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);





            ////BIND VAO
            //vertexArrayObject = GL.GenVertexArray();
            //GL.BindVertexArray(vertexArrayObject);

            ////рассказали, как VAO интерпретировать данные, находящиеся внутри VBO
            //var vertexLocation = shader.GetAttribLocation("aPosition");
            //GL.EnableVertexAttribArray(vertexLocation);
            //GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            //var texCoordLocation = shader.GetAttribLocation("aTexCoord");
            //GL.EnableVertexAttribArray(texCoordLocation);
            //GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            //elementBufferObject = GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);

            //GL.BufferData(BufferTarget.ElementArrayBuffer, indexies.Length * sizeof(uint), indexies, BufferUsageHint.StaticDraw);

            //texture = Texture.LoadFromFile("Resources/container.png");
            //texture.Use(TextureUnit.Texture0);

            //texture2 = Texture.LoadFromFile("Resources/awesomeface.png");
            //texture2.Use(TextureUnit.Texture1);

            //shader.SetInt("texture0", 0);
            //shader.SetInt("texture1", 1);

            vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);

            vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertex.Length * sizeof(float), vertex, BufferUsageHint.StaticDraw);

            elementBufferObject = GL.GenBuffer();
          
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexies.Length * sizeof(uint), indexies, BufferUsageHint.StaticDraw);


            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            shader.Use();

            var vertexLocation = shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            var texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            texture = Texture.LoadFromFile("Resources/container.png");
            texture.Use(TextureUnit.Texture0);

            texture2 = Texture.LoadFromFile("Resources/awesomeface.png");
            texture2.Use(TextureUnit.Texture1);

            shader.SetInt("texture0", 0);
            shader.SetInt("texture1", 1);
        }

        public void Draw(Camera _camera, double _time)
        {
            GL.BindVertexArray(vertexArrayObject);

            texture.Use(TextureUnit.Texture0);
            texture2.Use(TextureUnit.Texture1);
            //shader.Use();
            shader.Use();
            var model = Matrix4.Identity * Matrix4.CreateScale(0.2f) * Matrix4.CreateTranslation(-3, 10, 3);
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", _camera.GetViewMatrix());
            shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            
            //Код с передачей данных в шейдер
            GL.DrawElements(PrimitiveType.Triangles, indexies.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
