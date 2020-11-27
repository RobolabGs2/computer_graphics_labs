#pragma once

#include "garbage_collector.h"
#include "entity.h"

#include <Windows.h>
#include <gl\freeglut.h>

struct Mesh : public Garbage
{
	Entity* parent;
	Mesh(Entity* parent) :
		parent(parent)
	{ }

	virtual void Tick(double dt) 
	{
		if (!parent->alive)
			alive = false;
	}
};

class Cube : public Mesh
{
	float size;
public:
	Cube(Entity* parent, float size) :
		Mesh(parent),
		size(size)
	{ }

	void Tick(double dt) override
	{
		Mesh::Tick(dt);
		if (!alive) return;

		glPushMatrix();
		parent->TransformGL();
		glutWireCube(size);
		//glutSolidCube(size);
		glPopMatrix();
	}
};

class Sphere : public Mesh
{
	float size;
	double radius;

public:
	Sphere(Entity* parent, double radius) :
		Mesh(parent),
		radius(radius)
	{ }

	void Tick(double dt) override
	{
		Mesh::Tick(dt);
		if (!alive) return;

		glPushMatrix();
		parent->TransformGL();
		glutWireSphere(radius, 10, 10);
		glPopMatrix();
	}
};

class Cone : public Mesh
{
	float size;
	double base;
	double height;

public:
	Cone(Entity* parent, double base, double height) :
		Mesh(parent),
		base(base),
		height(height)
	{ }

	void Tick(double dt) override
	{
		Mesh::Tick(dt);
		if (!alive) return;

		glPushMatrix();
		parent->TransformGL();
		glutWireCone(base, height, 10, 10);
		glPopMatrix();
	}
};


class Torus : public Mesh
{
	float size;
	double innerRadius;
	double outerRadius;

public:
	Torus(Entity* parent, double innerRadius, double outerRadius) :
		Mesh(parent),
		innerRadius(innerRadius),
		outerRadius(outerRadius)
	{ }

	void Tick(double dt) override
	{
		Mesh::Tick(dt);
		if (!alive) return;

		glPushMatrix();
		parent->TransformGL();
		glutWireTorus(innerRadius, outerRadius, 10, 30);
		glPopMatrix();
	}
};


class Graphics: public GarbageCollector<Mesh>
{
public:

	void Tick(double dt)
	{
		GarbageCollector::Tick();
		for (Mesh* m : data)
			m->Tick(dt);
	}

	Cube* AddCube(Entity* parent, double size)
	{
		Cube* result = new Cube(parent, size);
		GarbageCollector::AddTracking(result);
		return result;
	}

	Sphere* AddSphere(Entity* parent, double radius)
	{
		Sphere* result = new Sphere(parent, radius);
		GarbageCollector::AddTracking(result);
		return result;
	}

	Cone* AddCone(Entity* parent, double base, double height)
	{
		Cone* result = new Cone(parent, base, height);
		GarbageCollector::AddTracking(result);
		return result;
	}

	Torus* AddTorus(Entity* parent, double innerRadius, double outerRadius)
	{
		Torus* result = new Torus(parent, innerRadius, outerRadius);
		GarbageCollector::AddTracking(result);
		return result;
	}
};
