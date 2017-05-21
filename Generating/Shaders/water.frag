#version 430

struct Fog
{
	vec4 color;
	float start;
	float end;
	float density;
	int type;
};

in vec4 clipSpace;
in vec2 texCoords;
in vec3 cameraVector;
smooth in vec4 eyeSpacePosition;

out vec4 outputColor;

uniform sampler2D reflectionTexture;
uniform sampler2D refractionTexture;
uniform sampler2D dudvMapTexture;
uniform float time;
uniform sampler2D normalMap;
uniform sampler2D depthMap;
uniform vec3 lightColor;
uniform vec3 lightDirection;
uniform float specularIntensity;
uniform float specularPower;
uniform float waveStrength;
uniform Fog fog;

vec4 getSpecularColor(vec3 vertexToCameraVector, vec3 normal);
float getFogFactor(Fog fog, float coord);

void main(){
	
	vec2 ndc = clipSpace.xy/clipSpace.w / 2.0 + 0.5;
	vec2 reflectTexCoords = vec2(ndc.x, -ndc.y);
	vec2 refractTexCoords = vec2(ndc.x, ndc.y);

	float near = 10.0;
	float far = 20000.0;
	float depth = texture2D(depthMap, refractTexCoords).r;
	float floorDistance = 2.0 * near * far / (far + near - (2.0 * depth - 1.0) * (far - near));;

	depth = gl_FragCoord.z;
	float waterDistance = 2.0 * near * far / (far + near - (2.0 * depth - 1.0) * (far - near));
	float waterDepth = floorDistance - waterDistance;

	vec2 distortedTexCoords = texture2D(dudvMapTexture, vec2(texCoords.x + time, texCoords.y)).rg * 0.1;
	distortedTexCoords = texCoords + vec2(distortedTexCoords.x, distortedTexCoords.y + time);
	vec2 totalDistortion = (texture2D(dudvMapTexture, distortedTexCoords).rg * 2.0 - 1.0) * waveStrength * clamp(waterDepth/5.0, 0.0, 1.0);


	reflectTexCoords += totalDistortion;
	reflectTexCoords.x = clamp(reflectTexCoords.x, 0.001, 0.999);
	reflectTexCoords.y = clamp(reflectTexCoords.y, -0.999, -0.001);

	refractTexCoords += totalDistortion;
	refractTexCoords = clamp(refractTexCoords, 0.000, 0.999);

	vec4 reflectionColor = texture2D(reflectionTexture, reflectTexCoords);
	vec4 refractionColor = texture2D(refractionTexture, refractTexCoords);

	vec4 normalMapColor = texture2D(normalMap, distortedTexCoords);
	vec3 normal = normalize(vec3(normalMapColor.r * 2.0 - 1.0, normalMapColor.b * 3.0, normalMapColor.g * 2.0 - 1.0));

	vec3 viewVector = normalize(cameraVector);
	float viewAngleInfluence = dot(viewVector, normal);

	outputColor = mix(reflectionColor, refractionColor, 0.5*viewAngleInfluence);
	outputColor = mix(outputColor, vec4(0.0, 1.0, 1.0, 1.0), 0.2) + getSpecularColor(viewVector, normal) * clamp(waterDepth/5.0, 0.0, 1.0);
	outputColor.a = clamp(waterDepth/15.0, 0.0, 1.0);

	
	float fogCoord = abs(eyeSpacePosition.z / eyeSpacePosition.w);
	outputColor = mix(outputColor, fog.color, getFogFactor(fog, fogCoord));
}

vec4 getSpecularColor(vec3 vertexToCameraVector, vec3 normal)
{
		vec4 specularColor = vec4(0.0, 0.0, 0.0, 0.0);
		vec3 reflectedVector = normalize(reflect(lightDirection, normal));
		float specularFactor = dot(vertexToCameraVector, reflectedVector);

		if (specularFactor > 0){
			specularFactor = pow(specularFactor, specularPower);
			specularColor = vec4(lightColor, 0.0) * specularIntensity * specularFactor;
		}
		return specularColor;
}

float getFogFactor(Fog fog, float coord)
{
	float result = 0.0;
	if (fog.type == 0)
		result = (fog.end - coord) / (fog.end - fog.start);
	else if (fog.type == 1)
		result = exp(-fog.density * coord);
	else if (fog.type == 2)
		result = exp(-pow(fog.density * coord, 2.0));

	result = 1.0 - clamp(result, 0.0, 1.0);

	return result;
}