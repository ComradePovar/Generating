#version 450

uniform mat4 projection;
uniform mat4 modelView;

layout (location = 0) in vec3 inPosition;
layout (location = 1) in vec3 inColor;

smooth out vec3 color;

void main()
{
	gl_Position = projection * modelView * vec4(inPosition, 1.0);
	color = inColor;
}