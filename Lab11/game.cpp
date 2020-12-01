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
		AddUserCar({ 0, 2, 10 });

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

	}
	return car;
}

Entity* Game::AddTank(Point location, float rotation)
{
	Entity* car = world.AddEntity(location); {
		car->yAngle = rotation;

		graphics.AddTriangleMesh(car, "Tank.obj");

	}
	return car;
}


Entity* Game::AddGun(Point location, float rotation)
{
	Entity* car = world.AddEntity(location); {
		car->yAngle = rotation;

		graphics.AddTriangleMesh(car, "Gun.obj");

	}
	return car;
}


Entity* Game::AddBus(Point location, float rotation)
{
	Entity* car = world.AddEntity(location); {
		car->yAngle = rotation;

		graphics.AddTriangleMesh(car, "Bus.obj");

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
	}
	return car;
}

Entity* Game::AddBullet(Point location, float yAngle)
{
	Entity* bullet = world.AddEntity(location); {
		bullet->yAngle = yAngle;
		graphics.AddSphere(bullet, 0.2,
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
			body->velocity.z = -100 * std::cos(radYAngle);
			body->velocity.x = -100 * std::sin(radYAngle);
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
		graphics.AddPlane(plane, 100, 100, getTexture(Floor), 10, 10);
		physics.AddStaticCube(plane, { 100, 0, 100 });
	}

	Entity* car2 = AddTank({ 5,2, -10 }, 45); {
		DynamicCylinder* body = physics.AddDynamicCylinder(car2, 0.7, 0.7);
	}

	Entity* gun = AddGun({ 5,2, -15 }, 45); {
		DynamicCylinder* body = physics.AddDynamicCylinder(gun, 0.7, 0.7);
	}

	Entity* bus = AddBus({ 5,2, -20 }, 90); {
		DynamicCylinder* body = physics.AddDynamicCylinder(bus, 0.7, 1.1);
	}

	Entity* tree = world.AddEntity({ 0, 2, 0 }); {
		controller.AddWhirligig(tree);
		physics.AddDynamicCylinder(tree, 2, 10)->friction = { 1, 1, 1 };
		Entity* visuzl = world.AddTailEntity(tree, { 0, -5, 0 }); {
			visuzl->xAngle = -90;
			graphics.AddCone(visuzl, 2, 10,
				{
							  {0.07, 0.59, 0.28},
							  {0.07, 0.59, 0.28}
				}
			);
		}
		srand(time(0));
		Entity* star = world.AddTailEntity(tree, { 0, 5, 0 }); {
			star->xAngle = -90;
			graphics.AddTriangleMesh(star, "star.obj");
		}
		for (float i = -2; i <= 2; i += 4)
		{
			Entity* ball = world.AddTailEntity(tree, { i, -4, 0 }); {
				graphics.AddSphere(ball, 0.2, getTexture((textures)(rand() % 11)));
			}
			Entity* ball2 = world.AddTailEntity(tree, { 0, -4, i }); {
				graphics.AddSphere(ball2, 0.2, getTexture((textures)(rand() % 11)));
			}
		}
		for (float i = -1.3; i <= 1.5; i += 2.6)
		{
			Entity* ball1 = world.AddTailEntity(tree, { i, -3.4, i }); {
				graphics.AddSphere(ball1, 0.2, getTexture((textures)(rand() % 11)));
			}
			Entity* ball2 = world.AddTailEntity(tree, { i * -1, -3.4, i }); {
				graphics.AddSphere(ball2, 0.2, getTexture((textures)(rand() % 11)));
			}
		}
		for (float i = -1.5; i <= 1.5; i += 3)
		{
			Entity* ball1 = world.AddTailEntity(tree, { i, -1.7, 0 }); {
				graphics.AddSphere(ball1, 0.2, getTexture((textures)(rand() % 11)));
			}
			Entity* ball2 = world.AddTailEntity(tree, { 0, -1.7, i }); {
				graphics.AddSphere(ball2, 0.2, getTexture((textures)(rand() % 11)));
			}
		}
		for (float i = -0.8; i <= 1; i += 1.6)
		{
			Entity* ball1 = world.AddTailEntity(tree, { i, 0, i }); {
				graphics.AddSphere(ball1, 0.2, getTexture((textures)(rand() % 11)));
			}
			Entity* ball2 = world.AddTailEntity(tree, { i * -1, 0, i }); {
				graphics.AddSphere(ball2, 0.2, getTexture((textures)(rand() % 11)));
			}
		}
		for (float i = -0.8; i <= 1; i += 1.6)
		{
			Entity* ball1 = world.AddTailEntity(tree, { i, 1.7, 0 }); {
				graphics.AddSphere(ball1, 0.2, getTexture((textures)(rand() % 11)));
			}
			Entity* ball2 = world.AddTailEntity(tree, { 0, 1.7, i }); {
				graphics.AddSphere(ball2, 0.2, getTexture((textures)(rand() % 11)));
			}
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
	Entity* platform = world.AddEntity({ -20, -7, 20 }); {
		graphics.AddCube(platform, 20);
		physics.AddStaticCube(platform, { 20, 20, 20 });
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
