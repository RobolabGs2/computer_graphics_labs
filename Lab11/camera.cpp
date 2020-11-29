#include "camera.h"

#include <Windows.h>
#include <gl\freeglut.h>


Camera::Camera(Entity* parent) :
	parent(parent)
{ }

void Camera::TransformGL()
{
	glMatrixMode(GL_PROJECTION);
	gluPerspective(65.0f, aspectRatio, 0.1f, 1000.0f);
	if (parent)
		parent->ReTransformGL();
}
