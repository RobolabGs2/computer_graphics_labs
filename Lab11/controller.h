#pragma once

#include "garbage_collector.h"
#include "world.h"

class Game;

struct Control: public Garbage
{ 
	virtual void Tick(double dt) = 0;
};

struct SimpleUser : public Control
{
	Game& game;
	Entity* entity;

	SimpleUser(Game& game, Entity* entity);
	void Tick(double dt) override;
};

struct Bullet : public Control
{
	Entity* entity;

	Bullet(Entity* entity);
	void Tick(double dt) override;
};

class Controller : public GarbageCollector<Control>
{
public:
	Game& game;

	Controller(Game& game);
	void Tick(double dt);
	SimpleUser* AddSimpleUser(Entity* entity);
	Bullet* AddBullet(Entity* entity);
};
