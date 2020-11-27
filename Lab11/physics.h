#pragma once

#include "garbage_collector.h"
#include "entity.h"

#include <Windows.h>
#include <gl\freeglut.h>

struct Body : Garbage
{
	Entity* parent;
	Body(Entity* parent) :
		parent(parent)
	{ }

	virtual void Tick(double dt)
	{
		if (!parent->alive)
			alive = false;
	}
};

class MovingRight : Body
{
	
};

class Physics : public GarbageCollector<Body>
{
public:
	void Tick(double dt)
	{
		GarbageCollector::Tick();

	}

};
