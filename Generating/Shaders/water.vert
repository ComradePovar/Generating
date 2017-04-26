#version 430

layout (location = 0) in vec3 inPosition;

out vec4 clipSpace;
out vec2 texCoords;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

const float tiling = 3.0;

void main() {
	clipSpace = projectionMatrix * viewMatrix * modelMatrix * vec4(inPosition, 1.0);
	gl_Position = clipSpace;
	texCoords = vec2(inPosition.x / 1024, inPosition.y / 1024) * tiling;
}