#version 430
#define PI 3.1415926535897932384626433832795
#define PIOver6 0.52359877559829887307
#define PIOver4 0.78539816339744830961
#define PIOver2 1.57079632679489661923

smooth in vec2 texCoord;
smooth in vec3 normal;
in float normalizedHeight;
smooth in vec3 worldPos;
smooth in vec4 eyeSpacePosition;
in float moisture;

out vec4 outputColor;

struct DirectionalLight
{
	vec3 color;
	vec3 direction;
	float ambientIntensity;
	float specularIntensity;
	float specularPower;
};
struct Fog
{
	vec4 color;
	float start;
	float end;
	float density;
	int type;
};

// Samplers:
// 0) rock;
// 1) water;
// 2) mossyrock;
// 3) dirt;
// 4) sand;
// 5) darkgrass;
uniform sampler2D samplers[6];
uniform vec4 color;
uniform DirectionalLight light;
uniform Fog fog;
uniform vec3 eyePos;

float getFogFactor(Fog fog, float coord);
vec4 getSpecularColor();

void main()
{
	outputColor = vec4(0.0, 0.0, 0.0, 1.0);
	vec4 specularColor = vec4(0.0, 0.0, 0.0, 0.0);
	float angle = abs(acos(normal.y));

	const float waterLowerBound = 0.0;
	const float waterUpperBound = 0.13;
	const float sandLowerBound = 0.18;
	const float sandUpperBound = 0.45;
	const float darkgrassLowerBound = 0.5;
	const float darkgrassUpperBound = 0.7;
	const float rockLowerBound = 0.75;
	const float rockUpperBound = 1.0;

	const float moistureLevel1 = 0.305;
	const float moistureLevel2 = 0.405;
	const float moistureLevel3 = 0.475;
	const float moistureLevel4 = 0.575;

	vec4 water = texture2D(samplers[1], texCoord);
	vec4 sand = texture2D(samplers[4], texCoord);
	vec4 darkgrass = texture2D(samplers[5], texCoord);
	vec4 mossyrock = texture2D(samplers[2], texCoord);
	vec4 dirt = texture2D(samplers[3], texCoord);
	vec4 rock = texture2D(samplers[0], texCoord);

	bool isSteep = angle > PIOver4;
	float steepInfluence = angle * (waterUpperBound - normalizedHeight)*4/ PIOver2;

	vec4 texColor = vec4(0.0, 0.0, 0.0, 1.0);

	float height = normalizedHeight;
    if (height <= waterUpperBound) {
		outputColor = sand;
	}
	else if (height <= sandLowerBound) {
		if (moisture <= moistureLevel1) {
			float sandInfluence = (sandLowerBound - height) / (sandLowerBound - waterUpperBound);
			outputColor = sand * sandInfluence + darkgrass * (1.0 - sandInfluence);
		} else if (moisture <= moistureLevel2) {
			float sandInfluence = (moisture - moistureLevel1) / (moistureLevel2 - moistureLevel1);
			float heightInfluence = (sandLowerBound - height) / (sandLowerBound - waterUpperBound);
			outputColor = sand * sandInfluence + darkgrass * (1.0 - sandInfluence);
			outputColor = outputColor * (1.0 - heightInfluence) + sand * heightInfluence;
		}
		else{
			outputColor = sand;
		}
    } else if (height <= sandUpperBound) {
		if (moisture <= moistureLevel1) {
			outputColor = darkgrass;
		} else if (moisture <= moistureLevel2) {
			float sandInfluence = (moisture - moistureLevel1) / (moistureLevel2 - moistureLevel1);
			outputColor = sand * sandInfluence + darkgrass * (1.0 - sandInfluence);
		}
		else {
			outputColor = sand;
		}
	} else if (height <= darkgrassLowerBound) {
		if (moisture <= moistureLevel1) {
			float dirtInfluence = (height - sandUpperBound)/(darkgrassLowerBound - sandUpperBound);
			outputColor = dirt * dirtInfluence + darkgrass * (1.0 - dirtInfluence);
		} else if (moisture <= moistureLevel2) {
			float sandInfluence = (moisture - moistureLevel1) / (moistureLevel2 - moistureLevel1);
			float darkgrassInfluence = (moisture - moistureLevel1) / (moistureLevel2 - moistureLevel1);
			float heightInfluence = (height - sandUpperBound) / (darkgrassLowerBound - sandUpperBound);
			vec4 darkgrassSandColor = sand * sandInfluence + darkgrass * (1.0 - sandInfluence);
			vec4 dirtDarkGrassColor = darkgrass * darkgrassInfluence + dirt * (1.0 - darkgrassInfluence);
			outputColor = dirtDarkGrassColor * heightInfluence + darkgrassSandColor * (1.0 - heightInfluence);
		} else {
			float darkgrassInfluence = (height - sandUpperBound) / (darkgrassLowerBound - sandUpperBound);
			outputColor = darkgrass * darkgrassInfluence + sand * (1.0 - darkgrassInfluence);
		}
	} else if (height <= darkgrassUpperBound) {
		if (moisture <= moistureLevel1){
			outputColor = dirt;
		} else if (moisture <= moistureLevel2){
			float darkgrassInfluence = (moisture - moistureLevel1) / (moistureLevel2 - moistureLevel1);
			outputColor = darkgrass * darkgrassInfluence + dirt * (1.0 - darkgrassInfluence);
			//outputColor = vec4(0.0, 0.0, 0.0, 1.0);
		} else {
			outputColor = darkgrass;
		}
	} else if (height <= rockLowerBound) {
		if (moisture <= moistureLevel1){
			float rockInfluence = (height - darkgrassUpperBound) / (rockLowerBound - darkgrassUpperBound);
			specularColor = getSpecularColor() * rockInfluence;
			outputColor = rock * rockInfluence + dirt * (1.0 - rockInfluence);
		} else if (moisture <= moistureLevel2) {
			float mossyrockInfluence = (moisture - moistureLevel1) / (moistureLevel2 - moistureLevel1);
			float darkgrassInfluence = (moisture - moistureLevel1) / (moistureLevel2 - moistureLevel1);
			float heightInfluence = (height - darkgrassUpperBound) / (rockLowerBound - darkgrassUpperBound);
			specularColor = getSpecularColor() * heightInfluence;
			vec4 rockMossyRockColor = mossyrock * mossyrockInfluence + rock * (1.0 - mossyrockInfluence);
			vec4 dirtDarkGrassColor = darkgrass * darkgrassInfluence + dirt * (1.0 - darkgrassInfluence);
			outputColor = rockMossyRockColor * heightInfluence + dirtDarkGrassColor * (1.0 - heightInfluence);
			//outputColor = vec4(0.0, 0.0, 0.0, 1.0);
		} else {
			float mossyrockInfluence = (height - darkgrassUpperBound) / (rockLowerBound - darkgrassUpperBound);
			outputColor = mossyrock * mossyrockInfluence + darkgrass * (1.0 - mossyrockInfluence);
		}
	} else {
		if (moisture <= moistureLevel1){
			specularColor = getSpecularColor();
			outputColor = rock;
		} else if (moisture <= moistureLevel2){
			float mossyrockInfluence = (moisture - moistureLevel1) / (moistureLevel2 - moistureLevel1);
			specularColor = getSpecularColor() * (1.0 - mossyrockInfluence);
			outputColor = mossyrock * mossyrockInfluence + rock * (1.0 - mossyrockInfluence);
		} else {
			outputColor = mossyrock;
		}
	}

	float diffuseIntensity = max(0.0, dot(normalize(normal), -light.direction));
	vec4 diffuseColor = vec4(1.0, 1.0, 1.0, 1.0);
	diffuseColor = vec4(light.color*(light.ambientIntensity+diffuseIntensity), 1.0);

	outputColor *= color*(diffuseColor + specularColor);

	float fogCoord = abs(eyeSpacePosition.z / eyeSpacePosition.w);
	outputColor = mix(outputColor, fog.color, getFogFactor(fog, fogCoord));
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

vec4 getSpecularColor()
{
		vec4 specularColor = vec4(0.0, 0.0, 0.0, 0.0);
		vec3 normalized = normalize(normal);
		vec3 reflectedVector = normalize(reflect(light.direction, normalized));
		vec3 vertexToEyeVector = normalize(eyePos - worldPos);
		float specularFactor = dot(vertexToEyeVector, reflectedVector);

		if (specularFactor > 0){
			specularFactor = pow(specularFactor, light.specularPower);
			specularColor = vec4(light.color, 1.0) * light.specularIntensity * specularFactor;
		}
		return specularColor;
}