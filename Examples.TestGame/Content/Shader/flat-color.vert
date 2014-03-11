#version 130

precision highp float;
precision highp int;

uniform mat4 MVP;
in vec3 Position;

void main()
{	
	gl_Position = MVP * vec4(Position, 1.0);
}
