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
namespace Billiards
{
    class Plane
    {
        int vertexBufferObject;
        int vertexArrayObject;
        int elementBufferObject;
        float[] lightingPos =
        {
            0.0f, 3.0f, -1.0f // 1 proj

        };
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

        private readonly Vector3[] spotLightPositions =
        {
            new Vector3(-2.0f, 3.0f, 0.0f),
            new Vector3(0f, 3.0f, 0.0f),
            new Vector3(2f, 3.0f, 0.0f)
        };
        Shader shader;
        Vector3 position;

        private Texture diffuseMap;
        private Texture specularMap;

        public Plane(Vector3 pos)
        {   
            position = pos;
            vertexBufferObject = GL.GenBuffer();
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

            shader.SetFloat("material.shininess", 32.0f);

            for (int i = 0; i < spotLightPositions.Length; i++)
            {
                shader.SetVector3($"pointsSpotLight[{i}].position", spotLightPositions[i]);
                shader.SetVector3($"pointsSpotLight[{i}].direction", new Vector3(0, -1, 0));
                shader.SetVector3($"pointsSpotLight[{i}].ambient", new Vector3(0.2f, 0.2f, 0.2f));
                shader.SetVector3($"pointsSpotLight[{i}].diffuse", new Vector3(1.0f, 1.0f, 1.0f));
                shader.SetVector3($"pointsSpotLight[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                shader.SetFloat($"pointsSpotLight[{i}].constant", 1.0f);
                shader.SetFloat($"pointsSpotLight[{i}].linear", 0.09f);
                shader.SetFloat($"pointsSpotLight[{i}].quadratic", 0.032f);
                shader.SetFloat($"pointsSpotLight[{i}].cutOff", MathF.Cos(MathHelper.DegreesToRadians(25f)));
                shader.SetFloat($"pointsSpotLight[{i}].outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(30f)));
            }          
            Matrix4 model = Matrix4.CreateScale(1.0f) * Matrix4.CreateTranslation(position);

            shader.SetMatrix4("model", model);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
