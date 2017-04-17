#version 430

uniform mat4 projectionMatrix; 
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;
uniform mat4 normalMatrix;

layout (location = 0) in vec3 inPosition;
layout (location = 1) in vec2 inCoord;
layout (location = 2) in vec3 inNormal;
layout (location = 3) in float inNormalizedHeight;

out vec2 texCoord;
smooth out vec3 normal;
out float normalizedHeight;
smooth out vec3 worldPos;
smooth out vec4 eyeSpacePosition;

void main()
{
	vec4 eyeSpacePositionVertex = viewMatrix*modelMatrix*vec4(inPosition, 1.0);
	gl_Position = projectionMatrix*eyeSpacePositionVertex;

	texCoord = inCoord;
	normal = inNormal;//(normalMatrix * vec4(inNormal, 0.0)).xyz;
	normalizedHeight = inNormalizedHeight;
	worldPos = (modelMatrix*vec4(inPosition, 1.0)).xyz;
	eyeSpacePosition = eyeSpacePositionVertex;
}