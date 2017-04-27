#version 430

layout (location = 0) in vec3 inPosition;
layout (location = 1) in vec2 inTexCoords;

out vec4 clipSpace;
out vec2 texCoords;
out vec3 cameraVector;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;
uniform vec3 cameraPosition;

const float tiling = 6.0;

void main() {
	vec4 worldPosition = modelMatrix * vec4(inPosition, 1.0);
	clipSpace = projectionMatrix * viewMatrix * worldPosition;
	gl_Position = clipSpace;
	texCoords = vec2(inPosition.x / 1024, inPosition.z / 1024) * tiling;
	//texCoords = inTexCoords;
	cameraVector = cameraPosition - worldPosition.xyz;
}