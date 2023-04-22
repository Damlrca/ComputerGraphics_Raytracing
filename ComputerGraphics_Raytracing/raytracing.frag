#version 330 core
out vec4 FragColor;
in vec3 glPosition;

void main()
{
    FragColor = vec4(abs(glPosition.xy), 0, 1.0);
}
