#version 430
#define PI 3.1415926535897932384626433832795
#define PIOver6 0.52359877559829887307
#define PIOver4 0.78539816339744830961
#define PIOver2 1.57079632679489661923

smooth in vec2 texCoord;
smooth in vec3 normal;
in float normalizedHeight;
smooth in vec4 eyeSpacePosition;

out vec4 outputColor;

struct DirectionalLight
{
	vec3 color;
	vec3 direction;
	float ambientIntensity;
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
// 1) grass;
// 2) mud;
// 3) dirt;
uniform sampler2D samplers[4];
uniform vec4 color;
uniform DirectionalLight light;
uniform Fog fog;

float getFogFactor(Fog fog, float coord);

void main()
{
	outputColor = vec4(0.0, 0.0, 0.0, 1.0);
	float angle = abs(acos(normal.y));


	const float grassLowerBound = 0.0;
	const float grassUpperBound = 0.5;
	const float mudLowerBound = 0.6;
	const float mudUpperBound = 0.7;
	const float rockLowerBound = 0.8;
	const float rockUpperBound = 1.0;

	bool isSteep = angle > PIOver4;
	float steepInfluence = angle * (grassUpperBound - normalizedHeight)*4/ PIOver2;

	vec4 texColor = vec4(0.0, 0.0, 0.0, 1.0);

	if (normalizedHeight <= grassUpperBound){
		texColor = texture2D(samplers[3], texCoord);
		outputColor += texColor * steepInfluence;
		texColor = texture2D(samplers[1], texCoord);
		outputColor += texColor * (1 - steepInfluence);
	}
	else if (normalizedHeight < mudLowerBound){
	
		float mudInfluence = (normalizedHeight - grassUpperBound) / (mudLowerBound - grassUpperBound);

		texColor = texture2D(samplers[2], texCoord);
		outputColor += texColor * mudInfluence;

		texColor = texture2D(samplers[1], texCoord);
		outputColor += texColor * (1.0 - mudInfluence);
	}
	else if (normalizedHeight <= mudUpperBound){
		texColor = texture2D(samplers[2], texCoord);
		outputColor += texColor;
	}
	else if (normalizedHeight < rockLowerBound){
		float rockInfluence = (normalizedHeight - mudUpperBound) / (rockLowerBound - mudUpperBound);

		texColor = texture2D(samplers[0], texCoord);
		outputColor += texColor * rockInfluence;

		texColor = texture2D(samplers[2], texCoord);
		outputColor += texColor * (1.0 - rockInfluence);
	}
	else {
		texColor = texture2D(samplers[0], texCoord);
		outputColor += texColor;
	}

	float diffuseIntensity = max(0.0, dot(normalize(normal), -light.direction));
	outputColor *= color*vec4(light.color*(light.ambientIntensity+diffuseIntensity), 1.0);

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