#pragma once

#include "garbage_collector.h"
#include "entity.h"
#include "camera.h"
#include "graphics.h"
#include "controller.h"

#include <gl\freeglut.h>
#include <iostream>

class World : public GarbageCollector<Entity>
{
	Camera camera;

public:
	Keyboard keys;

	Graphics graphics;
	Controller controller;

	World()
	{
		Init();
	}

	void Tick(double dt)
	{
		GarbageCollector::Tick();

		if (dt > 0.1)
			dt = 0.1;

		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
		glLoadIdentity();
		camera.TransformGL();

		controller.Tick(dt);
		graphics.Tick(dt);

		glFlush(); glutSwapBuffers();
	}

	Entity* AddEntity(Point location)
	{
		Entity* result = new Entity(location);
		GarbageCollector::AddTracking(result);
		return result;
	}

	TailEntity* AddTailEntity(Entity* parent, Point location)
	{
		TailEntity* result = new TailEntity(parent, location);
		GarbageCollector::AddTracking(result);
		return result;
	}

	void ResizeWindow(int width, int height)
	{
		glViewport(0, 0, width, height);
		camera.aspectRatio = width / (float)height;
	}


private:
	void Init()
	{
		Entity* car = AddCar({ 0, 0.65, 10 }, 0); {
			controller.AddSimpleUser(keys, car);
			Entity* cameraPoint = AddTailEntity(car, { 0, 2, 10 }); {
				camera.parent = cameraPoint;
			}
		}
		Entity* car2 = AddCar({ 5, 0.65, -10 }, 45);
		Entity* tree = AddEntity({ 0, 0, 0 }); {
			tree->xAngle = -90;
			graphics.AddCone(tree, 2, 10);
		}
	}

	Entity* AddCar(Point location, float rotation)
	{
		Entity* car = AddEntity(location); {
			graphics.AddCube(car, 1);
			car->yAngle = rotation;

			Entity* weel1 = AddTailEntity(car, { 0.4, -0.5, 0.5 }); {
				weel1->yAngle = 90;
				graphics.AddTorus(weel1, 0.13, 0.15);
			}
			Entity* weel2 = AddTailEntity(car, { -0.4, -0.5, 0.5 }); {
				weel2->yAngle = 90;
				graphics.AddTorus(weel2, 0.13, 0.15);
			}
			Entity* weel3 = AddTailEntity(car, { 0.4, -0.5, -0.5 }); {
				weel3->yAngle = 90;
				graphics.AddTorus(weel3, 0.13, 0.15);
			}
			Entity* weel4 = AddTailEntity(car, { -0.4, -0.5, -0.5 }); {
				weel4->yAngle = 90;
				graphics.AddTorus(weel4, 0.13, 0.15);
			}
		}
		return car;
	}
};
