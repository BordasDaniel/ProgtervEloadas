#version 330 core
// Vertex attribute locations match the layout configured when the cube VAO is created.
layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aColor;

// Pass the vertex color to the fragment shader for interpolation across each triangle.
out vec3 vColor;

// Transforms supplied by the CPU for this frame.
uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

void main()
{
	vColor = aColor;
	// Convert the vertex from model space through view and projection into clip space.
	gl_Position = uProjection * uView * uModel * vec4(aPos, 1.0);
}