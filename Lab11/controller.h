#pragma once

#include "garbage_collector.h"
#include "world.h"
#include "physics.h"

class Game;

struct Control: public Garbage
{ 
	virtual void Tick(double dt) = 0;
};

struct SimpleUser : public Control
{
	Game& game;
	DynamicCylinder* parent;

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

class Controller : public GarbageCollector<Control>
{
public:
	Game& game;

	Controller(Game& game);
	void Tick(double dt);
	SimpleUser* AddSimpleUser(DynamicCylinder* parent);
	Bullet* AddBullet(DynamicCylinder* parent);
	Whirligig* AddWhirligig(Entity* parent);
	Suicidal* AddSuicidal(Entity* parent, double ttl);
};
