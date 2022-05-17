#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 Normal;
out vec3 FragPos;
out vec2 TexCoords;

void main()
{
    gl_Position = vec4(aPos, 1.0) * model * view * projection;
    //To calculate light in world space
    FragPos = vec3(vec4(aPos, 1.0) * model);
    //So that normals don't die when scaling
    Normal = aNormal * mat3(transpose(inverse(model)));
    TexCoords = aTexCoords;
}