#version 330 core
in vec2 vTexCoords;
out vec4 FragColor;

uniform sampler2D uScreenTexture;

void main()
{
	vec3 col = texture(uScreenTexture, vTexCoords).rgb;
	
	// Invert
	vec3 inverted = vec3(1.0) - col;

	// Vignette
	float dist = distance(vTexCoords, vec2(0.5));
	inverted *= smoothstep(0.8, 0.5, dist);

	FragColor = vec4(inverted, 1.0);

}