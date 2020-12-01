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

Tank* Controller::AddTank(DynamicCylinder* parent, Entity* user)
{
	Tank* result = new Tank(game, parent, user);
	AddTracking(result);
	return result;
}

Gun* Controller::AddGun(DynamicCylinder* parent, Entity* user)
{
	Gun* result = new Gun(game, parent, user);
	AddTracking(result);
	return result;
}

Bus* Controller::AddBus(DynamicCylinder* parent, Entity* user)
{
	Bus* result = new Bus(game, parent, user);
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

FallSuicidal* Controller::AddFallSuicidal(Entity* parent)
{
	FallSuicidal* result = new FallSuicidal(parent);
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
			Point{ -(float)std::sin(radYAngle), 0, -(float)std::cos(radYAngle) } *2 +
			Point{ -(float)std::cos(radYAngle), 0, (float)std::sin(radYAngle) } *0.7,
			parent->parent->yAngle);

		game.AddBullet(parent->parent->location +
			Point{ -(float)std::sin(radYAngle), 0, -(float)std::cos(radYAngle) } *2 +
			Point{ (float)std::cos(radYAngle), 0, -(float)std::sin(radYAngle) } *0.7,
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
//	**			     Tank                  **  //
//	*****************************************  //

Tank::Tank(Game& game, DynamicCylinder* parent, Entity* user) :
	game(game), parent(parent), user(user)
{ }

void Tank::Tick(double dt)
{
	if (!(alive = parent->alive) &&
		!(alive = parent->parent->alive)) return;


	double radYAngle = parent->parent->yAngle * 2 * PI / 360;
	Point user_location = user->alive ? user->location : Point{ 0, 0, 0 };
	Point user_delta = user_location - parent->parent->location;
	double y_len = std::sqrt(user_delta.x * user_delta.x + user_delta.z * user_delta.z);

	float angle_vel = (-parent->velocity.z * std::sin(radYAngle) + parent->velocity.x * std::cos(radYAngle));
	parent->parent->zAngle = -5 * angle_vel;

	double user_angle = PI - std::acos(user_delta.z / y_len) * Signum(user_delta.x);
	parent->parent->yAngle += (radYAngle - user_angle > 0) ? -1 : 1;

	parent->force.z -= 10 * dt * std::cos(radYAngle);
	parent->force.x -= 10 * dt * std::sin(radYAngle);

	delay += dt;
	if (delay > 1 && std::abs(radYAngle - user_angle) < 1)
	{
		delay = 0;
		game.AddBullet(parent->parent->location +
			Point{ -(float)std::sin(radYAngle), 0, -(float)std::cos(radYAngle) } *2,
			parent->parent->yAngle);
	}
}

//	*****************************************  //
//	**			      Gun                  **  //
//	*****************************************  //

Gun::Gun(Game& game, DynamicCylinder* parent, Entity* user) :
	game(game), parent(parent), user(user)
{ }

void Gun::Tick(double dt)
{
	if (!(alive = parent->alive) &&
		!(alive = parent->parent->alive)) return;

	double radYAngle = parent->parent->yAngle * 2 * PI / 360;
	Point user_location = user->alive ? user->location : Point{ 0, 0, 0 };
	Point user_delta = user_location - parent->parent->location;
	double y_len = std::sqrt(user_delta.x * user_delta.x + user_delta.z * user_delta.z);

	float angle_vel = (-parent->velocity.z * std::sin(radYAngle) + parent->velocity.x * std::cos(radYAngle));
	parent->parent->zAngle = -5 * angle_vel;

	double user_angle = PI - std::acos(user_delta.z / y_len) * Signum(user_delta.x);
	parent->parent->yAngle += (radYAngle - user_angle > 0) ? -1 : 1;

	parent->force.z -= speed_mult * dt * std::cos(radYAngle);
	parent->force.x -= speed_mult * dt * std::sin(radYAngle);

	delay += dt;
	if (delay > 3 && std::abs(radYAngle - user_angle) < 1)
	{
		speed_mult = 5 * (rand() * 2.0 / RAND_MAX - 1);
		delay = 0;
		game.AddBullet(parent->parent->location +
			Point{ -(float)std::sin(radYAngle), 0, -(float)std::cos(radYAngle) } *2,
			parent->parent->yAngle, 0.2, y_len * 0.8, 50);
	}
}


//	*****************************************  //
//	**			      Bus                  **  //
//	*****************************************  //

Bus::Bus(Game& game, DynamicCylinder* parent, Entity* user) :
	game(game), parent(parent), user(user)
{ }

void Bus::Tick(double dt)
{
	if (!(alive = parent->alive) &&
		!(alive = parent->parent->alive)) return;

	double radYAngle = parent->parent->yAngle * 2 * PI / 360;
	Point user_location = user->alive ? user->location : Point{ 0, 0, 0 };
	Point user_delta = user_location - parent->parent->location;
	double y_len = std::sqrt(user_delta.x * user_delta.x + user_delta.z * user_delta.z);

	double user_angle = PI - std::acos(user_delta.z / y_len) * Signum(user_delta.x);
	parent->parent->yAngle += ((radYAngle - user_angle > 0) ? -10 : 10) / (std::abs(radYAngle - user_angle) < 0.1 ? 20 : 1);

	float angle_vel = (-parent->velocity.z * std::sin(radYAngle) + parent->velocity.x * std::cos(radYAngle));
	parent->parent->zAngle = -5 * angle_vel;

	parent->force.z -= 30 * dt * std::cos(radYAngle);
	parent->force.x -= 30 * dt * std::sin(radYAngle);

	if (parent->lastDCylinderCollision != nullptr && parent->lastDCylinderCollision->alive) {
		parent->lastDCylinderCollision->parent->alive = false;
		parent->parent->alive = false;
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
		game.graphics.AddSphere(part, 0.05, {
							  {0.235, 0.294, 0.431},
							  {0.235, 0.294, 0.431},
							  {0.3500, 0.3500, 0.3500},
							  static_cast<signed char>(16.0 / 1000 * 128),
							  {0.9, 0.4, 0},
			});
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
//	**           FallSuicidal              **  //
//	*****************************************  //

FallSuicidal::FallSuicidal(Entity* parent) :
	parent(parent)
{ }

void FallSuicidal::Tick(double dt)
{
	if (!(alive = parent->alive)) return;
	if (parent->location.y < -100)
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