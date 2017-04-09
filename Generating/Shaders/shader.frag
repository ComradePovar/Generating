#version 450

in vec2 texCoord;
out vec4 outputColor;

uniform sampler2D sampler;

void main()
{
	outputColor = texture2D(sampler, texCoord);
}