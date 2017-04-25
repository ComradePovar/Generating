#version 430

layout (location = 0) in vec2 inPosition;

out vec2 textureCoords;

uniform mat4 modelMatrix;

void main(){
	gl_Position = modelMatrix * vec4(inPosition, 0.0, 1.0);
	textureCoords = vec2((inPosition.x + 1.0) / 2.0, 1.0 - (inPosition.y + 1.0) / 2.0);
}