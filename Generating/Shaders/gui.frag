#version 430

in vec2 textureCoords;

out vec4 out_Color;

uniform sampler2D guiTexture;

void main(){
	out_Color = texture2D(guiTexture, textureCoords);
}