#version 130

precision highp float;
precision highp int;

uniform mat4 MVP;
in vec3 Position;

void main()
{	
	gl_Position = MVP * vec4(Position, 1.0);
    
    /*
mat4 test = mat4(
    vec4(0.9797959, -0.5656855, 0.8, 0),
    vec4(0, 1.414214, 1, 0),
    vec4(0.7074605, 0.4084525, 345.8325, -1),
    vec4(0.3537302, 0.2042262, -0.2888195, 0)
);
  gl_Position = test * vec4(Position, 1.0);
  */
}
