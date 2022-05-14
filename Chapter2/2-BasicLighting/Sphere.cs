using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearnOpenTK
{
    class Sphere
    {


        List<float> vertices = new List<float>();
        List<uint> indices = new List<uint>();

        Shader shader;

        private int elementBufferObject;

        private int vertexArrayObject;

        private int vertexBufferObject;

        Texture texture;
        Shader lightShader;

        public Sphere(float radius, uint stackCount, uint sectorCount)
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
                z = radius * MathF.Sin(stackAngle) * lengthInv;              // r * sin(u)

                // add (sectorCount+1) vertices per stack
                // the first and last vertices have same position and normal, but different tex coords
                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;           // starting from 0 to 2pi

                    // vertex position (x, y, z)
                    x = xy * MathF.Cos(sectorAngle) * lengthInv;             // r * cos(u) * cos(v)
                    y = xy * MathF.Cos(sectorAngle) * lengthInv;             // r * cos(u) * sin(v)
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

                    //// vertex tex coord (s, t) range between [0, 1]
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

        }

        public void Init()
        {
            vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);

            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            shader.Use();

            
            var vertexLocation = shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            lightShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            lightShader.Use();

            var positionLocation = lightShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);

            var normalLocation = lightShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            var texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

            texture = Texture.LoadFromFile("Resources/container.png");
            texture.Use(TextureUnit.Texture0);
            shader.SetInt("texture0", 0);

            elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);


            

            //texture2 = Texture.LoadFromFile("Resources/awesomeface.png");
            //texture2.Use(TextureUnit.Texture1);

            //shader.SetInt("texture0", 0);
            //shader.SetInt("texture1", 1);
        }

        public void Draw(Camera _camera, double _time)
        {
            GL.BindVertexArray(vertexArrayObject);

            //texture.Use(TextureUnit.Texture0);
            //texture2.Use(TextureUnit.Texture1);
            //shader.Use();
            shader.Use();
            var model = Matrix4.Identity;
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", _camera.GetViewMatrix());
            shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            //Код с передачей данных в шейдер
            GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
        }
    }
    }
