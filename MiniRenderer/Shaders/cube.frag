#version 330 core
// Interpolated color from the cube's vertex data.
in vec3 vColor;

// Final color written for this fragment (pixel).
out vec4 FragColor;

void main()
{
	// Use the face color unchanged and make the cube fully opaque.
	FragColor = vec4(vColor, 1.0);
}
