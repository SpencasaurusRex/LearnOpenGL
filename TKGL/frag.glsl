#version 330 core
out vec4 FragColor;
in vec4 vertexColor;
in vec2 vertexUV;
uniform sampler2D texture0;
uniform float time;

void main()
{
    FragColor = mix(texture(texture0, vertexUV), vertexColor, sin(time) * 0.5 + 0.5);
}