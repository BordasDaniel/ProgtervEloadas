#version 330 core
// Full-screen quad position and matching coordinate for sampling the scene texture.
layout(location = 0) in vec2 aPos;
layout(location = 1) in vec2 aTexCoords;

// Pass texture coordinates to the post-process fragment shader.
out vec2 vTexCoords;

void main()
{
	// Quad positions are already in clip space, so no camera transform is needed.
	vTexCoords = aTexCoords;
	gl_Position = vec4(aPos.x, aPos.y, 0.0, 1.0);
}