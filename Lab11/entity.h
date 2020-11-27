#pragma once

#include "garbage_collector.h"
#include "point.h"

#include <Windows.h>
#include <gl/GL.h>

class Entity: public Garbage
{
public:
	Point location;
	float xAngle = 0;
	float yAngle = 0;
	float zAngle = 0;

	Entity(Point location):
		location(location)
	{ }

	virtual void TransformGL()
	{
		glTranslated(location.x, location.y, location.z);
		glRotatef(xAngle, 1, 0, 0);
		glRotatef(yAngle, 0, 1, 0);
		glRotatef(zAngle, 0, 0, 1);
	}

	virtual void ReTransformGL()
	{
		glRotatef(-zAngle, 0, 0, 1);
		glRotatef(-yAngle, 0, 1, 0);
		glRotatef(-xAngle, 1, 0, 0);
		glTranslated(-location.x, -location.y, -location.z);
	}
};

class TailEntity: public Entity
{
public:
	Entity* parent;
	TailEntity(Entity* parent, Point location) :
		Entity(location),
		parent(parent)
	{ }

	virtual void TransformGL() override
	{
		if (!(alive = parent->alive)) return;
		parent->TransformGL();
		Entity::TransformGL();
	}

	virtual void ReTransformGL() override
	{
		if (!(alive = parent->alive)) return;
		Entity::ReTransformGL();
		parent->ReTransformGL();
	}
};

class World;
