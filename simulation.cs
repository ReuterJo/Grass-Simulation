#version 430 compatibility
#extension GL_ARB_compute_shader:			enable
#extension GL_ARB_shader_storage_buffer_object:		enable

layout( std140, binding=4 ) buffer Pos
{
	vec4 Positions[ ];
};

layout( local_size_x = 128, local_size_y = 1, local_size_z = 1 ) in;

const vec3 G	= vec3( 0., -9.8, 0. );
const float DT	= 0.1;
const float PI = 3.14159265;
const float TWOPI = 2. * PI;

uniform float mass;
uniform float stiffness;
uniform float ymin;
uniform float ymax;
uniform float time;
uniform int windActive;

void
main( )
{
	uint gid = gl_GlobalInvocationID.x;	// the .y and .z are both 1 in this case

	// operations on v2 of the Bezier curve
	if (gid % 3 == 2)
	{
		// natural forces
		// translation of v2 = (r + g + w)*DT
		
		// gravity
		vec3 gravity = mass * G;	
	
		// recovery
		float x = Positions[ gid - 2 ].x;
		float z = Positions[ gid - 2 ].z;
		float h = ymax - ymin;
		
		vec3 v0 = vec3( x, ymin, z );
		vec3 I = vec3( x, ymin + h, z );
		vec3 recovery = stiffness * ( I - Positions[ gid ].xyz );

		// wind
		vec3 wind = vec3( 0., 0., 0. );
		if ( windActive == 1 )
		{
			vec3 wind1 = vec3( 2., 0., 0. ) * sin(TWOPI*time + v0.x); 
			vec3 wind2 = vec3( 0., 0., 5. ) * cos(TWOPI*time + v0.z + v0.x); 
			vec3 wind3 = vec3( 1., 0., 0. ) * v0.x; 

			wind = wind1 + wind2 + wind3;
		}

		// composition
		vec3 delta = ( gravity + recovery + wind ) * DT;
		vec3 v2 = Positions[ gid ].xyz + delta;

		// state validation
	
		// ensure that v2 is above the local plane	
		vec3 up = vec3( 0., 1., 0. );
		v2 = v2 - up * min( dot(up, (v2 - v0) ), 0. ); 

		// ensure that v0 is never the same as v0
		float lproj = length( v2 - v0 - up * dot( (v2 - v0), up ));
		vec3 v1 = v0 + h * up * max( 1 - lproj / h, 0.05 * max( lproj / h, 1. ));
		
		// ensure that blade stays the same length
		float L0 = length(v2 - v0);
		float L1 = length(v2 - v1) + length(v1 - v0);
		float L = ( 2. * L0 + L1 ) / 3.;

		float r = h / L;

		v1 = v0 + r * ( v1 - v0 );
		v2 = v1 + r * ( v2 - v1 );

		Positions[ gid - 1 ].xyz = v1;
		Positions[ gid ].xyz = v2;	
	}
}
