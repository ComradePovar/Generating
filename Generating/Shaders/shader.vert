#version 450

uniform mat4 projectionMatrix; 
uniform mat4 modelViewMatrix;
uniform mat4 normalMatrix;

layout (location = 0) in vec3 inPosition;
layout (location = 1) in vec2 inCoord;
layout (location = 2) in vec3 inNormal;

out vec2 texCoord;
smooth out vec3 normal;

void main()
{
	gl_Position = projectionMatrix*modelViewMatrix*vec4(inPosition, 1.0);
	texCoord = inCoord;
	vec4 res = normalMatrix*vec4(inNormal, 0.0);
	normal = res.xyz;
}