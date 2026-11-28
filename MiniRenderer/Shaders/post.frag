#version 330 core
// Texture coordinates interpolated across the full-screen quad.
in vec2 vTexCoords;

// Final post-processed pixel color.
out vec4 FragColor;

// Color image rendered by the first (cube) pass.
uniform sampler2D uScreenTexture;

void main()
{
	// Read the scene color, then invert it to create the demo's negative-color effect.
	vec3 col = texture(uScreenTexture, vTexCoords).rgb;
	vec3 inverted = vec3(1.0) - col;

	// Darken pixels farther from the image center to create a vignette.
	float dist = distance(vTexCoords, vec2(0.5));
	inverted *= smoothstep(0.8, 0.5, dist);

	FragColor = vec4(inverted, 1.0);

}