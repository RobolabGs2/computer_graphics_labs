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

struct Tank : public Control
{
	Game& game;
	DynamicCylinder* parent;
	Entity* user;
	double delay = 0;

	Tank(Game& game, DynamicCylinder* parent, Entity* user);
	void Tick(double dt) override;
};

struct Gun : public Control
{
	Game& game;
	DynamicCylinder* parent;
	Entity* user;
	float speed_mult = 0;
	double delay = 0;

	Gun(Game& game, DynamicCylinder* parent, Entity* user);
	void Tick(double dt) override;
};

struct Bus : public Control
{
	Game& game;
	DynamicCylinder* parent;
	Entity* user;
	double delay = 0;

	Bus(Game& game, DynamicCylinder* parent, Entity* user);
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

struct FallSuicidal : public Control
{
	Entity* parent;

	FallSuicidal(Entity* parent);
	void Tick(double dt) override;
};

struct Lantern : public Control
{
	Game& game;
	StaticCube* parent;
	bool on_off = false;

	Lantern(Game& game, StaticCube* parent);
	void Tick(double dt) override;
};

class Controller : public GarbageCollector<Control>
{
public:
	Game& game;
	SpotLight* light;
	std::vector<SpotLight*> lanternLight;
	bool on = false;
	int onn = 0;

	Controller(Game& game);
	void Tick(double dt);

	SimpleUser* AddSimpleUser(DynamicCylinder* parent);
	Tank* AddTank(DynamicCylinder* parent, Entity* user);
	Gun* AddGun(DynamicCylinder* parent, Entity* user);
	Bus* AddBus(DynamicCylinder* parent, Entity* user);
	Bullet* AddBullet(DynamicCylinder* parent);
	Whirligig* AddWhirligig(Entity* parent);
	Suicidal* AddSuicidal(Entity* parent, double ttl);
	FallSuicidal* AddFallSuicidal(Entity* parent);
	Lantern* AddLantern(StaticCube* parent);

};
