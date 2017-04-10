#version 450

in vec2 texCoord;
smooth in vec3 normal;

out vec4 outputColor;

struct DirectionalLight
{
	vec3 color;
	vec3 direction;
	float ambientIntensity;
};

uniform sampler2D sampler;
uniform vec4 color;
uniform DirectionalLight light;


void main()
{
	vec4 texColor = texture2D(sampler, texCoord);
	float diffuseIntensity = max(0.0, dot(normalize(normal), -light.direction));
	outputColor = texColor*color*vec4(light.color*(light.ambientIntensity+diffuseIntensity), 1.0);
}