#include "graphics.h"

#include <Windows.h>
#include <gl\freeglut.h>



//	*****************************************
//	**             Graphics                **
//	*****************************************

void Graphics::Tick(double dt)
{
	GarbageCollector::Tick();
	for (Mesh* m : data)
		m->Tick(dt);
}

Cube* Graphics::AddCube(Entity* parent, double size)
{
	Cube* result = new Cube(parent, size);
	GarbageCollector::AddTracking(result);
	return result;
}

Sphere* Graphics::AddSphere(Entity* parent, double radius)
{
	Sphere* result = new Sphere(parent, radius);
	GarbageCollector::AddTracking(result);
	return result;
}

Cone* Graphics::AddCone(Entity* parent, double base, double height)
{
	Cone* result = new Cone(parent, base, height);
	GarbageCollector::AddTracking(result);
	return result;
}

Torus* Graphics::AddTorus(Entity* parent, double innerRadius, double outerRadius)
{
	Torus* result = new Torus(parent, innerRadius, outerRadius);
	GarbageCollector::AddTracking(result);
	return result;
}


//	*****************************************
//	**               Mesh                  **
//	*****************************************

Mesh::Mesh(Entity* parent) :
	parent(parent)
{ }

void Mesh::Tick(double dt)
{
	if (!(alive = parent->alive))
		return;
	glPushMatrix();
	parent->TransformGL();
	Draw();
	glPopMatrix();
}


//	*****************************************
//	**               Cube                  **
//	*****************************************

Cube::Cube(Entity* parent, float size) :
	Mesh(parent),
	size(size)
{ }

void Cube::Draw()
{
	glutWireCube(size);
}


//	*****************************************
//	**              Sphere                 **
//	*****************************************

Sphere::Sphere(Entity* parent, double radius) :
	Mesh(parent),
	radius(radius)
{ }

void Sphere::Draw()
{
	glutWireSphere(radius, 10, 10);
}


//	*****************************************
//	**               Cone                  **
//	*****************************************

Cone::Cone(Entity* parent, double base, double height) :
	Mesh(parent),
	base(base),
	height(height)
{ }

void Cone::Draw()
{
	glutWireCone(base, height, 10, 10);
}


//	*****************************************
//	**              Torus                  **
//	*****************************************

Torus::Torus(Entity* parent, double innerRadius, double outerRadius) :
	Mesh(parent),
	innerRadius(innerRadius),
	outerRadius(outerRadius)
{ }

void Torus::Draw()
{
	glutWireTorus(innerRadius, outerRadius, 10, 30);
}
