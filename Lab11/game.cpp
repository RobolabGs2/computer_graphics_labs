#include "game.h"

#include <SOIL2.h>

#include "common.h"

#include <Windows.h>
#include <gl\freeglut.h>


//	*****************************************  //
//	**                Game                 **  //
//	*****************************************  //

Game::Game():
	controller(*this)
{
	Init();
}

void Game::Tick(double dt)
{
	if (dt > 0.03)
		dt = 0.03;

	if (!camera.parent->alive)
	{
		AddUserCar({ 0, 2, 10 });
		if (gamemode)
		{
			if (game_bus->alive) game_bus->user = user;
			if (game_tank->alive) game_tank->user = user;
			if (game_gun->alive) game_gun->user = user;
		}
	}

	if (gamemode) {
		if (!game_bus->alive) AddBus({ (float)(rand() % 100 - 50), 2 , float(rand() % 100 - 50) });
		if (!game_tank->alive) AddTank({ (float)(rand() % 100 - 50), 2 , float(rand() % 100 - 50) });
		if (!game_gun->alive) AddGun({ (float)(rand() % 100 - 50), 2 , float(rand() % 100 - 50) });
	}

	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	glClearDepth(1.0f);

	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	camera.TransformGL();
	
	glEnable(GL_LIGHTING);
	glEnable(GL_DEPTH_TEST);

	glMatrixMode(GL_MODELVIEW);
	glLoadIdentity();

	world.Tick(dt);
	physics.Tick(dt);
	graphics.Tick(dt);
	controller.Tick(dt);
	illumination.Tick(dt);

	glFlush(); glutSwapBuffers();
}


void Game::ResizeWindow(int width, int height)
{
	glViewport(0, 0, width, height);
	camera.aspectRatio = width / (float)height;
}

Entity* Game::AddCar(Point location, float rotation)
{
	Entity* car = world.AddEntity(location); {
		car->yAngle = rotation;

		graphics.AddTriangleMesh(car, "Car.obj");
		controller.AddFallSuicidal(car);
	}
	return car;
}

Entity* Game::AddTank(Point location)
{
	Entity* car = world.AddEntity(location); {
		DynamicCylinder* body = physics.AddDynamicCylinder(car, 0.7, 0.7);
		graphics.AddTriangleMesh(car, "Tank.obj");
		if (gamemode) game_tank = controller.AddTank(body, user);
		controller.AddFallSuicidal(car);
	}
	return car;
}


Entity* Game::AddGun(Point location)
{
	Entity* car = world.AddEntity(location); {

		graphics.AddTriangleMesh(car, "Gun.obj");
		DynamicCylinder* body = physics.AddDynamicCylinder(car, 0.7, 0.7);
		if (gamemode) game_gun = controller.AddGun(body, user);
		controller.AddFallSuicidal(car);
	}
	return car;
}


Entity* Game::AddBus(Point location)
{
	Entity* car = world.AddEntity(location); {

		graphics.AddTriangleMesh(car, "Bus.obj");
		DynamicCylinder* body = physics.AddDynamicCylinder(car, 0.7, 1.1);
		if(gamemode) game_bus = controller.AddBus(body, user);
		controller.AddFallSuicidal(car);
	}
	return car;
}

Entity* Game::AddUserCar(Point location)
{
	Entity* car = AddCar(location, 0); {
		DynamicCylinder* body = physics.AddDynamicCylinder(car, 0.7, 0.9); {
			controller.AddSimpleUser(body);
			body->friction = { 0.1, 0, 0.01 };
		}

		Entity* cameraPoint = world.AddTailEntity(car, { 0, 2, 10 }); {
			camera.parent = cameraPoint;
		}

		Entity* lightPoint = world.AddTailEntity(car, { 0, 0, -1 }); {
			controller.light = illumination.AddSpot(lightPoint, { 1, 1, 0.5 }, 30);
		}

		user = car;
	}
	return car;
}

Entity* Game::AddBullet(Point location, float yAngle, float size, float speed, float v_speed)
{
	Entity* bullet = world.AddEntity(location); {
		bullet->yAngle = yAngle;
		graphics.AddSphere(bullet, size,
			{
			{0.235, 0.294, 0.431},
			{0.235, 0.294, 0.431},
			{0.3500, 0.3500, 0.3500},
			static_cast<signed char>(16.0 / 1000 * 128),
			{0.9, 0.4, 0}
		 }
		);
		controller.AddSuicidal(bullet, 5);
		DynamicCylinder* body = physics.AddDynamicCylinder(bullet, 0.2, 0.4); {
			double radYAngle = yAngle * 2 * PI / 360;
			body->velocity.z = -speed * std::cos(radYAngle);
			body->velocity.x = -speed * std::sin(radYAngle);
			body->velocity.y = v_speed;
			body->friction = { 0, 0, 0 };
			controller.AddBullet(body);
		}
	}
	return bullet;
}

Entity* Game::AddLantern(Point location, float rotation)
{
	Entity* lantern = world.AddEntity(location); {
		lantern->yAngle = rotation;
		graphics.AddTriangleMesh(lantern, "lantern.obj");
		StaticCube* body = physics.AddStaticCube(lantern, { 1, 3, 1 });
		controller.AddLantern(body);

		Entity* lightPoint = world.AddTailEntity(lantern, { 0.5, 2, 0 }); {
			controller.lanternLight = illumination.AddSpot(lightPoint, { 1, 1, 0.5 }, 180);
		}		

	}
	return lantern;
}


//	*****************************************  //
//	**       Описание игрового мира	       **  //
//	*****************************************  //

void Game::Init()
{
	AddUserCar({ 0, 2, 10 });
	 
	AddLantern({ 15, 0, 14 }, 90);
	AddLantern({ 25, 0, 14 }, 90);
	AddLantern({ -17, 0, 9 }, 90);
	AddLantern({ -23, 0, 9 }, 90);

	Entity* plane = world.AddEntity({ 0, 0, 0 }); {
		graphics.AddPlane(plane, 100, 100, { "floor_148.jpg" }, 10, 10);
		physics.AddStaticCube(plane, {100, 0, 100});
	}

	Entity* car2 = AddTank({ 5,2, -10 });
	Entity* gun = AddGun({ 5,2, -15 });
	Entity* bus = AddBus({ 5,2, -20 });

	Entity* tree = world.AddEntity({ 0, 0, 0 }); {
		controller.AddWhirligig(tree);
		physics.AddDynamicCylinder(tree, 2, 10)->friction = { 1, 1, 1 };
		Entity* visuzl = world.AddTailEntity(tree, {0, -5, 0}); {
			visuzl->xAngle = -90;
			graphics.AddCone(visuzl, 2, 10,
				{
							  {0.235, 1, 0.431},
							  {0.235, 1, 0.431}
				}
				);
		}
	}

	Entity* lightPoint = world.AddEntity({ 0, 0, 3 }); {
		illumination.AddDirection(lightPoint, { 1, 1, 1 });
		lightPoint->xAngle = -70;
	}

	Entity* testCube = world.AddEntity({ 20, 0, 20 }); {
		physics.AddStaticCube(testCube, { 10, 10, 10 });
		graphics.AddCube(testCube, 10, {
							  {0.235, 0.294, 0.431},
							  {0.235, 0.294, 0.431},
							  {0.3500, 0.3500, 0.3500},
							  static_cast<signed char>(16.0 / 1000 * 128),
			});
	}
	//Entity* skull = world.AddEntity({ -10, 0, -10 }); {
	//	skull->xAngle = -90;
	//	graphics.AddTriangleMesh(skull, "..\\Lab06\\Resources\\Skull.obj");
	//}
	for (float i = 0; i < 10; ++i)
	{
		Entity* step = world.AddEntity({ -20, -2 + i / 3, i }); {
			float size = 3 + i / 20;
			graphics.AddCube(step, size, {
							  {0.235, 0.294, 0.431},
							  {0.235, 0.294, 0.431},
							  {0.3500, 0.3500, 0.3500},
							  static_cast<signed char>(16.0 / 1000 * 128),
				});
			physics.AddStaticCube(step, { size, size, size });
		}
	}
	Entity* platform = world.AddEntity({ -20, -7, 20}); {
		graphics.AddCube(platform,20);
		physics.AddStaticCube(platform, { 20, 20, 20 });
	}
}