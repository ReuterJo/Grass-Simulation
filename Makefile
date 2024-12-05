tessellation:	tessellation.cpp
		g++   -o tessellation   tessellation.cpp  -I. -lGL -lGLU -lglut  -lm -lGLEW


save:
		cp tessellation.cpp tessellation.save.cpp
