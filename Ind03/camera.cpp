#include "camera.h"

#include <GL\glew.h>
#include <gl\freeglut.h>


Camera::Camera(Entity* parent) :
	parent(parent)
{ }

Matrix Camera::Transform()
{
	if (parent && parent->alive)
		return parent->ReTransform();
	return Matrix::Ident();
}

Matrix Camera::Projection()
{
	return Matrix::Projection(0, 0, 1) * Matrix::Scale(1, aspectRatio, 1);
}
