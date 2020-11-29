#include "game.h"
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
	if (dt > 0.1)
		dt = 0.1;

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
		graphics.AddCube(car, 1);
		car->yAngle = rotation;

		Entity* weel1 = world.AddTailEntity(car, { 0.4, -0.5, 0.5 }); {
			weel1->yAngle = 90;
			graphics.AddTorus(weel1, 0.05, 0.2);
		}
		Entity* weel2 = world.AddTailEntity(car, { -0.4, -0.5, 0.5 }); {
			weel2->yAngle = 90;
			graphics.AddTorus(weel2, 0.05, 0.2);
		}
		Entity* weel3 = world.AddTailEntity(car, { 0.4, -0.5, -0.5 }); {
			weel3->yAngle = 90;
			graphics.AddTorus(weel3, 0.05, 0.2);
		}
		Entity* weel4 = world.AddTailEntity(car, { -0.4, -0.5, -0.5 }); {
			weel4->yAngle = 90;
			graphics.AddTorus(weel4, 0.05, 0.2);
		}

	}
	return car;
}

Entity* Game::AddUserCar(Point location)
{
	Entity* car = AddCar(location, 0); {
		DynamicCylinder* body = physics.AddDynamicCylinder(car, 0.7, 1.5); {
			controller.AddSimpleUser(body);
			body->friction = { 0.1, 0, 0.01 };
		}

		Entity* cameraPoint = world.AddTailEntity(car, { 0, 2, 10 }); {
			camera.parent = cameraPoint;
		}

		Entity* lightPoint = world.AddTailEntity(car, { 0, 0, -1 }); {
			illumination.AddSpot(lightPoint, { 1, 1, 0.5 }, 30);
		}
	}
	return car;
}

Entity* Game::AddBullet(Point location, float yAngle)
{
	Entity* bullet = world.AddEntity(location); {
		bullet->yAngle = yAngle;
		graphics.AddSphere(bullet, 0.2);
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


//	*****************************************  //
//	**       Описание игрового мира	       **  //
//	*****************************************  //

void Game::Init()
{
	AddUserCar({ 0, 2, 10 });

	Entity* plane = world.AddEntity({ 0, -50, 0 }); {
		graphics.AddCube(plane, 100);
		physics.AddStaticCube(plane, {100, 100, 100});
	}

	Entity* car2 = AddCar({ 5,2, -10 }, 45); {
		DynamicCylinder* body = physics.AddDynamicCylinder(car2, 0.7, 1.5);
	}

	Entity* tree = world.AddEntity({ 0, 0, 0 }); {
		physics.AddDynamicCylinder(tree, 2, 10)->friction = { 1, 1, 1 };
		Entity* visuzl = world.AddTailEntity(tree, {0, -5, 0}); {
			visuzl->xAngle = -90;
			graphics.AddCone(visuzl, 2, 10);
		}
	}

	Entity* lightPoint = world.AddEntity({ 0, 0, 3 }); {
		illumination.AddDirection(lightPoint, { 1, 1, 1 });
		lightPoint->xAngle = -70;
	}

	Entity* testCube = world.AddEntity({ 20, 0, 20 }); {
		physics.AddStaticCube(testCube, { 10, 10, 10 });
		graphics.AddCube(testCube, 10);
	}
	//Entity* skull = world.AddEntity({ -10, 0, -10 }); {
	//	skull->xAngle = -90;
	//	graphics.AddTriangleMesh(skull, "..\\Lab06\\Resources\\Skull.obj");
	//}
	for (float i = 0; i < 10; ++i)
	{
		Entity* step = world.AddEntity({ -20, -2 + i / 3, i }); {
			float size = 3 + i / 20;
			graphics.AddCube(step, size);
			physics.AddStaticCube(step, { size, size, size });
		}
	}
	Entity* platform = world.AddEntity({ -20, -7, 20}); {
		graphics.AddCube(platform, 20);
		physics.AddStaticCube(platform, { 20, 20, 20 });
	}
}