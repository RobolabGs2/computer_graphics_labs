#pragma once

#include "garbage_collector.h"
#include "world.h"
#include "physics.h"
#include "illumination.h"

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
	SpotLight* headlight;

	CarUser(Game& game, DynamicCylinder* parent, SpotLight* headlight);
	void Tick(double dt) override;
};

struct LightContriller : public Control
{
	Game& game;
	SpotLight* light;

	LightContriller(Game& game, SpotLight* light);
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

class Controller : public GarbageCollector<Control>
{
public:
	Game& game;

	Controller(Game& game);
	void Tick(double dt);

	CarUser* AddCarUser(DynamicCylinder* parent, SpotLight* headlight);
	LightContriller* AddLightContriller(SpotLight* light);
	SimpleUser* AddSimpleUser(Entity* parent);

	Tank* AddTank(DynamicCylinder* parent, Entity* user);
	Gun* AddGun(DynamicCylinder* parent, Entity* user);
	Bus* AddBus(DynamicCylinder* parent, Entity* user);
	Bullet* AddBullet(DynamicCylinder* parent);
	Whirligig* AddWhirligig(Entity* parent);
	Suicidal* AddSuicidal(Entity* parent, double ttl);
	FallSuicidal* AddFallSuicidal(Entity* parent);
};
