#version 430

in vec4 clipSpace;
in vec2 texCoords;
in vec3 cameraVector;

out vec4 outputColor;

uniform sampler2D reflectionTexture;
uniform sampler2D refractionTexture;
uniform sampler2D dudvMapTexture;
uniform float time;
uniform sampler2D normalMap;
uniform vec3 lightColor;
uniform vec3 lightDirection;
uniform float specularIntensity;
uniform float specularPower;

const float waveStrength = 0.02;

vec4 getSpecularColor(vec3 vertexToCameraVector, vec2 texCoords);

void main(){
	
	vec2 ndc = clipSpace.xy/clipSpace.w / 2.0 + 0.5;
	vec2 reflectTexCoords = vec2(ndc.x, -ndc.y);
	vec2 refractTexCoords = vec2(ndc.x, ndc.y);

	//vec2 distortion1 = (texture2D(dudvMapTexture, vec2(texCoords.x + time, texCoords.y)).rg * 2.0 - 1.0) * waveStrength;
	//vec2 distortion2 = (texture2D(dudvMapTexture, vec2(-texCoords.x + time, texCoords.y + time)).rg * 2.0 - 1.0) * waveStrength;
	//vec2 totalDistortion = distortion1 + distortion2;
	vec2 distortedTexCoords = texture2D(dudvMapTexture, vec2(texCoords.x + time, texCoords.y)).rg * 0.1;
	distortedTexCoords = texCoords + vec2(distortedTexCoords.x, distortedTexCoords.y + time);
	vec2 totalDistortion = (texture2D(dudvMapTexture, distortedTexCoords).rg * 2.0 - 1.0) * waveStrength;


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
	outputColor = mix(outputColor, vec4(0.0, 0.4, 0.7, 1.0), 0.2) + getSpecularColor(viewVector, distortedTexCoords);
}

vec4 getSpecularColor(vec3 vertexToCameraVector, vec2 texCoords)
{
		vec4 specularColor = vec4(0.0, 0.0, 0.0, 0.0);
		vec4 normalMapColor = texture2D(normalMap, texCoords);
		vec3 normal = normalize(vec3(normalMapColor.r * 2.0 - 1.0, normalMapColor.b * 2.0 - 1.0, normalMapColor.g * 2.0 - 1.0));
		vec3 reflectedVector = normalize(reflect(lightDirection, normal));
		float specularFactor = dot(vertexToCameraVector, reflectedVector);

		if (specularFactor > 0){
			specularFactor = pow(specularFactor, specularPower);
			specularColor = vec4(lightColor, 0.0) * specularIntensity * specularFactor;
		}
		return specularColor;
}