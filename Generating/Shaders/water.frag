#version 430

in vec4 clipSpace;


out vec4 outputColor;

uniform sampler2D reflectionTexture;
uniform sampler2D refractionTexture;

void main(){
	
	vec2 ndc = clipSpace.xy/clipSpace.w / 2.0 + 0.5;
	vec2 reflectTexCoords = vec2(ndc.x, -ndc.y);
	vec2 refractTexCoords = vec2(ndc.x, ndc.y);

	vec4 reflectionColor = texture2D(reflectionTexture, reflectTexCoords);
	vec4 refractionColor = texture2D(refractionTexture, refractTexCoords);

	outputColor = mix(reflectionColor, refractionColor, 0.5);
}