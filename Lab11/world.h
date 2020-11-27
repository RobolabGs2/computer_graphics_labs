#pragma once

#include "garbage_collector.h"
#include "point.h"


class Entity : public Garbage
{
public:
	Point location;
	float xAngle = 0;
	float yAngle = 0;
	float zAngle = 0;

	Entity(Point location);
	virtual void TransformGL();
	virtual void ReTransformGL();
};

class TailEntity : public Entity
{
public:
	Entity* parent;
	TailEntity(Entity* parent, Point location);

	virtual void TransformGL() override;
	virtual void ReTransformGL() override;
};

class World : public GarbageCollector<Entity>
{
public:
	void Tick(double dt);

	Entity* AddEntity(Point location);
	TailEntity* AddTailEntity(Entity* parent, Point location);
};
