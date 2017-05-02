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

void main() {
	vec4 worldPosition = modelMatrix * vec4(inPosition, 1.0);
	clipSpace = projectionMatrix * viewMatrix * worldPosition;
	gl_Position = clipSpace;
	texCoords = inTexCoords;
	cameraVector = cameraPosition - worldPosition.xyz;
}