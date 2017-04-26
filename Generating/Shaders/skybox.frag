#version 430

in vec3 texCoords;

out vec4 outputColor;

uniform samplerCube skyboxTexture;

void main() {
	outputColor = texture(skyboxTexture, texCoords);
}