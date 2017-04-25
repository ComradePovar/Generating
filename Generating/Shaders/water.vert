#version 430

layout (location = 0) in vec3 inPosition;

out vec4 clipSpace;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;


void main() {
	clipSpace = projectionMatrix * viewMatrix * modelMatrix * vec4(inPosition, 1.0);
	gl_Position = clipSpace;
}