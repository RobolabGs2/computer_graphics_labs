#pragma once

#include "garbage_collector.h"
#include "world.h"
#include "physics.h"
#include "illumination.h"

class Game;

struct Control: public Garbage
{ 
	virtual void Tick(double dt) = 0;
};

struct SimpleUser : public Control
{
	Game& game;
	DynamicCylinder* parent;
	bool on_off = true;

	SimpleUser(Game& game, DynamicCylinder* parent);
	void Tick(double dt) override;
};

struct Bullet : public Control
{
	Game& game;
	DynamicCylinder* parent;

	Bullet(Game& game, DynamicCylinder* parent);
	void Tick(double dt) override;
};

struct Whirligig : public Control
{
	Entity* parent;

	Whirligig(Entity* parent);
	void Tick(double dt) override;
};

struct Suicidal : public Control
{
	Entity* parent;
	double ttl;

	Suicidal(Entity* parent, double ttl);
	void Tick(double dt) override;
};

struct Lantern : public Control
{
	Game& game;
	StaticCube* parent;
	bool on_off = true;

	Lantern(Game& game, StaticCube* parent);
	void Tick(double dt) override;
};

class Controller : public GarbageCollector<Control>
{
public:
	Game& game;
	SpotLight* light;
	SpotLight* lanternLight;

	Controller(Game& game);
	void Tick(double dt);

	SimpleUser* AddSimpleUser(DynamicCylinder* parent);
	Bullet* AddBullet(DynamicCylinder* parent);
	Whirligig* AddWhirligig(Entity* parent);
	Suicidal* AddSuicidal(Entity* parent, double ttl);
	Lantern* AddLantern(StaticCube* parent);

};
