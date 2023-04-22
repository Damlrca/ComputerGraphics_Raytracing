#version 430

in vec3 glPosition;
out vec4 FragColor;

void main(void) {
	FragColor = vec4(abs(glPosition.xy), 0, 1.0);
}
