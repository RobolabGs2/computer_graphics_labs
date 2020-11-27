#pragma once

#include "garbage_collector.h"
#include "physics.h"
#include "entity.h"

#include <Windows.h>
#include <gl\freeglut.h>
#include <cmath>

struct Keyboard
{
	bool left = false;
	bool right = false;
	bool up = false;
	bool down = false;
};

struct Control: public Garbage
{ 
	virtual void Tick(double dt) = 0;
};

struct SimpleUser: public Control
{
	const Keyboard& keys;
	Entity* entity;
	SimpleUser(const Keyboard& keys, Entity* entity):
		keys(keys), entity(entity)
	{ }

	void Tick(double dt) override
	{
		if (!(alive = entity->alive)) return;
		
		if (keys.left) entity->yAngle += 100 * dt;
		if (keys.right) entity->yAngle -= 100 * dt;
		
		double radYAngle = entity->yAngle * 2 * PI / 360;
		dt *= 10;
		if (keys.up)
		{
			entity->location.z -= dt * std::cos(radYAngle);
			entity->location.x -= dt * std::sin(radYAngle);
		}

		if (keys.down)
		{
			entity->location.z += dt * std::cos(radYAngle);
			entity->location.x += dt * std::sin(radYAngle);
		}

	}
};

class Controller : public GarbageCollector<Control>
{
public:
	void Tick(double dt)
	{
		GarbageCollector::Tick();
		for (Control* m : data)
			m->Tick(dt);
	}

	SimpleUser* AddSimpleUser(const Keyboard& keys, Entity* entity)
	{
		SimpleUser* result = new SimpleUser(keys, entity);
		AddTracking(result);
		return result;
	}

};
