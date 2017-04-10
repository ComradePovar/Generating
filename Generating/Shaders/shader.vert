#version 430

uniform mat4 projectionMatrix; 
uniform mat4 modelViewMatrix;
uniform mat4 normalMatrix;

layout (location = 0) in vec3 inPosition;
layout (location = 1) in vec2 inCoord;
layout (location = 2) in vec3 inNormal;
layout (location = 3) in float inNormalizedHeight;

out vec2 texCoord;
smooth out vec3 normal;
smooth out vec4 eyeSpacePosition;
out float normalizedHeight;

void main()
{
	vec4 eyeSpacePositionVertex = modelViewMatrix*vec4(inPosition, 1.0);
	gl_Position = projectionMatrix*eyeSpacePositionVertex;

	texCoord = inCoord;
	vec4 res = normalMatrix*vec4(inNormal, 0.0);
	normal = res.xyz;
	normalizedHeight = inNormalizedHeight;
	eyeSpacePosition = eyeSpacePositionVertex;
}