#version 430 compatibility
#extension GL_ARB_compute_shader:			enable
#extension GL_ARB_shader_storage_buffer_object:		enable

layout( std140, binding=4 ) buffer Pos
{
	vec4 Positions[ ];
};

layout( local_size_x = 128, local_size_y = 1, local_size_z = 1 ) in;

const vec3 C	= vec3( .1, .1, .1 );
const float DT	= 0.01;

void
main( )
{
	uint gid = gl_GlobalInvocationID.x;	// the .y and .z are both 1 in this case

	vec3 p = Positions[ gid ].xyz;

	vec3 pp = p + C*DT;
	
	if (gid % 3 == 2)
	{
		Positions[ gid ].xyz = pp;
	}
	else
	{
		Positions[ gid ].xyz = p;
	}
}
