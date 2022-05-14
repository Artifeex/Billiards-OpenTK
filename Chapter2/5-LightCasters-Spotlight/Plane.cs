using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
namespace LearnOpenTK
{
    class Plane
    {
        int vertexBufferObject = GL.GenBuffer();
        int vertexArrayObject = GL.GenVertexArray();
        int elementBufferObject = GL.GenBuffer();

        float[] vertices =
        { 
            -5.0f, 0.0f, 3.0f,  0f, 1.0f, 0f, 0.0f, 0.0f,
            -5.0f, 0.0f, -3.0f, 0f, 1.0f, 0f, 0.0f, 1.0f,
            5.0f,  0.0f, -3.0f, 0f, 1.0f, 0f, 1.0f, 1.0f,
            5.0f,  0.0f, 3.0f, 0f, 1.0f, 0f, 1.0f, 0.0f
        };

        uint[] indices =
        {
            0, 1, 2, 0, 2, 3
        };
        Shader shader;
        Vector3 position;


        private Texture diffuseMap;
        private Texture specularMap;

        public Plane(Vector3 pos)
        {   
            
            position = pos;
            //Bind and send in VBO
            vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);


            shader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");

            var positionLocation = shader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            var normalLocation = shader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            var texCoordLocation = shader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

            diffuseMap = Texture.LoadFromFile("Resources/BilliardTexture.jpg");
            specularMap = Texture.LoadFromFile("Resources/Specular.jpg");


        }

        public void Draw(Camera camera)
        {
            GL.BindVertexArray(vertexArrayObject);

            diffuseMap.Use(TextureUnit.Texture0);
            specularMap.Use(TextureUnit.Texture1);

            shader.Use();

            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            shader.SetVector3("viewPos", camera.Position);

            shader.SetInt("material.diffuse", 0);
            shader.SetInt("material.specular", 1);
            shader.SetVector3("material.specular", new Vector3(0.0f, 0.0f, 0.0f));
            shader.SetFloat("material.shininess", 32.0f);

            shader.SetVector3("light.position", camera.Position);
            shader.SetVector3("light.direction", camera.Front);
            shader.SetFloat("light.cutOff", MathF.Cos(MathHelper.DegreesToRadians(30f)));
            shader.SetFloat("light.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(40f)));
            shader.SetFloat("light.constant", 1.0f);
            shader.SetFloat("light.linear", 0.09f);
            shader.SetFloat("light.quadratic", 0.032f);
            shader.SetVector3("light.ambient", new Vector3(0.2f));
            shader.SetVector3("light.diffuse", new Vector3(0.5f));
            shader.SetVector3("light.specular", new Vector3(1.0f));
            
            Matrix4 model = Matrix4.CreateScale(1.0f) * Matrix4.CreateTranslation(position);

            shader.SetMatrix4("model", model);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        void Move(Vector3 borders)
        {

        }
    }
}
