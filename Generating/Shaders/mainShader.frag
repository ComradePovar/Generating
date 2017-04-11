#version 430

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

uniform sampler2D samplers[2];
uniform vec4 color;
uniform DirectionalLight light;
uniform Fog fog;

float getFogFactor(Fog fog, float coord);

void main()
{
	outputColor = vec4(0.0, 0.0, 0.0, 0.0);

	vec4 texColor = texture2D(samplers[0], texCoord);
	outputColor += texColor * normalizedHeight;

	texColor = texture2D(samplers[1], texCoord);
	outputColor += texColor * (1.0 - normalizedHeight);

	float diffuseIntensity = max(0.0, dot(normalize(normal), -light.direction));
	outputColor *= color*vec4(light.color*(light.ambientIntensity+diffuseIntensity), 1.0);

	float fogCoord = abs(eyeSpacePosition.z / eyeSpacePosition.w);
	//Fog, don't need yet
	//outputColor = mix(outputColor, fog.color, getFogFactor(fog, fogCoord));
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