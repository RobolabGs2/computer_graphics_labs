#include "controller.h"
#include "game.h"
#include "common.h"

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

SimpleUser* Controller::AddSimpleUser(DynamicCylinder* parent)
{
	SimpleUser* result = new SimpleUser(game, parent);
	AddTracking(result);
	return result;
}

Bullet* Controller::AddBullet(DynamicCylinder* parent)
{
	Bullet* result = new Bullet(game, parent);
	AddTracking(result);
	return result;
}

Whirligig* Controller::AddWhirligig(Entity* parent)
{
	Whirligig* result = new Whirligig(parent);
	AddTracking(result);
	return result;
}

Suicidal* Controller::AddSuicidal(Entity* parent, double ttl)
{
	Suicidal* result = new Suicidal(parent, ttl);
	AddTracking(result);
	return result;
}

Lantern* Controller::AddLantern(StaticCube* parent)
{
	Lantern* result = new Lantern(game, parent);
	AddTracking(result);
	return result;
}

//	*****************************************  //
//	**            SimpleUser               **  //
//	*****************************************  //

SimpleUser::SimpleUser(Game& game, DynamicCylinder* parent) :
	game(game), parent(parent)
{ }

void SimpleUser::Tick(double dt)
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
		parent->force.z -= dt * std::cos(radYAngle);
		parent->force.x -= dt * std::sin(radYAngle);
	}

	if (game.keys.down)
	{
		parent->force.z += dt * std::cos(radYAngle);
		parent->force.x += dt * std::sin(radYAngle);
	}

	if (game.keys.space)
	{
		game.keys.space = false;
		double radYAngle = parent->parent->yAngle * 2 * PI / 360;
		game.AddBullet(parent->parent->location + 
			Point{ -(float)std::sin(radYAngle), 0, -(float)std::cos(radYAngle) } * 2,
			parent->parent->yAngle);
	}

	if (game.keys.headlamp)
	{
		game.keys.headlamp = false;
		on_off = !on_off;
		if (alive = game.controller.light->alive)
			game.controller.light->enable = on_off;
	}
}

//	*****************************************  //
//	**              Bullet                 **  //
//	*****************************************  //

Bullet::Bullet(Game& game, DynamicCylinder* parent) :
	game(game),
	parent(parent)
{ }

void Bullet::Tick(double dt)
{
	if (!(alive = parent->alive)|| !(alive = parent->parent->alive)) return;

	Entity* part = game.world.AddEntity(Point{ parent->parent->location } +
		(Point{ (float)(rand() % 1000) , (float)(rand() % 1000) , (float)(rand() % 1000) } - Point{500, 500, 500}) / 2000.0); {
		game.controller.AddSuicidal(part, 0.3);
		game.graphics.AddSphere(part, 0.05);
	}

	if (parent->lastDCylinderCollision != nullptr)
	{
		parent->parent->alive = false;
		parent->alive = false;
		alive = false;

		if (parent->lastDCylinderCollision->alive && parent->lastDCylinderCollision->parent->alive)
			parent->lastDCylinderCollision->parent->alive = false;
	}
}

//	*****************************************  //
//	**            Whirligig                **  //
//	*****************************************  //

Whirligig::Whirligig(Entity* parent) :
	parent(parent)
{ }

void Whirligig::Tick(double dt)
{
	if (!(alive = parent->alive)) return;

	parent->yAngle += dt * 100;
}

//	*****************************************  //
//	**             Suicidal                **  //
//	*****************************************  //

Suicidal::Suicidal(Entity* parent, double ttl) :
	parent(parent),
	ttl(ttl)
{ }

void Suicidal::Tick(double dt)
{
	if (!(alive = parent->alive)) return;
	ttl -= dt;
	if (ttl < 0)
		parent->alive = false;
}

//	*****************************************  //
//	**            Lantern               **  //
//	*****************************************  //

Lantern::Lantern(Game& game, StaticCube* parent) :
	game(game), parent(parent)
{ }

void Lantern::Tick(double dt)
{
	if (!(alive = parent->alive)) return;

	if (game.keys.lantern)
	{
		game.keys.lantern = false;
		on_off = !on_off;
		if (alive = game.controller.light->alive)
			game.controller.lanternLight->enable = on_off;
	}
}