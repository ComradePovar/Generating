#version 430

uniform mat4 projectionMatrix; 
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;
uniform mat4 normalMatrix;
uniform vec4 plane;

layout (location = 0) in vec3 inPosition;
layout (location = 1) in vec2 inCoord;
layout (location = 2) in vec3 inNormal;
layout (location = 3) in float inNormalizedHeight;
layout (location = 4) in float inMoisture;

out vec2 texCoord;
smooth out vec3 normal;
out float normalizedHeight;
smooth out vec3 worldPos;
smooth out vec4 eyeSpacePosition;
out float moisture;


void main()
{
	vec4 eyeSpacePositionVertex = viewMatrix*modelMatrix*vec4(inPosition, 1.0);
	gl_Position = projectionMatrix*eyeSpacePositionVertex;
	gl_ClipDistance[0] = dot(modelMatrix*vec4(inPosition, 1.0), plane);

	texCoord = inCoord;
	normal = inNormal;//(normalMatrix * vec4(inNormal, 0.0)).xyz;
	normalizedHeight = inNormalizedHeight;
	worldPos = (modelMatrix*vec4(inPosition, 1.0)).xyz;
	eyeSpacePosition = eyeSpacePositionVertex;
	moisture = inMoisture;
}