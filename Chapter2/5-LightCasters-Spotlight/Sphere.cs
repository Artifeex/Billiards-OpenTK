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
    class Sphere
    {
        int vertexBufferObject = GL.GenBuffer();
        int vertexArrayObject = GL.GenVertexArray();
        int elementBufferObject = GL.GenBuffer();
        List<float> vertices = new List<float>();
        List<uint> indices = new List<uint>();
        Shader shader;
        Vector3 position;

        float speedRotation = 2f;
        float speedMove = 0.01f;
        float rotation = -90.0f;

        private Texture diffuseMap;
        private Texture specularMap;

        public Sphere(float radius, uint stackCount, uint sectorCount, Vector3 pos, string pathTexture, string pathSpecular)
        {
            float x, y, z, xy;                              // vertex position
            float nx, ny, nz, lengthInv = 1.0f / radius;    // vertex normal
            float s, t;                                     // vertex texCoord

            float sectorStep = 2 * MathF.PI / sectorCount;
            float stackStep = MathF.PI / stackCount;
            float sectorAngle, stackAngle;

            for (int i = 0; i <= stackCount; ++i)
            {
                stackAngle = MathF.PI / 2 - i * stackStep;        // starting from pi/2 to -pi/2
                xy = radius * MathF.Cos(stackAngle);             // r * cos(u)
                z = radius * MathF.Sin(stackAngle);              // r * sin(u)

                // add (sectorCount+1) vertices per stack
                // the first and last vertices have same position and normal, but different tex coords
                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;           // starting from 0 to 2pi

                    // vertex position (x, y, z)
                    x = xy * MathF.Cos(sectorAngle);             // r * cos(u) * cos(v)
                    y = xy * MathF.Sin(sectorAngle);             // r * cos(u) * sin(v)
                    vertices.Add(x);
                    vertices.Add(y);
                    vertices.Add(z);

                    // normalized vertex normal (nx, ny, nz)
                    nx = x * lengthInv;
                    ny = y * lengthInv;
                    nz = z * lengthInv;
                    vertices.Add(nx);
                    vertices.Add(ny);
                    vertices.Add(nz);

                    // vertex tex coord (s, t) range between [0, 1]
                    s = (float)j / sectorCount;
                    t = (float)i / stackCount;
                    vertices.Add(s);
                    vertices.Add(t);
                }
            }

            uint k1, k2;
            for (uint i = 0; i < stackCount; ++i)
            {
                k1 = i * (sectorCount + 1);     // beginning of current stack
                k2 = k1 + sectorCount + 1;      // beginning of next stack

                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    // 2 triangles per sector excluding first and last stacks
                    // k1 => k2 => k1+1
                    if (i != 0)
                    {
                        indices.Add(k1);
                        indices.Add(k2);
                        indices.Add(k1 + 1);
                    }

                    // k1+1 => k2 => k2+1
                    if (i != (stackCount - 1))
                    {
                        indices.Add(k1 + 1);
                        indices.Add(k2);
                        indices.Add(k2 + 1);
                    }

                }                
            }

            position = pos;
            //Bind and send in VBO
            vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);


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

            diffuseMap = Texture.LoadFromFile(pathTexture);
            specularMap = Texture.LoadFromFile(pathSpecular);

            
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
            shader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
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

            Matrix4 rotationX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-90.0f));
            Matrix4 rotationZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(8.0f));
            Matrix4 model = Move(new Vector3(1f, 1f, 2.2f));
            shader.SetMatrix4("model", model);

            GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
        }

        Matrix4 Move(Vector3 borders)
        {
            rotation += speedRotation;
            Matrix4 rotationX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotation));
            Matrix4 rotationZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(8.0f));
            position += new Vector3(0f, 0f, speedMove);
            if(position.Z >= borders.Z || position.Z <= -borders.Z)
            {
                speedMove = -speedMove;
                speedRotation = -speedRotation;
            }
            Matrix4 model = rotationX * rotationZ * Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(position);
            return model;
        }
    }
}
