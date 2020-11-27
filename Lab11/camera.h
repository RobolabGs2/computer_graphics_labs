#pragma once

#include "point.h"
#include "entity.h"

#include <Windows.h>
#include <gl\freeglut.h>

class Camera
{
public:
	Entity* parent;
	float aspectRatio = 1;

	Camera(Entity* parent = nullptr) :
		parent(parent)
	{ }

	virtual void TransformGL()
	{
		glMatrixMode(GL_PROJECTION);
		gluPerspective(65.0f, aspectRatio, 0.0f, 1000.0f);
		if (parent)
			parent->ReTransformGL();
	}
};
