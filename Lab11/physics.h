#pragma once

#include "garbage_collector.h"
#include "world.h"

#include <vector>

struct ABBA
{
	Point minPoint;
	Point maxPoint;
};

struct Body;
class StaticCube;
class DynamicCylinder;

struct Body : public Garbage
{
	Entity* parent;
	Body(Entity* parent);
	virtual void Tick(double dt);
	virtual ABBA Abba() = 0;

	virtual void Hit(Body* body) = 0;
	virtual void Hit(StaticCube* body) = 0;
	virtual void Hit(DynamicCylinder* body) = 0;
};

class StaticCube : public Body
{
public:
	Point size;

	StaticCube(Entity* parent, Point size);

	void Tick(double dt) override;
	ABBA Abba() override;
	void Hit(Body* body) override;
	void Hit(StaticCube* body) override;
	void Hit(DynamicCylinder* body) override;
};

class DynamicCylinder : public Body
{
public:
	Point velocity = {0, 0, 0};
	Point force = {0, 0, 0};
	Point friction = {0.1, 0.1, 0.1 };

	//	костыли)
	DynamicCylinder* lastDCylinderCollision = nullptr;
	StaticCube* lastSCubeCollision = nullptr;

	float radius;
	float height;
	bool gravity = true;
	
	DynamicCylinder(Entity* parent, float radius, float height);

	void Tick(double dt) override;
	ABBA Abba() override;
	void Hit(Body* body) override;
	void Hit(StaticCube* body) override;
	void Hit(DynamicCylinder* body) override;
};


class Physics : public GarbageCollector<Body>
{
	std::vector<Body*> sortedBodyes;
public:
	void Tick(double dt);
	StaticCube* AddStaticCube(Entity* parent, Point size);
	DynamicCylinder* AddDynamicCylinder(Entity* parent, float radius, float height);
};
