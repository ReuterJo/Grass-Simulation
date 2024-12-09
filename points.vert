#version 330 compatibility
// use 120 for the Mac

void
main( )
{ 
	gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
}
