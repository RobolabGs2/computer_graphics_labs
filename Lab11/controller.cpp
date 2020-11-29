#include "controller.h"
#include "game.h"
#include "common.h"

#include <Windows.h>
#include <gl\freeglut.h>
#include <cmath>


//	*****************************************
//	**            Controller               **
//	*****************************************

Controller::Controller(Game& game):
	game(game)
{ }

void Controller::Tick(double dt)
{
	GarbageCollector::Tick();
	for (Control* m : data)
		m->Tick(dt);
}

SimpleUser* Controller::AddSimpleUser(Entity* entity)
{
	SimpleUser* result = new SimpleUser(game, entity);
	AddTracking(result);
	return result;
}

Bullet* Controller::AddBullet(Entity* entity)
{
	Bullet* result = new Bullet(entity);
	AddTracking(result);
	return result;
}

Whirligig* Controller::AddWhirligig(Entity* entity)
{
	Whirligig* result = new Whirligig(entity);
	AddTracking(result);
	return result;
}


//	*****************************************  //
//	**            SimpleUser               **  //
//	*****************************************  //

SimpleUser::SimpleUser(Game& game, Entity* entity) :
	game(game), entity(entity)
{ }

void SimpleUser::Tick(double dt)
{
	if (!(alive = entity->alive)) return;

	if (game.keys.left) entity->yAngle += 100 * dt;
	if (game.keys.right) entity->yAngle -= 100 * dt;

	double radYAngle = entity->yAngle * 2 * PI / 360;
	dt *= 10;
	if (game.keys.up)
	{
		entity->location.z -= dt * std::cos(radYAngle);
		entity->location.x -= dt * std::sin(radYAngle);
	}

	if (game.keys.down)
	{
		entity->location.z += dt * std::cos(radYAngle);
		entity->location.x += dt * std::sin(radYAngle);
	}

	if (game.keys.space)
	{
		game.keys.space = false;
		game.AddBullet(entity->location, entity->yAngle);
	}
}

//	*****************************************  //
//	**              Bullet                 **  //
//	*****************************************  //

Bullet::Bullet(Entity* entity) :
	entity(entity)
{ }

void Bullet::Tick(double dt)
{
	if (!(alive = entity->alive)) return;

	double radYAngle = entity->yAngle * 2 * PI / 360;
	dt *= 50;
	entity->location.z -= dt * std::cos(radYAngle);
	entity->location.x -= dt * std::sin(radYAngle);
}

//	*****************************************  //
//	**            Whirligig                **  //
//	*****************************************  //

Whirligig::Whirligig(Entity* entity) :
	entity(entity)
{ }

void Whirligig::Tick(double dt)
{
	if (!(alive = entity->alive)) return;

	entity->yAngle += dt * 100;
}
