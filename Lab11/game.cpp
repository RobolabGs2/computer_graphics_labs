#include "game.h"

#include <SOIL2.h>

#include "common.h"

#include <Windows.h>
#include <gl\freeglut.h>

#include <math.h>
#include <ctime>

//	*****************************************  //
//	**                Game                 **  //
//	*****************************************  //

Game::Game() :
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
	if (!tree->alive) tree = AddTree();

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

		Entity* lightPoint = world.AddEntity({ location.x, location.y + 2.0f, location.z - 0.5f }); {
			controller.lanternLight.push_back(illumination.AddSpot(lightPoint, { 1, 1, 0.5 }, 180));
		}
	}
	return lantern;
}

Entity* Game::AddHouse(Point location)
{
	Entity* house = world.AddEntity(location+Point({0, 7.5, 0})); {
		graphics.AddTriangleMesh(house, "House.obj");
		physics.AddStaticCube(house, { 10, 15, 10 });
	}
	return house;
}


Entity* Game::AddTree()
{
	Entity* tree = world.AddEntity({ 0, 10, 0 }); {
		controller.AddWhirligig(tree);
		physics.AddDynamicCylinder(tree, 2, 10)->friction = { 1, 0.1, 1 };
		Entity* visuzl = world.AddTailEntity(tree, { 0, -5, 0 }); {
			visuzl->xAngle = -90;
			graphics.AddCone(visuzl, 2, 10,
				{
							  {0.07, 0.59, 0.28},
							  {0.07, 0.59, 0.28}
				}
			);
		}
		Entity* star = world.AddTailEntity(tree, { 0, 5, 0 }); {
			star->xAngle = -90;
			graphics.AddTriangleMesh(star, "star.obj");
		}

		for (float i = 0; i < 90; ++i) {
			float rad = (100 - i) / 50;
			Entity* ball = world.AddTailEntity(tree, { rad * std::sin(i), i / 10 - 5, rad * std::cos(i) }); {
				graphics.AddSphere(ball, 0.2, getTexture((textures)(rand() % 11)));
			}
		}

		for (float i = 0; i < 100; ++i) {
			float rad = (100 - i) / 50;
			Entity* ball = world.AddTailEntity(tree, { rad * std::sin(i + 0.5f), i / 10 - 5, rad * std::cos(i + 0.5f) }); {
				graphics.AddSphere(ball, 0.05, Material({ 0, 0, 0 }, { 0.18, 0.1, 1 }, { 1, 1, 1 }, '\111', rand() % 3 == 0 ? Color{ 0, 0, 1 }: 
					rand() % 3 == 0 ? Color{ 1, 0, 0 }: Color{0, 1, 0}));
			}
		}

		//for (float i = -2; i <= 2; i += 4)
		//{
		//	Entity* ball = world.AddTailEntity(tree, { i, -4, 0 }); {
		//		graphics.AddSphere(ball, 0.2, getTexture((textures)(rand() % 11)));
		//	}
		//	Entity* ball2 = world.AddTailEntity(tree, { 0, -4, i }); {
		//		graphics.AddSphere(ball2, 0.2, getTexture((textures)(rand() % 11)));
		//	}
		//}
		//for (float i = -1.3; i <= 1.5; i += 2.6)
		//{
		//	Entity* ball1 = world.AddTailEntity(tree, { i, -3.4, i }); {
		//		graphics.AddSphere(ball1, 0.2, getTexture((textures)(rand() % 11)));
		//	}
		//	Entity* ball2 = world.AddTailEntity(tree, { i * -1, -3.4, i }); {
		//		graphics.AddSphere(ball2, 0.2, getTexture((textures)(rand() % 11)));
		//	}
		//}
		//for (float i = -1.5; i <= 1.5; i += 3)
		//{
		//	Entity* ball1 = world.AddTailEntity(tree, { i, -1.7, 0 }); {
		//		graphics.AddSphere(ball1, 0.2, getTexture((textures)(rand() % 11)));
		//	}
		//	Entity* ball2 = world.AddTailEntity(tree, { 0, -1.7, i }); {
		//		graphics.AddSphere(ball2, 0.2, getTexture((textures)(rand() % 11)));
		//	}
		//}
		//for (float i = -0.8; i <= 1; i += 1.6)
		//{
		//	Entity* ball1 = world.AddTailEntity(tree, { i, 0, i }); {
		//		graphics.AddSphere(ball1, 0.2, getTexture((textures)(rand() % 11)));
		//	}
		//	Entity* ball2 = world.AddTailEntity(tree, { i * -1, 0, i }); {
		//		graphics.AddSphere(ball2, 0.2, getTexture((textures)(rand() % 11)));
		//	}
		//}
		//for (float i = -0.8; i <= 1; i += 1.6)
		//{
		//	Entity* ball1 = world.AddTailEntity(tree, { i, 1.7, 0 }); {
		//		graphics.AddSphere(ball1, 0.2, getTexture((textures)(rand() % 11)));
		//	}
		//	Entity* ball2 = world.AddTailEntity(tree, { 0, 1.7, i }); {
		//		graphics.AddSphere(ball2, 0.2, getTexture((textures)(rand() % 11)));
		//	}
		//}
	}
	return tree;
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

	AddHouse({ 30, 0, 35 });
	AddHouse({ -30, 0, -30 });
	AddHouse({ -400, 0, 10 });
	
	Entity* plane = world.AddEntity({ 0, -10, 0 }); {
		physics.AddStaticCube(plane, { 100, 20, 100 });
		Entity* local = world.AddTailEntity(plane, { 0, 10, 0 }); {
			graphics.AddPlane(local, 100, 100, getTexture(Floor), 100, 100);
		}
	}

	Entity* car2 = AddTank({ 5,2, -10 });
	Entity* gun = AddGun({ 5,2, -15 });
	Entity* bus = AddBus({ 5,2, -20 });

	tree = AddTree();

	Entity* lightPoint = world.AddEntity({ 0, 0, 3 }); {
		illumination.AddDirection(lightPoint, { 0.5, 0.5, 0.5 });
		lightPoint->xAngle = -70;
	}

	Entity* testCube = world.AddEntity({ 20, 0, 20 }); {
		physics.AddStaticCube(testCube, { 10, 10, 10 });
		graphics.AddCube(testCube, 10, {"house_panel.jpg"});
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
	Entity* platform = world.AddEntity({ -20, 0, 20}); {
		graphics.AddBox(platform,{20, 5, 20});
		physics.AddStaticCube(platform, { 20, 5, 20 });
	}
}

Material Game::getTexture(textures name)
{
	switch (name)
	{
	case(Ball): return Material("ball.jpg");
		break;
	case(Flowers): return Material("flowers.jpg");
		break;
	case(Glass): return Material("glass.jpg");
		break;
	case(GoldenGlitter): return Material("golden-glitter.jpg");
		break;
	case(GoldenGreen): return Material("golden-green.jpg");
		break;
	case(Santa): return Material("santa.jpg");
		break;
	case(Xmas): return Material("xmas.jpg");
		break;
	case(Pink): return Material({0, 0, 0}, { 0.86, 0.1, 0.77 }, { 1, 1, 1 }, '\111');
		break;
	case(White): return Material({ 0, 0, 0 }, { 1, 1, 1 }, { 1, 1, 1 }, '\111');
		break;
	case(Yellow): return Material({ 0, 0, 0 }, { 0.9, 0.8, 0.07 },  { 1, 1, 1 }, '\111');
		break;
	case(Blue): return Material({ 0, 0, 0 }, { 0.18, 0.1, 1 }, { 1, 1, 1 }, '\111');
		break;
	case(Tree): return Material("tree.jpg");
		break;
	case(Floor): return Material("floor_148.jpg");
		break;
	default:
		break;
	}
}
