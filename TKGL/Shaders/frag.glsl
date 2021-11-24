#version 330 core
out vec4 FragColor;
in vec4 vertexColor;
in vec2 vertexUV;
uniform sampler2D texture1;
uniform sampler2D texture2;
uniform float time;
uniform float blend;

void main()
{
    // FragColor = mix(texture(texture1, vertexUV), vertexColor, sin(time) * 0.5 + 0.5);
	// FragColor = texture(texture1, vertexUV);
	// FragColor = texture(texture1, vertexUV);
	FragColor = mix(texture(texture1, vertexUV), texture(texture2, vertexUV), blend);
}