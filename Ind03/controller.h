#pragma once

#include "garbage_collector.h"
#include "world.h"
#include "physics.h"

class Game;

struct Control : public Garbage
{
	virtual void Tick(double dt) = 0;
};


struct SimpleUser : public Control
{
	Game& game;
	Entity* parent;

	SimpleUser(Game& game, Entity* parent);
	void Tick(double dt) override;
};

struct CarUser : public Control
{
	Game& game;
	DynamicCylinder* parent;

	CarUser(Game& game, DynamicCylinder* parent);
	void Tick(double dt) override;
};

class Controller : public GarbageCollector<Control>
{
public:
	Game& game;

	Controller(Game& game);
	void Tick(double dt);

	CarUser* AddCarUser(DynamicCylinder* parent);
	SimpleUser* AddSimpleUser(Entity* parent);
};
