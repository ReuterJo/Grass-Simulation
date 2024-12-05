grass:		grass.cpp
		g++   -o grass   grass.cpp  -I. -lGL -lGLU -lglut  -lm -lGLEW


save:
		cp grass.cpp grass.save.cpp
