#version 430

struct Fog
{
	vec4 color;
	float start;
	float end;
	float density;
	int type;
};

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