#pragma once

#include "garbage_collector.h"
#include "geometry.h"


class Entity : public Garbage
{
public:
	Point location;
	float xAngle = 0;
	float yAngle = 0;
	float zAngle = 0;

	Entity(Point location);
	virtual Matrix Transform();
	virtual Matrix ReTransform();
};

class TailEntity : public Entity
{
public:
	Entity* parent;
	TailEntity(Entity* parent, Point location);

	virtual Matrix Transform() override;
	virtual Matrix ReTransform() override;
};

class World : public GarbageCollector<Entity>
{
public:
	void Tick(double dt);

	Entity* AddEntity(Point location);
	TailEntity* AddTailEntity(Entity* parent, Point location);
};
