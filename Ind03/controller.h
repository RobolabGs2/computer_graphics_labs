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

class Controller : public GarbageCollector<Control>
{
public:
	Game& game;

	Controller(Game& game);
	void Tick(double dt);

	CarUser* AddCarUser(DynamicCylinder* parent, SpotLight* headlight);
	LightContriller* AddLightContriller(SpotLight* light);
	SimpleUser* AddSimpleUser(Entity* parent);
};
