#include "game.h"

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
			graphics.AddTorus(weel1, 0.13, 0.15);
		}
		Entity* weel2 = world.AddTailEntity(car, { -0.4, -0.5, 0.5 }); {
			weel2->yAngle = 90;
			graphics.AddTorus(weel2, 0.13, 0.15);
		}
		Entity* weel3 = world.AddTailEntity(car, { 0.4, -0.5, -0.5 }); {
			weel3->yAngle = 90;
			graphics.AddTorus(weel3, 0.13, 0.15);
		}
		Entity* weel4 = world.AddTailEntity(car, { -0.4, -0.5, -0.5 }); {
			weel4->yAngle = 90;
			graphics.AddTorus(weel4, 0.13, 0.15);
		}
	}
	return car;
}

Entity* Game::AddBullet(Point location, float yAngle)
{
	Entity* bullet = world.AddEntity(location); {
		bullet->yAngle = yAngle;
		graphics.AddSphere(bullet, 0.2);
		controller.AddBullet(bullet);
	}
	return bullet;
}


//	*****************************************  //
//	**       Описание игрового мира	       **  //
//	*****************************************  //


void Game::Init()
{
	Entity* car = AddCar({ 0, 0.65, 10 }, 0); {
		controller.AddSimpleUser(car);
		Entity* cameraPoint = world.AddTailEntity(car, { 0, 2, 10 }); {
			camera.parent = cameraPoint;
		}

		Entity* lightPoint = world.AddTailEntity(car, { 0, 0, -1 }); {
			illumination.AddSpot(lightPoint, {1, 1, 0.5}, 30);
		}
	}
	Entity* car2 = AddCar({ 5, 0.65, -10 }, 45);
	Entity* tree = world.AddEntity({ 0, 0, 0 }); {
		tree->xAngle = -90;
		graphics.AddCone(tree, 2, 10);
	}

	Entity* lightPoint = world.AddEntity({ 0, 0, 3 }); {
		illumination.AddDirection(lightPoint, { 1, 1, 1 });
		lightPoint->xAngle = -90;
	}

	Entity* testPoint = world.AddEntity({ -10, 0, -10 }); {
		testPoint->xAngle = -90;
		graphics.AddTriangleMesh(testPoint, "..\\Lab06\\Resources\\Skull.obj");
	}
}