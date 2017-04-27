#version 430

in vec4 clipSpace;
in vec2 texCoords;
in vec3 cameraVector;

out vec4 outputColor;

uniform sampler2D reflectionTexture;
uniform sampler2D refractionTexture;
uniform sampler2D dudvMapTexture;
uniform float time;

const float waveStrength = 0.005;

void main(){
	
	vec2 ndc = clipSpace.xy/clipSpace.w / 2.0 + 0.5;
	vec2 reflectTexCoords = vec2(ndc.x, -ndc.y);
	vec2 refractTexCoords = vec2(ndc.x, ndc.y);

	vec2 distortion1 = (texture2D(dudvMapTexture, vec2(texCoords.x + time, texCoords.y)).rg * 2.0 - 1.0) * waveStrength;
	vec2 distortion2 = (texture2D(dudvMapTexture, vec2(-texCoords.x + time, texCoords.y + time)).rg * 2.0 - 1.0) * waveStrength;
	vec2 totalDistortion = distortion1 + distortion2;

	reflectTexCoords += totalDistortion;
	reflectTexCoords.x = clamp(reflectTexCoords.x, 0.001, 0.999);
	reflectTexCoords.y = clamp(reflectTexCoords.y, -0.999, -0.001);

	refractTexCoords += totalDistortion;
	refractTexCoords = clamp(refractTexCoords, 0.001, 0.999);

	vec4 reflectionColor = texture2D(reflectionTexture, reflectTexCoords);
	vec4 refractionColor = texture2D(refractionTexture, refractTexCoords);

	vec3 viewVector = normalize(cameraVector);
	float viewAngleInfluence = dot(viewVector, vec3(0.0, 1.0, 0.0));

	outputColor = mix(reflectionColor, refractionColor, viewAngleInfluence);
	outputColor = mix(outputColor, vec4(0.0, 0.3, 0.5, 1.0), 0.2);
}