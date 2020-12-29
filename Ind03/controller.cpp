#include "controller.h"
#include "game.h"
#include "geometry.h"

#include <Windows.h>
#include <gl\freeglut.h>
#include <cmath>


//	*****************************************
//	**            Controller               **
//	*****************************************

Controller::Controller(Game& game) :
	game(game)
{ }

void Controller::Tick(double dt)
{
	GarbageCollector::Tick();
	for (Control* m : data)
		m->Tick(dt);
}

CarUser* Controller::AddCarUser(DynamicCylinder* parent, SpotLight* headlight)
{
	CarUser* result = new CarUser(game, parent, headlight);
	AddTracking(result);
	return result;
}

LightContriller* Controller::AddLightContriller(SpotLight* headlight)
{
	LightContriller* result = new LightContriller(game, headlight);
	AddTracking(result);
	return result;
}

SimpleUser* Controller::AddSimpleUser(Entity* parent)
{
	SimpleUser* result = new SimpleUser(game, parent);
	AddTracking(result);
	return result;
}


//	*****************************************  //
//	**              CarUser                **  //
//	*****************************************  //

CarUser::CarUser(Game& game, DynamicCylinder* parent, SpotLight* headlight) :
	game(game), parent(parent), headlight(headlight)
{ }

void CarUser::Tick(double dt)
{
	if (!(alive = parent->alive) &&
		!(alive = parent->parent->alive)) return;

	double radYAngle = parent->parent->yAngle * 2 * PI / 360;
	if (game.keys.left)
		parent->parent->yAngle -= 5 * dt *
		(parent->velocity.z * std::cos(radYAngle) + parent->velocity.x * std::sin(radYAngle));
	if (game.keys.right)
		parent->parent->yAngle += 5 * dt *
		(parent->velocity.z * std::cos(radYAngle) + parent->velocity.x * std::sin(radYAngle));

	dt *= 10;
	if (game.keys.up)
	{
		parent->force.z += dt * std::cos(radYAngle);
		parent->force.x += dt * std::sin(radYAngle);
	}

	if (game.keys.down)
	{
		parent->force.z -= dt * std::cos(radYAngle);
		parent->force.x -= dt * std::sin(radYAngle);
	}

	if (game.keys.headlamp)
	{
		headlight->enable = !headlight->enable;
		game.keys.headlamp = false;
	}
}


//	*****************************************  //
//	**           LightContriller           **  //
//	*****************************************  //

LightContriller::LightContriller(Game& game, SpotLight* light) :
	game(game), light(light)
{ }

void LightContriller::Tick(double dt)
{
	if (game.keys.lantern)
	{
		light->enable = !light->enable;
		game.keys.lantern = false;
	}
}


//	*****************************************  //
//	**             SimpleUser              **  //
//	*****************************************  //

SimpleUser::SimpleUser(Game& game, Entity* parent) :
	game(game), parent(parent)
{ }

void SimpleUser::Tick(double dt)
{
	if (!(alive = parent->alive)) return;

	double radYAngle = parent->yAngle * 2 * PI / 360;
	if (game.keys.left) parent->yAngle += 50 * dt;
	if (game.keys.right) parent->yAngle -= 50 * dt;

	float k = 50;
	if (game.keys.up)
	{
		parent->location.z += k * dt * std::cos(radYAngle);
		parent->location.x += k * dt * std::sin(radYAngle);
	}

	if (game.keys.down)
	{
		parent->location.z -= k * dt * std::cos(radYAngle);
		parent->location.x -= k * dt * std::sin(radYAngle);
	}
}

